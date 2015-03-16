using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Query
    {
        string _id;
        string _name;
        string _object;
        string _exp1;
        string _exp2;
        string _exp3;
        string _exp4;

        public Query() { }
        public Query(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Object { get { return _object; } set { _object = value; } }
        [Binding()]
        public string Exp1 { get { return _exp1; } set { _exp1 = value; } }
        [Binding()]
        public string Exp2 { get { return _exp2; } set { _exp2 = value; } }
        [Binding()]
        public string Exp3 { get { return _exp3; } set { _exp3 = value; } }
        [Binding()]
        public string Exp4 { get { return _exp4; } set { _exp4 = value; } }
    }
}
