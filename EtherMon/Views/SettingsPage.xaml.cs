using System;
using System.Diagnostics;
using System.Windows.Input;
using EtherMon.Models;
using EtherMon.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EtherMon.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;
        
        public SettingsPage()
        {
            _viewModel = new SettingsViewModel();
            BindingContext = _viewModel;
            RefreshSavedAddresses();
            InitializeComponent();

            AddressEntry.TextChanged += AddressChangedHandler;
            AddAddressButton.Clicked += OnAddAddressClicked;
        }

        private async void OnAddAddressClicked(object sender, EventArgs args)
        {
            if (!CheckAddressEntryState()) return;
            
            var newAddress = new MinerAddress()
            {
                Id = Guid.NewGuid().ToString(),
                Address = AddressEntry.Text,
                Coin = "ETH",
                IsSoloMiner = SoloRadio.IsChecked,
                IsFavourite = false
            };
            await _viewModel.AddNewAddress(newAddress);
        }

        private void AddressChangedHandler(object sender, EventArgs e)
        {
            CheckAddressEntryState();
        }
        
        private bool CheckAddressEntryState()
        {
            var isValid = !string.IsNullOrEmpty(AddressEntry.Text);
            var visualState = isValid ? "Valid" : "Invalid";
            VisualStateManager.GoToState(AddressEntry, visualState);
            return isValid;
        }
        
        private async void RefreshSavedAddresses()
        {
            await _viewModel.FetchAddressesFromDb();
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            AddressesListView.SelectedItem = null;
        }
    }
}