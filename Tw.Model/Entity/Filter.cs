using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Filter
    {
        string _id;
        string _name = String.Empty;
        string _expression = String.Empty;
        string _exchange = "nyse";
        string _interval = "day";
        int _period = 10;
        string _description = String.Empty;
        int _debug = 0;

        public Filter(){}
        public Filter(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public String Name { get { return _name; } set{_name = value;} }
        [Binding()]
        public String Expression { get { return _expression; } set { _expression = value; } }
        [Binding()]
        public String Exchange { get { return _exchange; } set { _exchange = value; } }
        [Binding()]
        public String Interval { get { return _interval; } set { _interval = value; } }
        [Binding()]
        public int Period { get { return _period; } set { _period = value; } }
        [Binding()]
        public String Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public int Debug { get { return _debug; } set { _debug = value; } }
    }
}
