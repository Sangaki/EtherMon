using System;
using System.Collections.Generic;

namespace EtherMon.Models.Ethermine
{
    public class PoolStatistic
    {
        public PoolStatisticData data { get; set; }
    }
    
    public class PoolStatisticData
    {
        public List<MinedBlock> minedBlocks { get; set; }
        public PoolStats poolStats { get; set; }
        public EthPrice price { get; set; }
    }

    public class MinedBlock
    {
        public double number { get; set; }
        public string miner { get; set; }
        public double time { get; set; }
    }

    public class PoolStats
    {
        private double? hashrateRounded { get; set; }
        public double? hashrate
        {
            get => hashrateRounded;
            set => hashrateRounded = Math.Round(value!.Value / 1000000, 2);
        }
        public int miners { get; set; }
        public int workers { get; set; }
    }

    public class EthPrice
    {
        public double usd { get; set; }
        public double btc { get; set; }
    }
}