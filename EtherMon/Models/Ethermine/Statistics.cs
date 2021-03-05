using System;

namespace EtherMon.Models.Ethermine
{
    public class Statistics
    {
        public StatisticData? data { get; set; }
    }

    public class StatisticData
    {
        public double? time { get; set; }
        public double? lastSeen { get; set; }
        private double? reportedHashrateRounded { get; set; }
        public double? reportedHashrate
        {
            get => reportedHashrateRounded;
            set => reportedHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }

        private double? averageHashrateRounded { get; set; }
        public double? averageHashrate
        {
            get => averageHashrateRounded;
            set => averageHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }

        private double? currentHashrateRounded { get; set; }
        public double? currentHashrate
        {
            get => currentHashrateRounded;
            set => currentHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }
        public double? validShares { get; set; }
        public double? invalidShares { get; set; }
        public double? staleShares { get; set; }
        public int? activeWorkers { get; set; }

        private double? roundedUnpaid { get; set; }
        public double? unpaid
        {
            get => roundedUnpaid;
            set
            {
                roundedUnpaid = Math.Round(value!.Value / Math.Pow(10, 18), 5);
            }
        }

        public double? unconfirmed { get; set; }
        public double? coinsPerMin { get; set; }
        public double? usdPerMin { get; set; }
        public double? btcPerMin { get; set; }
    }
}