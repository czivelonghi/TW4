using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.View
{
    [Serializable()]
    public class MemoryMapView
    {
        public MemoryMapView()
        {
            Trade=new List<Entity.Trade>();
        }

        public string Action = "chart";
        public string Caller = "backtest";
        public Entity.Company Company;
        public string Interval;
        public int StartDate;
        public int EndDate;
        public List<Entity.Trade> Trade;
    }
}
