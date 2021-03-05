using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using EtherMon.Clients;
using EtherMon.Extensions;
using EtherMon.Models;
using EtherMon.Models.Ethermine;
using EtherMon.Services;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;

namespace EtherMon.ViewModels
{
    public class FrontViewModel : BaseViewModel
    {
        public List<MinerAddress> ExistingAddresses { get; private set; }
        public Statistics? CurrentStats { get; private set; }
        public Dashboard? CurrentDashboard { get; private set; }
        public PoolStatistic? CurrentPoolInfo { get; private set; }
        public LineChart? CurrentLineChart { get; }
        public LineChart? ReportedLineChart { get; }
        public string? EthUsdRate { get; private set; }
        public string? EthBtcRate { get; private set; }
        public string? CurrentAddress;
        private EthermineClient _ethermineClient;
        private AddressesDataStore _addressesDataStore;
        private DateTime? _lastMinerRequestDate;
        private bool _isRefreshing;
        public ICommand RefreshCommand { get; }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        private int _height;
        public int WorkersListHeight
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged("WorkersListHeight");
            }
        }
        private async void ExecuteRefreshCommand()
        {
            await UpdateData();
            // Stop refreshing
            IsRefreshing = false;
        }
        
        public FrontViewModel()
        {
            ExistingAddresses = new List<MinerAddress>();
            
            _addressesDataStore = DependencyService.Get<AddressesDataStore>();
            _ethermineClient = new EthermineClient();
            WorkersListHeight = 0;
            
            CurrentLineChart = new LineChart();
            CurrentLineChart.LineMode = LineMode.Straight;
            CurrentLineChart.PointSize = 1;
            CurrentLineChart.IsAnimated = false;
            CurrentLineChart.BackgroundColor = SKColor.Empty;
            
            ReportedLineChart = new LineChart();
            ReportedLineChart.LineMode = LineMode.Straight;
            ReportedLineChart.PointSize = 1;
            ReportedLineChart.IsAnimated = false;
            ReportedLineChart.BackgroundColor = SKColor.Empty;
            
            if (CurrentLineChart != null) CurrentLineChart.Entries = new List<ChartEntry>();
            if (ReportedLineChart != null) ReportedLineChart.Entries = new List<ChartEntry>();
            
            RefreshCommand = new Command(ExecuteRefreshCommand);
            
            AsyncInitializer();
        }
        
        public async Task<MinerAddress?> GetFavouriteOrFirstAddress()
        {
            return await _addressesDataStore.GetItemAsync();
        }
        
        public void OnSelectedAddressChanged(object? pickerSelectedItem)
        {
            _lastMinerRequestDate = null;
            AsyncInitializer(pickerSelectedItem!.ToString());
        }
        
        private async void AsyncInitializer()
        {
            CurrentAddress = (await GetFavouriteOrFirstAddress())?.Address;
            if (string.IsNullOrEmpty(CurrentAddress)) return;
            _ethermineClient.MinerAddress = CurrentAddress;
            await UpdateData();
        }

        private async void AsyncInitializer(string newAddress)
        {
            CurrentStats = null;
            CurrentDashboard = null;
            
            if (string.IsNullOrEmpty(CurrentAddress)) return;
            
            CurrentAddress = newAddress;
            await _addressesDataStore.UpdateFavourite(newAddress);
            
            _ethermineClient.MinerAddress = CurrentAddress;
            
            Debug.WriteLine($"Reinitializing with address {newAddress}");
            await UpdateData();
        }

        public async Task UpdateData()
        {
            if (_lastMinerRequestDate != null && DateTime.Now.AddMinutes(-2) < _lastMinerRequestDate)
            {
                MessagingCenter.Send(this, "two_minutes_caching_warning");
                return;
            }
            
            Debug.WriteLine("Updating data");
            IsRefreshing = true;

            CurrentStats = await _ethermineClient.GetStatisticData();
            OnPropertyChanged("CurrentStats");
            
            CurrentDashboard = await _ethermineClient.GetDashboardData();
            if (CurrentDashboard?.data != null)
            {
                CurrentDashboard.data.statistics =
                    StatsExtensions.GetFilledUpAndClearedStats(CurrentDashboard.data.statistics);
                WorkersListHeight = CurrentDashboard.data!.workers!.Count * 32;
            }
            OnPropertyChanged("CurrentDashboard");
            
            ExistingAddresses = await _addressesDataStore.GetItemsAsync();
            MessagingCenter.Send(this, "update_addresses_dropdown");
            
            await UpdatePoolInfo();
            UpdateChart();
            
            IsRefreshing = false;
            _lastMinerRequestDate = DateTime.Now;
            Debug.WriteLine("Data Updated");
        }

        private void UpdateChart()
        {
            Debug.WriteLine("Updating charts");
            
            var currentHsEntries = new List<ChartEntry>();
            var reportedHsEntries = new List<ChartEntry>();

            if (CurrentDashboard?.data == null)
            {
                if (CurrentLineChart != null) CurrentLineChart.Entries = currentHsEntries;
                if (ReportedLineChart != null) ReportedLineChart.Entries = currentHsEntries;
                OnPropertyChanged("CurrentLineChart");
                return;
            }

            double maxCurrentHashrate = 0;
            double maxReportedHashrate = 0;
            
            CurrentDashboard.data.statistics?.ForEach(s =>
            {
                if (s.currentHashrate > maxCurrentHashrate) maxCurrentHashrate = s.currentHashrate.Value;
                if (s.reportedHashrate > maxReportedHashrate) maxReportedHashrate = s.reportedHashrate.Value;
                
                currentHsEntries.Add(new ChartEntry(float.Parse(s.currentHashrate.ToString()))
                {
                    Color = SKColor.Parse("#77d065")
                });
                reportedHsEntries.Add(new ChartEntry(float.Parse(s.reportedHashrate.ToString()))
                {
                    Color = SKColor.Parse("#3498db")
                });
            });

            var maxChartValue = Math.Round(Math.Max(maxCurrentHashrate, maxReportedHashrate) * 1.1);
            if (CurrentLineChart != null)
            {
                CurrentLineChart.Entries = currentHsEntries;
                CurrentLineChart.MaxValue = (float)maxChartValue;
                CurrentLineChart.MinValue = 0;
            };
            if (ReportedLineChart != null)
            {
                ReportedLineChart.Entries = reportedHsEntries;
                ReportedLineChart.MaxValue = (float)maxChartValue;
                ReportedLineChart.MinValue = 0;
            };
            OnPropertyChanged("CurrentLineChart");
            OnPropertyChanged("ReportedLineChart");
            Debug.WriteLine("Charts Updated");
        }

        private async Task UpdatePoolInfo()
        {
            Debug.WriteLine("Updating pool info");
            CurrentPoolInfo = await _ethermineClient.GetPoolStats();
            
            if (CurrentPoolInfo == null) return;
            
            EthUsdRate = Math.Round(CurrentPoolInfo.data.price.usd, 2).ToString(CultureInfo.CurrentCulture);
            OnPropertyChanged("EthUsdRate");
            
            EthBtcRate = Math.Round(CurrentPoolInfo.data.price.btc, 4).ToString(CultureInfo.CurrentCulture);
            OnPropertyChanged("EthBtcRate");
            Debug.WriteLine("Pool info Updated");
        }
    }
}