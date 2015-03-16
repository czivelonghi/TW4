using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Metric
    {
        string _id;
        string _name = String.Empty;
        string _exchange = "nyse";
        string _interval = "day";
        string _type = "count";
        string _groupby = "period";
        int _periodstart = Comon.Utility.Date.ToInt(DateTime.Today.AddDays(-365));
        int _periodend = Comon.Utility.Date.ToInt(DateTime.Today);
        string _description = String.Empty;
        string _exporttype = String.Empty;
        int _debug = 0;
        List<Study> _study;

        public Metric() { _study = new List<Study>(); }
        public Metric(string name){_name = name;_id = Guid.NewGuid().ToString().Replace("-", "");_study = new List<Study>();}

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public String Name { get { return _name; } set{_name = value;} }
        [Binding()]
        public String Exchange { get { return _exchange; } set { _exchange = value; } }
        [Binding()]
        public String Type { get { return _type; } set { _type = value; } }
        [Binding()]
        public String GroupBy { get { return _groupby; } set { _groupby = value; } }
        [Binding()]
        public String Interval { get { return _interval; } set { _interval = value; } }
        [Binding()]
        public int PeriodStart { get { return _periodstart; } set { _periodstart = value; } }
        [Binding()]
        public int PeriodEnd { get { return _periodend; } set { _periodend = value; } }
        [Binding()]
        public String Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public int Debug { get { return _debug; } set { _debug = value; } }
        [Binding()]
        public string ExportType { get { return _exporttype; } set { _exporttype = value; } }
        public List<Study> Study { get { return _study; } set { _study = value; } }
    }
}
