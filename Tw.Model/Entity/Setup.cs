using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Setup
    {
        string _id;
        int _active=1;
        string _name;
        string _type;//live or paper
        string _description;
        decimal _capital;
        int _maxposition;
        decimal _risk;
        decimal _commission;
        string _sampledata;
        int _debug = 0;

        List<Entry> _entry;
        List<Company> _company;
        List<Trade> _trade;

        public Setup()
        {
            _entry = new List<Entry>();
            _company = new List<Company>();
            _trade = new List<Trade>();
        }

        public Setup(string name)
        {
            _entry = new List<Entry>();
            _company = new List<Company>();
            _trade = new List<Trade>();
            _name = name;
            _id = Guid.NewGuid().ToString().Replace("-", "");
        }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set{_name=value;} }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public string SampleData { get { return _sampledata; } set { _sampledata = value; } }
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
        public List<Entry> Entry { get { return _entry; } set { _entry = value; } }
        public List<Company> Company { get { return _company; } set { _company = value; } }
        public List<Trade> Trade { get { return _trade; } set { _trade = value; } }
    }
}
