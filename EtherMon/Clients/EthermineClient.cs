using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using EtherMon.Models.Ethermine;
using Newtonsoft.Json;

namespace EtherMon.Clients
{
    public class EthermineClient
    {
        public string? MinerAddress { get; set; }
        
        private DateTime _lastMinerRequestDate { get; set; }
        private DateTime _lastWorkersRequestDate { get; set; }
        private static string _baseUrl = "https://api.ethermine.org";
        private HttpClient _httpClient;
        
        public EthermineClient(string? address = null)
        {
            MinerAddress = address;

            _httpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};
        }

        public async Task<Dashboard?> GetDashboardData()
        {
            if (MinerAddress == null) return null;

            var response = await _httpClient.GetAsync($"/miner/{MinerAddress}/dashboard");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            try
            {
                Dashboard dashboard = JsonConvert.DeserializeObject<Dashboard>(responseContent);
                return dashboard;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<Statistics?> GetStatisticData()
        {
            if (MinerAddress == null) return null;
            
            var response = await _httpClient.GetAsync($"/miner/{MinerAddress}/currentStats");
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                Statistics stats = JsonConvert.DeserializeObject<Statistics>(responseContent);            
                return stats;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<PoolStatistic?> GetPoolStats()
        {
            var response = await _httpClient.GetAsync("/poolStats");
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                PoolStatistic stats = JsonConvert.DeserializeObject<PoolStatistic>(responseContent);
                return stats;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}