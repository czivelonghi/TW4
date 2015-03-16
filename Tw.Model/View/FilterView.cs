using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Tw.Model.Entity;

namespace Tw.Model.View
{
    public class FilterView : IView
    {
        public List<Entity.Company> Company { get; set; }
        public List<Entity.Expression> Expression { get; set; }
        public List<Entity.Price> Price { get; set; }
        public List<Entity.Error> Error { get; set; }
        public List<string> Debug { get; set; }
        public List<string> Timing { get; set; }
        public int DebugMode { get; set; }
        public bool Parallel { get; set; }
        public string ExportType { get; set; }
        public string Export() { return ""; }

        public List<Entity.Company> Result { get; set; }
        public string Filter;
        public string LastUpdate;
        public int Period { get; set; }
        public string Interval { get; set; }
        public List<string> Exchanges = new List<string>() { "AMEX", "NYSE", "NASDAQ" };

        public FilterView()
        {
            Company = new List<Company>();
            Expression = new List<Expression>();
            Price = new List<Price>();
            Result = new List<Entity.Company>();
            Error = new List<Error>();
            Debug = new List<string>();
            Timing = new List<string>();
        }

        public string Export(string format) { return ""; }

        public string Report()
        {
            var sb = new StringBuilder();

            if (Error.Count > 0)
            {
                sb.AppendLine("error(s):");
                foreach (var e in Error)
                {
                    sb.AppendLine(e.Description);
                }
            }

            if (Result.Count() > 0)
            {
                sb.AppendLine("exchange".PadRight(10) +
                                "symbol".PadRight(10) +
                                "company".PadRight(50) +
                                (DebugMode == 1 ? "debug" : ""));

                for (int i = 0; i < Result.Count(); i++)
                {
                    var company = Result[i];
                    if (company != null)
                    {

                        string x = company.Exchange.PadRight(10) +
                                    company.Symbol.PadRight(10) +
                                    company.Name.PadRight(50) +
                                    ((DebugMode == 1) ? "(" + Debug[i] + ")" : "");

                        sb.AppendLine(x);
                    }
                }
                sb.AppendLine(System.Environment.NewLine);
                sb.AppendLine("amex: ".PadRight(10) + ExchangeCount("amex", Company));
                sb.AppendLine("nasdaq: ".PadRight(10) + ExchangeCount("nasdaq", Company));
                sb.AppendLine("nyse: ".PadRight(10) + ExchangeCount("nyse", Company));
                sb.AppendLine("total: ".PadRight(10) + Result.Count());
                if (DebugMode == 1) sb.AppendLine(Timing[0]);
            }
            else
            {
                sb.AppendLine("no company(s) found");
            }

            return sb.ToString();
        }

        int ExchangeCount(string exchange, List<Company> company)
        {
            return Result.Where(x => x.Exchange.ToLower() == exchange).Count();
        }
    }
}
