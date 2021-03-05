using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EtherMon.Models.Ethermine;

namespace EtherMon.Extensions
{
    public class StatsExtensions
    {
        public static List<DashboardDataStatistic> GetFilledUpAndClearedStats(List<DashboardDataStatistic> pureStats)
        {
            List<DashboardDataStatistic> clearStatistics = new List<DashboardDataStatistic>();
            var lastTimestamp = pureStats[pureStats.Count - 1].time;

            if (lastTimestamp == null) return pureStats;
            for (var i = 0; i < 100; i++)
            {
                var statInTime = pureStats.FirstOrDefault(ps => (int)ps.time! == (int)(lastTimestamp! - i * 600));
                if (statInTime == null)
                {
                    clearStatistics.Add(new DashboardDataStatistic()
                    {
                        reportedHashrate = 0,
                        currentHashrate = 0,
                        invalidShares = 0,
                        staleShares = 0,
                        validShares = 0,
                        time = lastTimestamp - i * 600
                    });
                }
                else
                {
                    clearStatistics.Add(statInTime);
                }
            }
            clearStatistics.Reverse();
            return clearStatistics;
        }
    }
}