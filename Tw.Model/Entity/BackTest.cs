using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class BackTest
    {
        string _id;
        string _name;
        decimal _insampleperc = 0;//perc allocated to in sample data. two types of study's: in & out of sample. ex: .66: create 2/3s in sample and 1/3 out of sample data period.
        int _insamplealloc = 0;//# of intervals to divide data into.  ex: 3 and 300 day period: 300/3=100 days per in/out-sample test.
        string _description;
        int _periodend=Tw.Model.Comon.Utility.Date.ToInt(DateTime.Today);
        int _periodstart = Tw.Model.Comon.Utility.Date.ToInt(DateTime.Today.AddDays(-356));
        string _periodinterval="day";
        decimal _capital=10000M;
        int _maxposition=1;
        decimal _risk;
        decimal _commission=10.00M;
        string _sampledata;
        int _debug=0;
        List<Entry> _entry;
        string _exporttype;

        public BackTest() {_entry = new List<Entry>(); }
        public BackTest(string name){_id = Guid.NewGuid().ToString().Replace("-","");_name = name;_entry = new List<Entry>();}

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public int InSampleAlloc { get { return _insamplealloc; } set { _insamplealloc = value; } }
        [Binding()]
        public decimal InSamplePerc { get { return _insampleperc; } set { _insampleperc = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public string SampleData { get { return _sampledata; } set { _sampledata = value; } }
        [Binding()]
        public int PeriodEnd { get { return _periodend; } set { _periodend = value; } }//total bars to process
        [Binding()]
        public int PeriodStart { get { return _periodstart; } set { _periodstart = value; } }//start date or dateetime
        [Binding()]
        public string PeriodInterval { get { return _periodinterval; } set { _periodinterval = value; } } //day or minute charts
        [Binding()]
        public decimal Capital { get { return _capital; } set { _capital = value; } }
        [Binding()]
        public int MaxPosition { get { return _maxposition; } set { _maxposition = value; } }//total active positions allowed
        [Binding()]
        public decimal Risk { get { return _risk; } set { _risk = value; } } //risk percent per \
        [Binding()]
        public decimal Commission { get { return _commission; } set { _commission = value; } }
        [Binding()]
        public int Debug { get { return _debug; } set { _debug = value; } }
        [Binding()]
        public string ExportType { get { return _exporttype; } set { _exporttype = value; } }
        public List<Entry> Entry { get { return _entry; } set { _entry = value; } }

    }

}
