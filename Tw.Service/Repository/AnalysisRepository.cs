using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.View;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;
using Timer = Tw.Service.Common.Utility.Timing.StopWatch;
using Util = Tw.Service.Common.Utility;
using System.Text.RegularExpressions;

namespace Tw.Service.Repository
{
    public class AnalysisRepository
    {

        bool Parallel(Helper.ViewHelper h)
        {
            var e = h.Environment("threaded_ops");
            if ((e != null) && (e.Value == "yes"))
                return true;
            else
                return false;

        }

        List<Entity.Price> PriceByExchange(string exchange, int period, List<string> timing, string interval = "day")
        {

            var h = new Helper.ViewHelper();

            Timer.Start();

            int enddate = Util.Date.ToInt(DateTime.Today);
            int startdate = Util.Date.CalcStartDate(period, enddate);

            var p = h.PriceByExchange(exchange, startdate, enddate, interval);

            Timer.Stop();

            timing.Add("price: " + Timer.Results());

            return p;
        }

        public string Test(string expression, int debug=1)
        {   //regex tester: http://www.regexplanet.com/advanced/java/index.html
            var pattern ="[n]+\\d"; //to include additionals chars like s: [n|s]+\d
            var newexp  = expression;
            var match =  Regex.Matches(expression, pattern,RegexOptions.IgnoreCase);

            foreach (Match m in match)
            {
                newexp = Regex.Replace(newexp, m.Value, "1", RegexOptions.IgnoreCase);
            }

            return FSharpAnalysis.Filter.Test(newexp, debug);

        }

        public string Reduce(string expression)
        {
            var r = new ExpressionRepository();
            r.Load();
            IEnumerable<Tw.Model.Entity.Expression> expressions = Common.Utility.Conversion.ToSequence<Tw.Model.Entity.Expression>(r.Expression);
            return Reducer.ReduceFromSeq(expression, expressions);
        }

        public MetricView Metric(string name)
        {
            var r = new Helper.ViewHelper();
            var o = r.Metric(name);
            return Metric(o);
        }

        public MetricView Metric(Entity.Metric o)
        {
            var r = new Helper.ViewHelper();
            var v = new MetricView();

            var periodstart = Common.Utility.Date.DateAdd(o.PeriodStart, -Common.Utility.Helper.LargestMetricParam(o));

            v.Company = r.Company(o.Exchange);
            v.Price = r.PriceByExchange(o.Exchange, periodstart, o.PeriodEnd, o.Interval);
            v.Expression = r.Expression();
            v.Study = o.Study;
            v.Type = o.Type;
            v.GroupBy = o.GroupBy;
            v.PeriodStart = o.PeriodStart;
            v.PeriodEnd = o.PeriodEnd;
            v.ExportType = o.ExportType;

            FSharpAnalysis.Metric.Run(v);
            return v;
        }

        public FilterView Filter(string name)
        {
            var r = new Helper.ViewHelper();
            var o = r.Filter(name);
            return Filter(o);
        }

        public FilterView Filter(Entity.Filter o)
        {
            
            var exchanges = o.Exchange.Split(',');
            var h = new Helper.ViewHelper();
            int buffer = Common.Utility.Helper.LargestParam(o.Expression);

            var v = new FilterView() {Filter = o.Expression, Period =o.Period, Interval = o.Interval, DebugMode = o.Debug };

            v.Expression = h.Expression();
            v.Parallel = Parallel(h);

            foreach(var exch in exchanges)
            {
                var p = PriceByExchange(exch, o.Period + buffer,v.Timing, o.Interval);
                v.Company = h.Company(exch);
                v.Price = p;
                if ((p != null) && (p.Count > 0))
                {
                    Timer.Start();
                    FSharpAnalysis.Filter.Run(v);
                    Timer.Stop();
                    v.Timing.Add("filter: " + Timer.Results());
                }
            }

            return v;
        }

        public BackTestView BacktTest(List<Entity.Company> company, string name)
        {
            var r = new Helper.ViewHelper();
            var o = r.BackTest(name);
            return BacktTest(company, o);
        }

        public BackTestView BacktTest(List<Entity.Company> company,Entity.BackTest backtest)
        {

            var h = new Helper.ViewHelper();
            var periodstart = Common.Utility.Date.DateAdd(backtest.PeriodStart, -Common.Utility.Helper.LargestBacktestParam(backtest));

            BackTestView v = new BackTestView() { BackTest = backtest};
            v.Company=company;
            v.Expression=h.Expression();
            v.Parallel=Parallel(h);
            v.ExportType = backtest.ExportType;

            Timer.Start();
            v.Price = h.PriceByCompanyList(company, periodstart, v.BackTest.PeriodEnd, v.BackTest.PeriodInterval);
            Timer.Stop();
            v.Timing.Add("price: " + Timer.Results());

            Timer.Start();
            FSharpAnalysis.BackTest.Run(v);
            Timer.Stop();
            v.Timing.Add("backtest: " + Timer.Results());

            return v;
        }
    }
}
