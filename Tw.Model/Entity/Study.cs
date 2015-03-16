using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Study
    {
        string _id;
        int _active = 1;
        string _name = String.Empty;
        string _expression = String.Empty;
        string _description = String.Empty;

        public Study() { }
        public Study(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public String Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public String Expression { get { return _expression; } set { _expression = value; } }
        [Binding()]
        public String Description { get { return _description; } set { _description = value; } }
    }
}
