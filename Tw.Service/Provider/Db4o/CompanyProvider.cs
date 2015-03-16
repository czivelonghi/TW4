using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Db4objects.Db4o.Linq;

namespace Tw.Service.Provider.Db4o
{
    class CompanyProvider : BaseProvider
    {
        public CompanyProvider(): base("company")
        {
        }

        public IEnumerable<Entity.Company> Query()
        {
                return (from Entity.Company o
                           in base.Container
                            select o);
        }

        public IEnumerable<Entity.Company> Query(string exchange)
        {
            return (from Entity.Company o
                        in base.Container
                    where (o.Exchange == exchange.ToUpper())
                    select o);
        }

        public IEnumerable<Entity.Company> Query(List<string> company)
        {
            IEnumerable<Entity.Company> l=null;

            foreach (string c in company)
            {
                var cl = (from Entity.Company o
                            in base.Container
                            where (o.Symbol == c.ToUpper())
                            select o);

                l.Concat(cl);

            }

            return l;
        }

    }
}
