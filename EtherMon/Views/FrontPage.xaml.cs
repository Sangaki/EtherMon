using System.Threading.Tasks;
using EtherMon.Interfaces;
using EtherMon.ViewModels;
using Xamarin.Forms;

namespace EtherMon.Views
{
    public partial class FrontPage
    {
        private FrontViewModel _frontViewModel;
        
        public FrontPage()
        {
            _frontViewModel = new FrontViewModel();
            BindingContext = _frontViewModel;

            var toastService = DependencyService.Get<IMessage>();
            
            InitializeComponent();
            
            InitUpdateAddress();
            
            MessagingCenter.Subscribe<SettingsViewModel>(this, "update_address", async sender =>
            {
                await _frontViewModel.UpdateData();
                await InitUpdateAddress();
            });
            
            MessagingCenter.Subscribe<FrontViewModel>(this, "update_addresses_dropdown", async sender =>
            {
                await InitUpdateAddress();
            });
            
            MessagingCenter.Subscribe<FrontViewModel>(this, "two_minutes_caching_warning", sender =>
            {
                toastService.LongAlert("Ethermine information is cached for 2 minutes so there is no point in making more frequent updates.");
            });
        }

        private async Task InitUpdateAddress()
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
            
            Shell.SetTitleView(this, addressPicker);
        }
    }
}