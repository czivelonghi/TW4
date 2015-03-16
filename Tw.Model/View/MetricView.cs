using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.Entity;

namespace Tw.Model.View
{
    public class MetricView : IView
    {
        public List<Entity.Company> Company { get; set; }
        public List<Entity.Price> Price { get; set; }
        public List<Entity.Expression> Expression { get; set; }
        public List<Entity.Error> Error { get; set; }
        public List<string> Debug { get; set; }
        public List<string> Timing { get; set; }
        public List<Entity.Study> Study { get; set; }
        public int DebugMode { get; set; }
        public bool Parallel  { get; set; }
        //class specific
        public string Type;//count/
        public string GroupBy;//period/symbol
        public int PeriodStart { get; set; }
        public int PeriodEnd { get; set; }
        public string ExportType { get; set; }
        //report collections
        List<Count> CountResult { get; set; }
        
        public int RowCount()
        {
            int count=0;
            if (Type=="count")
            {
                if(GroupBy=="")
                    count = Comon.Utility.Date.PeriodsBetween(PeriodStart, PeriodEnd);
                else
                    count = Company.Count;
            }
            return count;
        }

        class Count
        {
            public string Study { get; set; }
            public string Symbol { get; set; }
            public int Period { get; set; }
        }

        public void AddCount(string study, string symbol, int period)
        {
            //dynamic o = new System.Dynamic.ExpandoObject();
            //o.test = 1;
            CountResult.Add(new Count() { Symbol = symbol, Period = period, Study = study });
        }

        public MetricView()
        {
            Company = new List<Company>();
            Expression = new List<Expression>();
            Price = new List<Price>();
            Error = new List<Error>();
            Debug = new List<string>();
            Timing = new List<string>();
            CountResult = new List<Count>();
        }

        public string Report()
        {
            string output = "";

            switch (Type)
            {
                case "count": output = ReportCount(); break;
            }

            return output;
        }

        public string Export()
        {
            string output = "";

            switch (Type)
            {
                case "count": output = ExportCount(); break;
            }

            return output;
        }

        #region "export"

        #region "count"

        string ExportCount()
        {
            var sb = new StringBuilder();

            if (CountResult.Count() > 0)
            {
                ExportCountHeader(sb);

                if (GroupBy == "period")
                    ExportCountGroupByPeriod(sb);
                else
                    ExportCountGroupBySymbol(sb);
            }
            else
            {
                sb.AppendLine("no company(s) found");
            }

            return sb.ToString();

        }

        string delimit(object value)
        {
            if (ExportType == "csv")
                return value.ToString() + ",";
            else
                return value.ToString() + "";//need tab char
        }

        void ExportCountHeader(StringBuilder sb)
        {
            string studyname = "";

            foreach (var s in Study)
                studyname += delimit(s.Name);

            sb.AppendLine(delimit(GroupBy) + studyname);
        }

        void ExportCountGroupByPeriod(StringBuilder sb)
        {
            int total = Comon.Utility.Date.PeriodsBetween(PeriodStart, PeriodEnd);

            for (int i = 0; i < total; i++)
            {
                //date          exp 1       exp 2
                //20110101      0           1
                //20110102      0           0
                int period = Comon.Utility.Date.DateAdd(PeriodStart, i);
                string line = "";

                foreach (var s in Study)
                {
                    line += delimit(LineCountByPeriod(period, s.Name));
                }

                sb.AppendLine(delimit(period.ToString()) + line);
            }
        }

        void ExportCountGroupBySymbol(StringBuilder sb)
        {
            foreach (var c in Company)
            {
                string line = "";
                foreach (var s in Study)
                {
                    line += delimit(LineCountBySymbol(c.Symbol, s.Name));
                }
                sb.AppendLine(delimit(c.Symbol) + line);
            }
        }

        #endregion

        #endregion

        #region "report"

        #region "count"

        string ReportCount()
        {
            var sb = new StringBuilder();

            if (Error.Count > 0)
            {
                ReportError(sb);
            }

            if (CountResult.Count() > 0)
            {
                ReportCountHeader(sb);

                if (GroupBy == "period")
                    ReportCountGroupByPeriod(sb);
                else
                    ReportCountGroupBySymbol(sb);

                ReportCountTotal(sb);
            }
            else
            {
                sb.AppendLine("no company(s) found");
            }

            return sb.ToString();

        }

        void ReportError(StringBuilder sb)
        {
            sb.AppendLine("error(s):");
            foreach (var e in Error)
            {
                sb.AppendLine(e.Description);
            }
        }

        void ReportCountHeader(StringBuilder sb)
        {
            string studyname = "";

            foreach (var s in Study)
                studyname += s.Name.PadRight(15);

            sb.AppendLine(GroupBy.PadRight(15) + studyname);
        }

        int LineCountByPeriod(int period,string study)
        {
            return CountResult.Where(x => (x.Period == period) && (x.Study == study)).Count();
        }

        int LineCountBySymbol(string symbol, string study)
        {
            return CountResult.Where(x => (x.Symbol == symbol) && (x.Study == study)).Count();
        }

        void ReportCountGroupBySymbol(StringBuilder sb)
        {

            foreach (var c in Company)
            {
                string line = "";

                foreach (var s in Study)
                {
                    line += LineCountBySymbol(c.Symbol, s.Name).ToString().PadRight(15);
                }

                sb.AppendLine(c.Symbol.PadRight(15) + line);
            }
            
        }

        void ReportCountGroupByPeriod(StringBuilder sb)
        {
            int total = Comon.Utility.Date.PeriodsBetween(PeriodStart, PeriodEnd);

            for (int i = 0; i < total; i++)
            {
                int period = Comon.Utility.Date.DateAdd(PeriodStart, i);
                string line = Pad(period,15);

                Study.ForEach(x => line += Pad(LineCountByPeriod(period, x.Name),15));
                sb.AppendLine(line);
                //foreach (var s in Study)
                //{
                //    line += LineCountByPeriod(period, s.Name).ToString().PadRight(15);
                //}

                //sb.AppendLine(period.ToString().PadRight(15) + line);
            }
        }

        void ReportCountTotal(StringBuilder sb)
        {
            //if (GroupBy == "period")
            //{
            //    var c = from r in CountResult
            //            group r by new { r.Period, r.Study} into newGroup
            //            orderby newGroup.Key
            //            select newGroup;

            //    sb.AppendLine("max: " + c.Max());
            //}
            //else
            //{
            //    var c = from r in CountResult
            //            group r by r.Symbol into newGroup
            //            orderby newGroup.Key
            //            select newGroup;
            //    sb.AppendLine("max: " + c.Max());
            //}
            
            foreach (var s in Study)
            {
                sb.AppendLine(s.Name + ": " + CountResult.Where(x => (x.Study == s.Name)).Count());
            }
            
            sb.AppendLine("total: " + CountResult.Count);
        }

        string Pad(object value, int count)
        {
            return value.ToString().PadRight(count);
        }

        #endregion

        #endregion
    }
        
}
