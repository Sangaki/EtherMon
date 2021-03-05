using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using EtherMon.Clients;
using EtherMon.Models;
using EtherMon.Models.Ethermine;
using EtherMon.Services;
using Microcharts;
using Xamarin.Forms;

namespace EtherMon.ViewModels
{
    public class FrontViewModel : BaseViewModel
    {
        public List<MinerAddress> ExistingAddresses { get; private set; }
        public string? CurrentAddress;
        private EthermineClient _ethermineClient;
        private AddressesDataStore _addressesDataStore;
        public Statistics? CurrentStats { get; private set; }
        public Dashboard? CurrentDashboard { get; private set; }
        public PoolStatistic? CurrentPoolInfo { get; private set; }
        public LineChart? CurrentLineChart { get; private set; }
        public string? EthUsdRate { get; private set; }
        public string? EthBtcRate { get; private set; }
        
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
            
            RefreshCommand = new Command(ExecuteRefreshCommand);
            
            AsyncInitializer();
        }
        
        public async Task<MinerAddress?> GetFavouriteOrFirstAddress()
        {
            return await _addressesDataStore.GetItemAsync();
        }
        
        public void OnSelectedAddressChanged(object? pickerSelectedItem)
        {
            Debug.WriteLine(pickerSelectedItem!.ToString());
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
            CurrentLineChart = new LineChart();
            
            if (string.IsNullOrEmpty(CurrentAddress)) return;
            
            CurrentAddress = newAddress;
            await _addressesDataStore.UpdateFavourite(newAddress);
            
            _ethermineClient.MinerAddress = CurrentAddress;
            
            Debug.WriteLine($"Reinitializing with address {newAddress}");
            await UpdateData();
        }

        public async Task UpdateData()
        {
            Debug.WriteLine("Updating data");
            CurrentLineChart = new LineChart();
            
            CurrentStats = await _ethermineClient.GetStatisticData();
            OnPropertyChanged("CurrentStats");
            
            CurrentDashboard = await _ethermineClient.GetDashboardData();
            OnPropertyChanged("CurrentDashboard");
            
            ExistingAddresses = await _addressesDataStore.GetItemsAsync();
            MessagingCenter.Send(this, "update_addresses_dropdown");
            
            await UpdatePoolInfo();
            UpdateChart();
            
            Debug.WriteLine("Data Updated");
        }

        private void UpdateChart()
        {
            Debug.WriteLine("Updating chart");
            
            var entries = new List<ChartEntry>();

            if (CurrentDashboard?.data == null)
            { 
                entries.Add(new ChartEntry(0));
                CurrentLineChart = new LineChart {Entries = entries};
                OnPropertyChanged("CurrentLineChart");
                return;
            }

            CurrentDashboard.data.statistics.ForEach(s =>
            {
                entries.Add(new ChartEntry(float.Parse(s.currentHashrate.ToString())));
            });
            CurrentLineChart = new LineChart {Entries = entries};
            OnPropertyChanged("CurrentLineChart");
            Debug.WriteLine("Chart Updated");
        }

        private async Task UpdatePoolInfo()
        {
            Debug.WriteLine("updating pool info");
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