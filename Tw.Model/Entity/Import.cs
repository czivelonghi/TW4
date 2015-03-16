using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Import
    {
        string _id;
        private string _name = string.Empty;
        private string _file = string.Empty;
        private string _status = string.Empty;
        private string _object = string.Empty;
        private string _category = string.Empty;
        private string _description = string.Empty;
        private int _date;
        private int _count;

        public Import(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Status { get { return _status; } set { _status = value; } }
        [Binding()]
        public string Object { get { return _object; } set { _object = value; } }
        [Binding()]
        public string Category { get { return _category; } set { _category = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public string File { get { return _file; } set { _file = value; } }
        [Binding()]
        public int Date { get { return _date; } set { _date = value; } }
        [Binding()]
        public int Count { get { return _count; } set { _count = value; } }
    }
}
