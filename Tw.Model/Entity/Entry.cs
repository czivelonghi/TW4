using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Entry
    {
        string _id;
        private int _active=1;
        private string _name;
        private string _entrytype="long"; //sell short/ buy long
        private decimal _risk=.05M; //% or amount
        private string _risktype="p"; //p = % or f = fixed amount
        private string _expression;
        private string _description;
        private int _timelimit=0;
        private decimal _stoploss=.1M; //percent of position before seeling all positions
        private List<Exit> _exit; //exit criteria

        public Entry() { _exit = new List<Exit>(); }
        public Entry(string name)
        {
            _id = Guid.NewGuid().ToString().Replace("-", "");
            _name = name;
            _exit = new List<Exit>();
        }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string EntryType { get { return _entrytype; } set { _entrytype = value; } }
        [Binding()]
        public decimal Risk { get { return _risk; } set { _risk = value; } }
        [Binding()]
        public string RiskType { get { return _risktype; } set { _risktype = value; } }
        [Binding()]
        public string Expression { get { return _expression; } set { _expression = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public decimal StopLoss { get { return _stoploss; } set { _stoploss = value; } }
        [Binding()]
        public int TimeLimit { get { return _timelimit; } set { _timelimit = value; } }
        public List<Exit> Exit { get { return _exit; } set { _exit = value; } }
    }
}
