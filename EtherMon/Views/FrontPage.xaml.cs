using EtherMon.ViewModels;
using Xamarin.Forms;

namespace EtherMon.Views
{
    public partial class FrontPage : ContentPage
    {
        private FrontViewModel _frontViewModel;
        
        public FrontPage()
        {
            _frontViewModel = new FrontViewModel();
            BindingContext = _frontViewModel;
            
            InitializeComponent();
            
            InitUpdateAddress();
            
            MessagingCenter.Subscribe<SettingsViewModel>(this, "update_address", sender =>
            {
                _frontViewModel.UpdateData();
                InitUpdateAddress();
            });
            
            MessagingCenter.Subscribe<FrontViewModel>(this, "update_addresses_dropdown", sender =>
            {
                InitUpdateAddress();
            });
        }

        private async void InitUpdateAddress()
        {
            var favouriteOrFirstAddress = await _frontViewModel.GetFavouriteOrFirstAddress();
            if (favouriteOrFirstAddress == null) return;
            UpdatePageTitleDropdown(favouriteOrFirstAddress.Address);
        }
        
        private void UpdatePageTitleDropdown(string address)
        {
            Picker addressPicker = new Picker();
            var existingAddressesOptions = _frontViewModel
                .ExistingAddresses
                .ConvertAll(e => $"[{(e.IsSoloMiner ? "solo" : "pool")}]{e.Address}");
            addressPicker.ItemsSource = existingAddressesOptions;
            addressPicker.Title = $"Current: {address}";
            addressPicker.TitleColor = Color.Bisque;
            addressPicker.FontSize = 12;
            if (_frontViewModel.CurrentAddress != null)
            {
                addressPicker.SelectedItem = _frontViewModel.CurrentAddress;
            }

            addressPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (!(sender is Picker picker)) return;
                _frontViewModel.OnSelectedAddressChanged(picker.SelectedItem.ToString().Substring(6));
            };
            
            Shell.SetTitleView(this, (View)addressPicker);
        }
    }
}