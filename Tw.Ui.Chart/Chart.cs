using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tw.Service.Common;
using Tw.Model.View;
using Tw.Model.Entity;
namespace Tw.Ui.Chart
{
    public partial class Chart : Form
    {
        public Chart()
        {
            InitializeComponent();
            var c = new Company() { Exchange = "NASDAQ", Symbol = "WIX" };
            var mmv = new MemoryMapView() { Action = "candlechart", Caller = "filter",Company=c, Interval = "day", StartDate = 20131002, EndDate = 20140105 };
            //List<Trade> t = new List<Trade>();
            //t.Add(new Trade() { Date = 20130204, Description = "oversold", PositionType = "long", TradeType = "buy", Shares = 100, Price = 10.12M });
            //t.Add(new Trade() { Date = 20130207, Description = "3daysup", PositionType = "long", TradeType = "sell", Shares = 100, Price = 12.12M });
            //mmv.Trade = t;
            financialChartType1.SetMemoryMap(mmv);
        }

        public Chart(string mmfile)
        {
            InitializeComponent();
            MemoryMapView mmv = Utility.MemoryMap.ReadObject(mmfile) as MemoryMapView;
            financialChartType1.SetMemoryMap(mmv);
        }
    }
}
