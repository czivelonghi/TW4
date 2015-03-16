using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [PersistanceAttribute(KeyColumn = "id")]
    public class Data
    {
        string _id;
        private string _name = string.Empty;
        private string _operation = "import";
        private string _object = string.Empty;
        private string _exp1 = String.Empty;//exchange<-nyse, interval=day, format=csv,pattern=*.*,excludeheader=1
        private string _exp2 = String.Empty;
        private string _exp3 = String.Empty;
        private string _exp4 = String.Empty;
        private string _exp5 = String.Empty;
        private string _exp6 = String.Empty;
        private string _exp7 = String.Empty;
        private string _exp8 = String.Empty;
        private string _description = String.Empty;

        public Data(string name) { _name = name; _id = Guid.NewGuid().ToString().Replace("-", ""); }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public string Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public string Operation { get { return _operation; } set { _operation = value; } }
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
        [Binding()]
        public string Exp5 { get { return _exp5; } set { _exp5 = value; } }
        [Binding()]
        public string Exp6 { get { return _exp6; } set { _exp6 = value; } }
        [Binding()]
        public string Exp7 { get { return _exp7; } set { _exp7 = value; } }
        [Binding()]
        public string Exp8 { get { return _exp8; } set { _exp8 = value; } }
        [Binding()]
        public String Description { get { return _description; } set { _description = value; } }
    }
}
