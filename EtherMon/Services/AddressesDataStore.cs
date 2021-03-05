using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EtherMon.Extensions;
using EtherMon.Models;
using SQLite;

namespace EtherMon.Services
{
    public class AddressesDataStore : IDataStore<MinerAddress>
    {
        static readonly Lazy<SQLiteAsyncConnection> LazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        private static SQLiteAsyncConnection Database => LazyInitializer.Value;
        private static bool _initialized;

        public AddressesDataStore()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        private static async Task InitializeAsync()
        {
            if (!_initialized)
            {
                if (Database.TableMappings.All(m => m.MappedType.Name != nameof(MinerAddress)))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(MinerAddress)).ConfigureAwait(false);
                }
                _initialized = true;
            }
        }
        
        public async Task<int> AddItemAsync(MinerAddress item)
        {
            return await Database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(string id)
        {
            var oldItem = await Database.Table<MinerAddress>().Where(a => a.Id == id).FirstOrDefaultAsync();
            
            return await Database.DeleteAsync(oldItem);
        }

        public async Task<MinerAddress> GetItemAsync(string id)
        {
            return await Database.Table<MinerAddress>().Where(a => a.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<MinerAddress?> GetItemAsync()
        {
            var favouriteAddress = await Database.Table<MinerAddress>().Where(a => a.IsFavourite).FirstOrDefaultAsync();
            if (favouriteAddress != null) return favouriteAddress;
            var anyAddress = await Database.Table<MinerAddress>().FirstOrDefaultAsync();
            return anyAddress;
        }

        public async Task<List<MinerAddress>> GetItemsAsync()
        {
            return await Database.Table<MinerAddress>().ToListAsync();
        }

        public async Task<MinerAddress?> AnyFavouriteAddress()
        {
            return await Database.Table<MinerAddress>().Where(a => a.IsFavourite).FirstOrDefaultAsync();
        } 

        public async Task UpdateFavourite(string address)
        {
            var favourites = await Database.Table<MinerAddress>()
                .Where(a => a.IsFavourite && a.Address != address)
                .ToListAsync();
            if (favourites != null)
            {
                favourites.ForEach(f => f.IsFavourite = false);

                var newFavourite = await Database.Table<MinerAddress>().Where(a => a.Address == address).FirstOrDefaultAsync();
                if (newFavourite != null)
                {
                    newFavourite.IsFavourite = true;
                    favourites.Add(newFavourite);
                }
                await Database.UpdateAllAsync(favourites);
            }
        } 
    }
}