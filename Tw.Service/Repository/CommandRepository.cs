using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;
using Utility = Tw.Service.Common.Utility;
using Timer = Tw.Service.Common.Utility.Timing.StopWatch;

namespace Tw.Service.Repository
{
    
    public class CommandRepository : UserRepository
    {
        public List<Entity.Command> Object = new List<Entity.Command>();

        StringBuilder report = new StringBuilder();
        Queue<string[]> que = new Queue<string[]>();

        public void Load()
        {
            Object = base.Query<Entity.Command>();
        }

        public string Report()
        {
            return report.ToString();
        }

        private void QueProcess(string expression)
        {
            que.Clear();
            var prc = System.Text.RegularExpressions.Regex.Split(expression, "->");

            foreach (var p in prc)
            {
                que.Enqueue(p.Split(' '));
            }
        }

        public void Run(Entity.Command cmd,System.IO.TextWriter console)
        {
            report.Clear();

            List<Entity.Company> company = new List<Entity.Company>();

            if (cmd != null)
            {
                QueProcess(cmd.Expression);
                do
                {
                    var p = (string[])que.Dequeue();
                    switch (p[0].Trim().ToLower())
                    {
                        case "filter": company = Filter(p[1], cmd.ExportType, console); break;
                        case "backtest": BacktTest(company, p[1],cmd.ExportType, console); break;
                    }
                    
                } while (que.Count != 0);

                if(report!=null && cmd.ExportType!="")
                    Tw.Service.Common.Utility.File.Export(report.ToString(), cmd.ExportType);
            }
        }

        private List<Entity.Company> Filter(string name, string exporttype, System.IO.TextWriter console)
        {
            console.WriteLine("filtering...");
            var r = new Repository.AnalysisRepository();
            var v = r.Filter(name);
            report.AppendLine(v.Report());
            return v.Result;
        }

        private void BacktTest(List<Entity.Company> company, string name, string exporttype, System.IO.TextWriter console)
        {
            console.WriteLine(String.Format("testing {0} companies...", company.Count));
            var r = new Repository.AnalysisRepository();
            var v = r.BacktTest(company,name);
            report.AppendLine(v.Report());
            if (exporttype == "xls")
                v.Xls();
        }
        
    }
}

