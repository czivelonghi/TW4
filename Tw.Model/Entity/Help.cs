using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Help
    {
        string _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _use = string.Empty;
        private string _example = string.Empty;
        private string _category = "context";

        public Help(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Category { get { return _category; } set { _category = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
        [Binding()]
        public string Use { get { return _use; } set { _use = value; } }
        [Binding()]
        public string Example { get { return _example; } set { _example = value; } }
    }
}
