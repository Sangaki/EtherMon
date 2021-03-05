using System.Collections.Generic;
using System.Threading.Tasks;
using EtherMon.Models;

namespace EtherMon.Services
{
    public interface IDataStore<T>
    {
        Task<int> AddItemAsync(T item);
        Task<int> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<List<MinerAddress>> GetItemsAsync();
    }
}
