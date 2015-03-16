using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.Entity;

namespace Tw.Model.Result
{
    public class BackTestResult
    {
        public Entity.Company Company { get; set; }
        public List<Entity.Trade> Trade = new List<Entity.Trade>();
        public decimal Capital = 0.0M;
        public decimal Balance = 0.0M;
        public int LongWins { get {return Wins("long");} }
        public int ShortWins { get{ return Wins("short"); }}
        public int LongLosses { get{return Losses("long"); }}
        public int ShortLosses { get{return Losses("short"); }}
        public int TotalWins { get{return LongWins + ShortWins; }}
        public int TotalLosses { get { return LongLosses + ShortLosses; } }
        public decimal LongExpectancy { get { return Expectancy("long"); } }
        public decimal ShortExpectancy { get { return Expectancy("short"); } }
        public decimal LongWinAverage { get { return AverageWin("long"); } }
        public decimal LongLossAverage { get { return AverageLoss("long"); } }
        public decimal ShortWinAverage { get { return AverageWin("short"); } }
        public decimal ShortLossAverage { get { return AverageLoss("short"); } }
        public decimal LongCommission { get { return Commissions("long"); } }
        public decimal ShortCommission { get {return Commissions("short"); }}
        public decimal TotalCommission { get {return Commissions("long") + Commissions("short"); }}

        ////Expectancy = (Probability of Win * Average Win) – (Probability of Loss * Average Loss) 
        ////(wins/total trades) * avg win per trade - (lossess/total trades) * avg loss per trade
        private decimal Expectancy(string positiontype)
        {
            //total trades
            int total = Trade.Where(x => x.PositionType == positiontype && x.ParentID == string.Empty).Count();
            //wins
            var wins = Wins(positiontype);
            //losses
            var losses = Losses(positiontype);

            var avgwinexp=0.0M;
            var avglossexp=0.0M;

            if (wins>0)
                avgwinexp = (wins / total) - (AverageWin(positiontype)/ total);

            if (losses>0)
                avglossexp = (losses / total) - (AverageLoss(positiontype) / total);

            return Math.Round(Math.Abs(avgwinexp) - Math.Abs(avglossexp), 2);
        }

        decimal AverageWin(string positiontype)
        {
            var avgwin = 0.0M;
            var avgloss = 0.0M;
            WinLossSum(positiontype, out avgwin, out avgloss);
            return avgwin;
        }

        decimal AverageLoss(string positiontype)
        {
            var avgwin = 0.0M;
            var avgloss = 0.0M;
            WinLossSum(positiontype, out avgwin, out avgloss);
            return avgloss;
        }
        ////long/short rate of return
        public decimal ROI
        {
            get { var total = (Balance - Capital);
                  if (total!=0)total = total / Capital;
                  return Math.Round(total,2);
                }
        }

        public decimal LongWinPercent
        {
            get
            {
                var perc = 0.0M;
                var total = (LongWins + LongLosses);
                if (total != 0)
                    perc = Math.Round(1 - ((decimal)(total - LongWins) / total), 2);
                return perc;
            }
        }

        public decimal LongLossPercent
        {
            get
            {
                var perc = 0.0M;
                var total = (LongWins + LongLosses);
                if (total != 0)
                    perc = Math.Round(1 - ((decimal)(total - LongLosses) / total), 2);
                return perc;
            }
        }

        public decimal ShortWinPercent
        {
            get
            {
                var perc = 0.0M;
                var total = (ShortWins + ShortLosses);
                if (total != 0)
                    perc = Math.Round(1 - ((decimal)(total - ShortWins) / total), 2);
                return perc;
            }
        }

        public decimal ShortLossPercent
        {
            get
            {
                var perc = 0.0M;
                var total = (ShortWins + ShortLosses);
                if (total != 0)
                    perc = Math.Round(1 - ((decimal)(total - ShortLosses) / total), 2);
                return perc;
            }
        }

        private int Wins(string positiontype)
        {
            int count=0;
            int count2=0;
            WinLossCount(positiontype, ref count, ref count2);
            return count;
        }

        private int Losses(string positiontype)
        {
            int count=0;
            int count2=0;
            WinLossCount(positiontype, ref count, ref count2);
            return count2;
        }

        private void WinLossCount(string positiontype,ref int wins,ref int losses)
        {
            //sum long/short
            var entry = Trade.Where(x => x.PositionType == positiontype && x.ParentID == string.Empty);
            foreach (var e in entry)
            {
                var amt = Trade.Where(x => x.ParentID == e.ID).Sum(x => x.Total) - e.Total;
                if (amt > 0)
                    wins += 1;
                else if (amt < 0)
                    losses += 1;
            }
        }

        private decimal WinLoss(Trade e)
        {
            return Trade.Where(x => x.ParentID == e.ID).Sum(x => x.Total) - e.Total;
        }

        private decimal TradeSum(string positiontype)                                                                                                 
        {
            var sum = Trade.Where(x => x.PositionType == positiontype && x.ParentID == string.Empty).Sum(delegate(Trade e){return WinLoss(e);});
            return sum;
        }

        private void WinLossSum(string positiontype,out decimal winSum,out decimal lossSum)
        {
            winSum = 0.0M;
            lossSum = 0.0M;
            var trades = Trade.Where(x => x.PositionType == positiontype && x.ParentID == string.Empty);
            foreach (var t in trades)
            {
                var wl = WinLoss(t);
                if (wl > 0)
                    winSum += wl;
                else
                    lossSum += wl;
            }
        }

        private decimal Commissions(string positiontype)
        {
            return Trade.Where(x => x.PositionType == positiontype).Sum(x=> x.Commission);
        }

    }
}
