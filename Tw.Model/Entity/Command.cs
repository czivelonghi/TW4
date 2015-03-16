using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Command
    {
        string _id;
        private string _name = string.Empty;
        private string _expression = string.Empty;
        private string _exporttype = "txt";
        private string _description = "txt";

        public Command(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Expression { get { return _expression; } set { _expression = value; } }
        [Binding()]
        public string ExportType { get { return _exporttype; } set { _exporttype = value; } }
        [Binding()]
        public string Description { get { return _description; } set { _description = value; } }
    }
}
