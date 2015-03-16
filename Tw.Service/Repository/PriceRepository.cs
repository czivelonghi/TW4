using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;
using Util = Tw.Service.Common.Utility;

namespace Tw.Service.Repository
{
    public class PriceRepository:BaseRepository
    {
        public List<Entity.Environment> Environment = new List<Entity.Environment>();

        public PriceRepository()
        {
            var r = new SystemRepository();
            Environment = r.Query<Entity.Environment>();
        }

        private int CalcWeekDays(int period)
        {
            decimal weeks = 1;
            int totalperiods = period;

            if (period > 5)
            {
                weeks = period / 5;
                totalperiods = period + ((int)Math.Ceiling(weeks) * 2);
            }

            return totalperiods;
        }

        private int CalcStartDate(int period)
        {
            DateTime date = DateTime.Today;
            
            var env = Environment.FirstOrDefault(x => x.Active == 1 && x.Name == "last_price_date");

            if (env != null)
                date = Util.Date.ToDate(env.Value);

            var startdate = date.AddDays(-CalcWeekDays(period));

            return int.Parse(startdate.ToString("yyyyMMdd"));
        }

        public int CalcStartDate(int period, int enddate)
        {

            DateTime date = Util.Date.ToDate(enddate.ToString());
            var newdate = date.AddDays(-CalcWeekDays(period));
            return int.Parse(newdate.ToString("yyyyMMdd"));
        }

        public List<Entity.Price> PriceByExchange(string exchange, int period, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            using (var p = new PriceProvider(exchange, interval))
            {
                Price = p.Query(CalcStartDate(period)).ToList();
            }

            return Price;
        }

        public List<Entity.Price> PriceByExchange(string exchange, int startdate, int enddate, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            using (var p = new PriceProvider(exchange, interval))
            {
                try
                {
                    var qry = p.Query(startdate, enddate);
                    Price = qry.ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return Price;
        }

        public List<Entity.Price> PriceBySymbol(string exchange, string symbol, int startdate, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            using (var p = new PriceProvider(exchange, interval))
            {
                try
                {
                    var qry = p.Query(startdate, symbol);
                    Price = qry.ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return Price;
        }

        public List<Entity.Price> PriceBySymbol(string exchange, string symbol, int startdate, int enddate, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            using (var p = new PriceProvider(exchange, interval))
            {
                try
                {
                    var qry = p.Query(startdate, enddate, symbol);
                    Price = qry.ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return Price;
        }

        public List<Entity.Price> PriceByCompanyList(List<Entity.Company> company, int startdate, int enddate, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            company.OrderBy(x => x.Exchange);

            foreach (var c in company)
            {
                using (var p = new PriceProvider(c.Exchange, interval))
                {
                    Price.AddRange(p.Query(startdate,enddate, c.Symbol).ToList());
                }
            }

            return Price;
        }

        public List<Entity.Price> PriceByCompany(Entity.Company company, int startdate, int enddate, string interval = "day")
        {
            List<Entity.Price> Price = new List<Entity.Price>();

            using (var p = new PriceProvider(company.Exchange, interval))
            {
                try
                {
                    var qry = p.Query(startdate, enddate, company.Symbol);
                    Price = qry.ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return Price;
        }


        public int Import(List<Entity.Price> l, string exchange, string interval = "day")
        {
            using (BaseProvider r = CreateProvider(exchange, interval))
            {
                return r.Insert<Entity.Price>(l);
            }
        }

        public void Index(string operation, string column, string exchange, string interval = "day")
        {
            using (BaseProvider r = CreateProvider(exchange, interval))
            {
                if (operation == "create")
                    r.CreateIndex<Entity.Price>(column);
                else
                    r.DropIndex<Entity.Price>(column);
            }
        }

        private BaseProvider CreateProvider(string exchange, string interval = "day")
        {
            BaseProvider p = null;
            if (interval == "day")
            {
                switch (exchange)
                {
                    case "amex": p = new AmexDayProvider() ; break;
                    case "nyse": p = new NyseDayProvider(); break;
                    case "nasdaq": p = new NasdaqDayProvider(); break;
                }
            }
            return p;
        }

    }
}
