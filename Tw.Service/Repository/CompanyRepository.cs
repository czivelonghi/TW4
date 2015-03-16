using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;

namespace Tw.Service.Repository
{
    
    public class CompanyRepository : BaseRepository
    {
        public List<Entity.Company> Company = new List<Entity.Company>();

        public CompanyRepository() : base("company"){}

        public void Load(string exchange = null)
        {
            using (var p = new CompanyProvider())
            {
                if (exchange != null)
                    Company = p.Query(exchange).ToList();
                else
                    Company = p.Query().ToList();
            }
        }

        public void Index(string operation, string column)
        {
            if (operation == "create")
                this.Index<Entity.Company>(column);
            else
                this.Index<Entity.Company>(column,false);
        }
    }
}
