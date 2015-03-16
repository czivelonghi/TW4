using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Environment
    {
        string _id;
        private string _name = string.Empty;
        private string _value = string.Empty;
        private int _active = 1;

        public Environment(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Value { get { return _value; } set { _value = value; } }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
    }
}
