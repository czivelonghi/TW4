using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Expression
    {
        string _id;
        int _active=1;
        string _type = "user_defined";
        string _category = "all";
        string _name = String.Empty;
        string _value = String.Empty;
        string _description = String.Empty;

        public Expression() { }
        public Expression(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public String Type { get { return _type; } set { _type = value; } }
        [Binding()]
        public String Category { get { return _category; } set { _category = value; } }
        [Binding()]
        public String Name { get { return _name; } set{_name = value;} }
        [Binding()]
        public String Value { get { return _value; } set { _value = value; } }
        [Binding()]
        public String Description { get { return _description; } set { _description = value; } }
    }
}
