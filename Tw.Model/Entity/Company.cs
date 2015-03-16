using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [Serializable()]
    [PersistanceAttribute(KeyColumn = "symbol")]
    public class Company
    {
        private string _name=string.Empty;
        private string _symbol=string.Empty;
        private string _exchange = string.Empty;
        private string _industry = string.Empty;
        private string _sector = string.Empty;
        
        public string Name { get { return _name; } set { _name = value; } }
        public string Symbol { get { return _symbol; } set { _symbol = value; } }
        public string Exchange { get { return _exchange; } set { _exchange = value; } }
        public string Industry { get { return _industry; } set { _industry = value; } }
        public string Sector { get { return _sector; } set { _sector = value; } }

        public Company() { }
    }
}
