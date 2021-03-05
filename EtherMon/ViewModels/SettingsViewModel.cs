using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using EtherMon.Models;
using EtherMon.Services;
using Xamarin.Forms;

namespace EtherMon.ViewModels
{
    public class SettingsViewModel: BaseViewModel
    {
        private readonly AddressesDataStore _dataStore;
        public List<MinerAddress> MinerAddresses { get; set; }

        private int _height;
        public int AddressesListHeight
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged("AddressesListHeight");
            }
        }
        
        public ICommand DeleteCommand => new Command<MinerAddress>(async (MinerAddress address) =>
        {
            Debug.WriteLine("Executing delete command");
            await DeleteAddressFromDb(address.Id);
        });
        
        public SettingsViewModel()
        {
            _dataStore = DependencyService.Get<AddressesDataStore>();
            Title = "Settings";
            AddressesListHeight = 0;
            MinerAddresses = new List<MinerAddress>();

            FetchAddressesFromDb();
        }

        public async Task AddNewAddress(MinerAddress item)
        {
            if (await _dataStore.AnyFavouriteAddress() == null)
            {
                item.IsFavourite = true;
            }
            
            await _dataStore.AddItemAsync(item);
            
            await FetchAddressesFromDb();
        }

        public async Task FetchAddressesFromDb()
        {
            MinerAddresses = await _dataStore.GetItemsAsync();
            
            AddressesListHeight = MinerAddresses.Count * 40;
            
            MessagingCenter.Send(this, "update_address");
            
            OnPropertyChanged(nameof(MinerAddresses));
        }

        public async Task DeleteAddressFromDb(string id)
        {
            await _dataStore.DeleteItemAsync(id);

            await FetchAddressesFromDb();
        }
    }
}