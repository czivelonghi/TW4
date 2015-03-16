using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity = Tw.Model.Entity;
using Db4objects.Db4o.Linq;

namespace Tw.Service.Provider.Db4o
{
    class PriceProvider : BaseProvider
    {

        public PriceProvider(string exchange, string interval = "day"):base(exchange.ToLower() + interval.ToLower())
        {
        }

        public IEnumerable<Entity.Price> Query()
        {
            var results = (from Entity.Price o
                           in base.Container
                           orderby o.Symbol ascending, o.Date descending
                           select o);

            return results;
        }

        public IEnumerable<Entity.Price> Query(int startdate)
        {
            var results = (from Entity.Price o
                           in base.Container
                           where (o.Date >= startdate)
                           orderby o.Symbol ascending,o.Date descending
                           select o);

            return results;
        }

        public IEnumerable<Entity.Price> Query(int startdate, string symbol)
        {
            var results = (from Entity.Price o
                           in base.Container
                           where (o.Symbol == symbol.ToUpper()) && (o.Date >= startdate)
                           orderby o.Date descending
                           select o);

            return results;
        }

        public IEnumerable<Entity.Price> Query(int startdate, int enddate)
        {
            var results = (from Entity.Price o
                           in base.Container
                           where (o.Date >= startdate) && (o.Date <= enddate)
                           orderby o.Date descending
                           select o);

            return results;
        }

        public IEnumerable<Entity.Price> Query(int startdate,int enddate, string symbol)
        {
            var results = (from Entity.Price o
                           in base.Container
                           where (o.Symbol == symbol.ToUpper()) && (o.Date >= startdate) && (o.Date <= enddate)
                           orderby o.Date descending
                           select o);

            return results;
        }

        //http://community.versant.com/documentation/reference/db4o-7.13-flare/net35/Content/implementation_strategies/linq_collection.htm
        public IEnumerable<Entity.Price> Query(int startdate, List<string> symbol)
        {
            //var results = (from Entity.Price o in base.Container
            //               join s in symbol
            //               on o.Symbol equals s
            //               where (o.Date >= startdate) && (o.Volume > 0)
            //               orderby o.Symbol ascending, o.Date descending
            //               select o);
            //
            var x = base.Container.Query();
            var y = x.Constrain(typeof(Entity.Price));
            
            var results = base.Container.Query<Entity.Price>(delegate(Entity.Price p)
            {
                return ((p.Date >= startdate) && (p.Symbol.Equals(symbol)));
            });
            
            return results;
        }

        public void Index(string operation, string column)
        {
            if(operation=="create")
                base.CreateIndex<Entity.Price>(column);
            else
                base.DropIndex<Entity.Price>(column);

        }
        //public int Update(string exchange, string interval = "day")
        //{
        //    int count = 0;
        //    var results = (from Entity.Price o
        //                   in base.Container
        //                   orderby o.Date descending
        //                   select o);

        //    foreach (var r in results)
        //    {
        //        r.Interval = "day";
        //        base.Container.Store(r);
        //    }
            
        //    return count;
        //}

        //public bool Convert()
        //{
        //    bool success = true;
        //    Common.Utility.SQLiteToDbo.Convert();
        //    return success;
        //}
    }
}
