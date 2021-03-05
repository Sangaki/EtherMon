using SQLite;

namespace EtherMon.Models
{
    public class MinerAddress
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Address { get; set; }
        public string Coin { get; set; }
        public bool IsSoloMiner { get; set; }
        public bool IsFavourite { get; set; }
    }
}