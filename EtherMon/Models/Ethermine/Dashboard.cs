using System;
using System.Collections.Generic;
using System.Diagnostics;
using EtherMon.Extensions;

namespace EtherMon.Models.Ethermine
{
    public class Dashboard
    {
        public DashboardData? data { get; set; }
    }

    public class DashboardData
    {
        public List<DashboardDataStatistic> statistics { get; set; }
        public List<DashboardDataWorker>? workers { get; set; }
        public DashboardDataCurrentStatistics? currentStatistics { get; set; }
        public DashboardDataSettings? settings { get; set; }
    }

    public class DashboardDataStatistic
    {
        public double? time { get; set; }
        private double? reportedHashrateRounded { get; set; }
        public double? reportedHashrate
        {
            get => reportedHashrateRounded;
            set => reportedHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }
        private double? currentHashrateRounded { get; set; }
        public double? currentHashrate
        {
            get => currentHashrateRounded;
            set => currentHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }
        public int? validShares { get; set; }
        public int? invalidShares { get; set; }
        public int? staleShares { get; set; }
    }

    public class DashboardDataWorker : DashboardDataStatistic
    {
        public string? worker { get; set; }
        public double? lastSeen { get; set; }
    }

    public class DashboardDataCurrentStatistics : DashboardDataStatistic
    {
        public double? lastSeen { get; set; }
        private double? averageHashrateRounded { get; set; }
        public double? averageHashrate
        {
            get => averageHashrateRounded;
            set => averageHashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }
        public int? activeWorkers { get; set; }
        public double? unpaid { get; set; }
        public double? unconfirmed { get; set; }
    }

    public class DashboardDataSettings
    {
        public string? email { get; set; }
        public int? monitor { get; set; }
        public double? minPayout { get; set; }
    }
}