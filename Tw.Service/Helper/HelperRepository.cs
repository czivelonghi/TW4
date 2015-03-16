using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity = Tw.Model.Entity;
using Tw.Model.View;
using Common = Tw.Service.Common;
using Tw.Service.Repository;

namespace Tw.Service.Helper
{
    class RepositoryFactory
    {
        public static BaseRepository Create<T>()
        {
            switch (typeof(T).Name.ToLower())
            {
                case "expression": return new ExpressionRepository();
                case "backtest": return new UserRepository();
                case "filter": return new UserRepository();
                case "environment": return new SystemRepository();
                case "query": return new UserRepository();
                case "command": return new UserRepository();
                case "help": return new SystemRepository();
                case "data": return new UserRepository();
                case "import": return new SystemRepository();
                case "price": return new PriceRepository();
                case "company": return new CompanyRepository();
                case "metric": return new UserRepository();
                default: return null;
            }
        }
    }

    public class PersistanceHelper
    {
        public bool Save<T>(T o)
        {
            var r = RepositoryFactory.Create<T>();
            return r.Save<T>(o);
        }

        public int Insert<T>(List<T> o)
        {
            var r = RepositoryFactory.Create<T>();
            return r.Insert<T>(o);
        }

        public int Delete<T>(T o)
        {
            var r = RepositoryFactory.Create<T>();
            return r.Delete<T>(o);
        }

        public int Delete<T>(string key, string name)
        {
            var r = RepositoryFactory.Create<T>();
            return r.Delete<T>(key,name);
        }

        public bool Exist<T>(string name)
        {
            var r = RepositoryFactory.Create<T>();
            return r.Exist<T>("name", name);
        }

        public bool Truncate<T>()
        {
            var r = RepositoryFactory.Create<T>();
            return r.Truncate<T>();
        }
    }

    public class ViewHelper
    {
        public static int CalcStartDate(int period, int enddate)
        {
            var p = new PriceRepository();
            return p.CalcStartDate(period, enddate);
        }

        private List<T> Import<T>()
        {
            var r = RepositoryFactory.Create<T>();
            return r.Query<T>();
        }

        private List<T> UserQuery<T>()
        {
            var r = RepositoryFactory.Create<T>();
            return r.Query<T>();
        }

        private List<T> SystemQuery<T>()
        {
            var r = RepositoryFactory.Create<T>();
            return r.Query<T>();
        }

        public List<Entity.Price> PriceByCompanyList(List<Entity.Company> company, int startdate, int enddate, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceByCompanyList(company, startdate, enddate, interval);
        }

        public List<Entity.Price> PriceByCompany(Entity.Company company, int startdate, int enddate, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceByCompany(company, startdate, enddate, interval);
        }

        public List<Entity.Price> PriceByExchange(string exchange, int period, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceByExchange(exchange, period, interval);
        }

        public List<Entity.Price> PriceByExchange(string exchange, int startdate, int enddate, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceByExchange(exchange, startdate, enddate, interval);
        }

        public List<Entity.Price> PriceByExchangeList(string[] exchange, int startdate, int enddate, string interval = "day")
        {
            List<Entity.Price> p = new List<Entity.Price>();
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;

            foreach (var exch in exchange)
                p.AddRange(r.PriceByExchange(exch, startdate, enddate, interval));

            return p;
        }

        public List<Entity.Price> PriceBySymbol(string exchange, string symbol, int startdate, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceBySymbol(exchange, symbol, startdate, interval);
        }

        public List<Entity.Price> PriceBySymbol(string exchange, string symbol, int startdate, int endate, string interval = "day")
        {
            var r = RepositoryFactory.Create<Entity.Price>() as PriceRepository;
            return r.PriceBySymbol(exchange, symbol, startdate, endate, interval);
        }

        public List<Entity.Company> Company(string exchange)
        {
            var r = RepositoryFactory.Create<Entity.Company>() as CompanyRepository;
            var query = new QueryView();

            r.Load(exchange);
            return r.Company.ToList();
        }

        public QueryView Company(string[] company)
        {
            var r = RepositoryFactory.Create<Entity.Company>() as CompanyRepository;
            var q = new QueryView();
            r.Load();
           
            foreach (var symbol in company)
            {
                var c = r.Company.FirstOrDefault(x => x.Symbol.ToLower() == symbol);
                if(c!=null) q.Company.Add(c);
            }
            
            return q;
        }

        public List<Entity.Import> Import()
        {
            return SystemQuery<Entity.Import>();
        }

        public Entity.Import Import(string name)
        {
            return Import().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Help> Help()
        {
            return SystemQuery<Entity.Help>();
        }

        public Entity.Help Help(string name)
        {
            return Help().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Environment> Environment()
        {
            return SystemQuery<Entity.Environment>();
        }

        public Entity.Environment Environment(string name)
        {
            return Environment().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Expression> Expression()
        {
            var r = RepositoryFactory.Create<Entity.Expression>() as ExpressionRepository;
            r.Load();
            return r.Expression;
        }

        public Entity.Expression Expression(string name)
        {
            return Expression().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Data> Data()
        {
            return UserQuery<Entity.Data>();
        }

        public Entity.Data Data(string name)
        {
            return Data().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Command> Command()
        {
            return UserQuery<Entity.Command>();
        }

        public Entity.Command Command(string name)
        {
            return Command().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Metric> Metric()
        {
            return UserQuery<Entity.Metric>();
        }

        public Entity.Metric Metric(string name)
        {
            return Metric().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Filter> Filter()
        {
            return UserQuery<Entity.Filter>();
        }

        public Entity.Filter Filter(string name)
        {
            return Filter().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Query> Query()
        {
            return UserQuery<Entity.Query>();
        }

        public Entity.Query Query(string name)
        {
            return Query().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.BackTest> BackTest()
        {
            return UserQuery<Entity.BackTest>();
        }

        public Entity.BackTest BackTest(string name)
        {
            return BackTest().FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Entry> Entry(string backtest)
        {
            var t = BackTest(backtest);
            return t.Entry;
        }

        public Entity.Entry Entry(string backtest, string name)
        {
            return Entry(backtest).FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Study> Study(string metric)
        {
            var t = Metric(metric);
            return t.Study;
        }

        public Entity.Study Study(string metric, string name)
        {
            return Study(metric).FirstOrDefault(x => x.Name == name);
        }

        public List<Entity.Exit> Exit(string backtest,string entry)
        {
            var e = Entry(backtest, entry);
            return e.Exit;
        }

        public Entity.Exit Exit(string backtest, string entry, string name)
        {
            return Exit(backtest,entry).FirstOrDefault(x => x.Name == name);
        }
    }
}
