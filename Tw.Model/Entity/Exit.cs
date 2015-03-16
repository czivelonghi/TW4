using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Exit
    {
        private int _active=1;
        private string _id;
        private string _name;
        private decimal _risk=0.05M; //% or amount
        private string _risktype="p"; //% or fixed amount
        private string _expression;
        private string _description;

        public Exit() { }
        public Exit(string name) { _id = Guid.NewGuid().ToString().Replace("-", ""); _name = name; }
        public string Id { get { return _id; } set { _id = value; } }

        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public decimal Risk { get { return _risk; } set { _risk = value; } }
        [Binding()]
        public string RiskType { get { return _risktype; } set { _risktype = value; } }
        [Binding()]
        public string Expression { get { return _expression; } set { _expression = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
    }
}
