using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using System.Globalization;
using Tw.Service.Provider;
using Common = Tw.Service.Common;
using Entity = Tw.Model.Entity;
using Util = Tw.Service.Common.Utility;

namespace Tw.Service.Repository.UtilityViewRepository
{
    public class Data
    {

        class DataParameter
        {
            public string Header;
            public string Format;
            public string Path;
            public string Pattern;
            public string Exchange;
            public string Interval;
            public string Delimiter;
            public string Operation;
            public string Column;
            public string Delete;
            public string Archive;
            public string Value;

            public DataParameter(Entity.Data obj)
            {
                ExpValue(obj, "format", out Format, "csv");
                Delimiter = (Format == "tab") ? "\t" : ",";
                ExpValue(obj, "path", out Path);
                ExpValue(obj, "header", out Header, "no");
                ExpValue(obj, "exchange", out Exchange);
                ExpValue(obj, "interval", out Interval);
                ExpValue(obj, "pattern", out Pattern);
                ExpValue(obj, "operation", out Operation);
                ExpValue(obj, "column", out Column);
                ExpValue(obj, "delete", out Delete,"no");
                ExpValue(obj, "archive", out Archive);
                ExpValue(obj, "value", out Value);
            }

            private bool ValueExists(object value, string name)
            {
                if((value!=null) && (value.ToString().IndexOf(name) > -1))
                    return true;
                else
                    return false;
            }

            private void ExpValue(Entity.Data obj, string name, out string value, string defValue = "")
            {

                string[] x = { "none", "none" };
                value = String.Empty;

                if (ValueExists(obj.Exp1, name))
                    x = obj.Exp1.Split('=');
                if (ValueExists(obj.Exp2, name))
                    x = obj.Exp2.Split('=');
                if (ValueExists(obj.Exp3, name))
                    x = obj.Exp3.Split('=');
                if (ValueExists(obj.Exp4, name))
                    x = obj.Exp4.Split('=');
                if (ValueExists(obj.Exp5, name))
                    x = obj.Exp5.Split('=');
                if (ValueExists(obj.Exp6, name))
                    x = obj.Exp6.Split('=');
                if (ValueExists(obj.Exp7,name))
                    x = obj.Exp7.Split('=');
                if (ValueExists(obj.Exp8,name))
                    x = obj.Exp8.Split('=');

                if (x[0] != "none")
                    value = x[1];
                else if (defValue != String.Empty)
                    value = defValue;
            }

        }

        public class Export
        {

            private string ExportToString<T>(string includeHeaderLine, List<T> objects)
            {

                StringBuilder sb = new StringBuilder();
                IList<PropertyInfo> propertyInfos = typeof(T).GetProperties();

                if (includeHeaderLine=="yes")
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (Tw.Service.Common.Utility.Attribute.IsBinding(propertyInfo))
                            sb.Append(propertyInfo.Name).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1).AppendLine();
                }

                foreach (T obj in objects)
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (Tw.Service.Common.Utility.Attribute.IsBinding(propertyInfo))
                            sb.Append(CsvFriendly(propertyInfo.GetValue(obj, null))).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1).AppendLine();
                }

                return sb.ToString();
            }

            public void ExportToFile<T>(string path,List<T> objects)
            {
                var sb = new StringBuilder();
                sb.Append(ExportToString("yes", objects));
                File.WriteAllText(path, sb.ToString());
                System.Diagnostics.Process.Start(path);
            }

            public void ExportToFile(Entity.Data obj)
            {
                var parm = new DataParameter(obj);
                var p = new Helper.ViewHelper();

                switch (obj.Object)
                {
                    case "exp": ExportToFile<Entity.Expression>(parm.Path, p.Expression()); break;
                }

            }

            //private byte[] ExportToBytes<T>()
            //{
            //    return Encoding.UTF8.GetBytes(Export());
            //}

            private string CsvFriendly(object value)
            {
                if (value == null) return "";
                if (value is Nullable && ((System.Data.SqlTypes.INullable)value).IsNull) return "";

                if (value is DateTime)
                {
                    if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                        return ((DateTime)value).ToString("yyyy-MM-dd");
                    return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                }
                string output = value.ToString();

                if (output.Contains(",") || output.Contains("\""))
                    output = '"' + output.Replace("\"", "\"\"") + '"';

                return output;

            }
        }

        public class Import
        {
            private bool CreateSystemObject<T>(T obj)
            {
                var p = new Helper.PersistanceHelper();
                return p.Save<T>(obj);
            }

            private decimal TotalRecordPercent(int total, int records)
            {
                return (records != 0) ? Math.Round(((decimal)records / (decimal)total), 4) : 0M;
            }

            private bool IsZeroVolumeDayFile(FileInfo f, string header, string delimiter)
            {

                int recordcount = 0;
                int zerovolcount = 0;
                int totalrecordcount = 0;

                using (var sr = f.OpenText())
                {
                    while (!sr.EndOfStream)
                    {
                        totalrecordcount += 1;
                        var line = sr.ReadLine();
                        var c = System.Text.RegularExpressions.Regex.Split(line, delimiter);

                        if (!(header == "yes" && totalrecordcount == 1))
                        {
                            recordcount += 1;
                            var volume = Int32.Parse(Util.Misc.Clean(c[6]));
                            if (volume == 0)
                                zerovolcount += 1;
                        }
                    }
                }

                return (TotalRecordPercent(recordcount,zerovolcount) > .8M);//80% o 0 vol records, skip

            }

            private Entity.Environment LastSymbolUpdate()
            {
                var p = new Helper.ViewHelper();
                var o = p.Environment("company_update");

                if (o == null)
                {
                    o = new Entity.Environment("company_update");
                    CreateSystemObject<Entity.Environment>(o);
                }

                return o;
            }

            private void LastSymbolUpdate(Entity.Environment o)
            {
                var p = new Helper.PersistanceHelper();
                o.Value = Common.Utility.Date.ToString(DateTime.Today);
                p.Save<Entity.Environment>(o);
            }

            private void Delete(List<string> files, string srcpath)
            {
                foreach (var f in files)
                {
                    Directory.Delete(srcpath + "\\" + f,false);
                    Console.WriteLine(String.Format("{0} deleted...", f));
                }
            }

            private void Archive(List<string> files, string srcpath, string destpath)
            {
                if (!Directory.Exists(destpath))
                {
                    Directory.CreateDirectory(destpath);
                }

                foreach (var f in files)
                {
                    Directory.Move(srcpath + "\\" + f, destpath + "\\" + f);
                    Console.WriteLine(String.Format("{0} archived...", f));
                }
            }

            private void Company(Entity.Data obj)
            {
                var o = LastSymbolUpdate();

                if (Util.Misc.NVL(o.Value,0) < Common.Utility.Date.ToInt(DateTime.Today))
                {
                    Common.Utility.Timing.StopWatch.Start();
                    System.Console.WriteLine("creating symbols...");

                    var r=new CompanyRepository();
                    r.Load();

                    var parm = new DataParameter(obj);

                    DirectoryInfo dir = new DirectoryInfo(parm.Path);
                    var file = dir.GetFiles(parm.Pattern);

                    int count=0;
                    foreach (var f in file)
                    {
                        var sr = f.OpenText();
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var c = System.Text.RegularExpressions.Regex.Split(line, parm.Delimiter);

                            if (!(parm.Header == "yes" && count == 1))
                            {
                                count += 1;

                                var exchange = Util.Misc.Clean(c[0]);
                                var symbol = Util.Misc.Clean(c[1]);
                                var name = Util.Misc.Clean(c[2]);

                                if (!r.Company.Exists(x => x.Symbol == symbol))
                                {
                                    var company = new Entity.Company() { Symbol = symbol, Name = name, Exchange = exchange };
                                    r.Company.Add(company);
                                    Console.WriteLine(line);
                                }
                            }
                        }
                    }

                    if (count > 0) r.Insert<Entity.Company>(r.Company);

                    LastSymbolUpdate(o);
                    Common.Utility.Timing.StopWatch.Stop();

                    System.Console.WriteLine(r.Company.Count + " record(s) imported...");
                    System.Console.WriteLine("done..." + Common.Utility.Timing.StopWatch.Results());
                        
                }
                else
                    System.Console.WriteLine("symbols already imported today...");

            }

            private string GetExtension(string file)
            {
                string ext = string.Empty;
                int fileExtPos = file.LastIndexOf(".", StringComparison.Ordinal);
                if (fileExtPos >= 0)
                    ext = file.Substring(fileExtPos, file.Length - fileExtPos);

                return ext;
            }

            private bool Imported(string file)
            {
                var r = new Helper.ViewHelper();
                var l = r.Import();
                var o = l.FirstOrDefault(x => x.File == file.ToLower() && (x.Status == "success" || x.Status == "skip"));
                return (o != null) ? true : false;
            }

            private Entity.Import CreateImport(string exchange,string interval,string file,int count,string status="success",string description="")
            {
                var f = file.Replace(GetExtension(file), "");
                var import =  new Entity.Import(f.ToLower())
                {
                    Category=(exchange + interval).ToLower(),
                    Object="price",
                    Date = Int32.Parse(DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString("00") + DateTime.Today.Day.ToString("00")),
                    File = file.ToLower(),
                    Count = count,
                    Status =status,
                    Description=description
                };

                return import;

            }

            private void Price(Entity.Data obj)
            {
                int recordcount = 0;
                int committedrecordcount = 0;
                int totalrecordcount = 0;
                int filecount = 0;
                var parm = new DataParameter(obj);
                List<string> processed = new List<string>();

                DirectoryInfo dir = new DirectoryInfo(parm.Path);
                var file = dir.GetFiles(parm.Pattern);

                List<Entity.Price> list = new List<Entity.Price>();
                List<Entity.Import> importlist = new List<Entity.Import>();
                
                foreach (var f in file)
                {
                    if (Imported(f.Name))
                    {
                        System.Console.WriteLine(String.Format("...skipping {0} (file already imported) !!!", f.Name));
                        processed.Add(f.Name);
                    }
                    else if (IsZeroVolumeDayFile(f, parm.Header, parm.Delimiter))
                    {
                        System.Console.WriteLine(String.Format("...skipping {0} (zero volume day file) !!!", f.Name));
                        importlist.Add(CreateImport(parm.Exchange, parm.Interval, f.Name, 0, "skip", "zero volume day file"));
                        processed.Add(f.Name);
                    }
                    else
                    {
                        System.Console.WriteLine(String.Format("...processing {0} ...", f.Name));

                        filecount += 1;
                        recordcount = 0;

                        using (var sr = f.OpenText())
                        {
                            while (!sr.EndOfStream)
                            {

                                recordcount += 1;

                                var line = sr.ReadLine();
                                var c = System.Text.RegularExpressions.Regex.Split(line,parm.Delimiter);

                                if (!(parm.Header == "yes" && recordcount == 1))
                                {

                                    var tmpdate = Convert.ToDateTime(Util.Misc.Clean(c[1]));

                                    var p = new Entity.Price()
                                    {
                                        Interval = parm.Interval,
                                        Symbol = Util.Misc.Clean(c[0]),
                                        Date = Int32.Parse(tmpdate.Year.ToString() + tmpdate.Month.ToString("00") + tmpdate.Day.ToString("00")),
                                        Open = Decimal.Parse(c[2]),
                                        High = Decimal.Parse(c[3]),
                                        Low = Decimal.Parse(c[4]),
                                        Close = Decimal.Parse(c[5]),
                                        Volume = Int32.Parse(Util.Misc.Clean(c[6]))
                                    };

                                    list.Add(p);

                                    totalrecordcount += 1;
                                }
                            }

                            if (list.Count > 0)
                            {
                                System.Console.WriteLine(String.Format("...importing {0} record(s)", list.Count));
                                var r = new PriceRepository();
                                var count = r.Import(list, parm.Exchange, parm.Interval);

                                if (count != list.Count)
                                {
                                    System.Console.WriteLine(String.Format("{0} record(s) failed to import!!!", (list.Count - count)));
                                    importlist.Add(CreateImport(parm.Exchange, parm.Interval, f.Name, list.Count - count, "error", "import failure"));
                                }
                                else
                                {
                                    committedrecordcount += list.Count;
                                    importlist.Add(CreateImport(parm.Exchange, parm.Interval, f.Name, list.Count));
                                    processed.Add(f.Name);
                                    list.Clear();
                                }
                            }
                        }
                    }
                }

                if (importlist.Count > 0)
                {
                    var h = new Helper.PersistanceHelper();
                    h.Insert<Entity.Import>(importlist);
                }

                if (parm.Delete == "yes") Delete(processed,parm.Path + "\\" + parm.Pattern);
                if (parm.Archive != string.Empty) Archive(processed, parm.Path, parm.Archive);

                var results = String.Format("{0} file(s) processed...", filecount) + System.Environment.NewLine +
                              String.Format("{0} ouf of {1} records committed...", committedrecordcount, totalrecordcount);

                System.Console.WriteLine(results);
            }

            private string UCase1stLetter(string input)
            {
                var arr = input.ToCharArray();
                arr[0] = Char.ToUpperInvariant(arr[0]);
                return new String(arr);
            }

            private void UserObject<T>(Entity.Data obj)
            {
                int count = 0;

                var parm = new DataParameter(obj);
                var sr = File.OpenText(parm.Path);
                var p = new Helper.PersistanceHelper();
                string[] header;

                while (!sr.EndOfStream)
                {
                    count += 1;
                    var text = sr.ReadLine();
                    if(count==1)
                    {
                        header = text.Split(',');
                    }
                    else
                    {
                        var line = text.Split(',');
                    }
                }

            }

            public void ImportToDB(Entity.Data obj)
            {
                switch(obj.Object)
                {
                    case "price": Price(obj);break;
                    case "company": Company(obj); break;
                    case "exp": UserObject<Entity.Expression>(obj); break;
                }

            }
        }

        public class Index
        {

            public void AlterIndex(Entity.Data obj)
            {
                switch (obj.Object)
                {
                    case "price": Price(obj); break;
                    case "company": Company(obj); break;
                }
            }

            private void Price(Entity.Data obj)
            {
                var parm = new DataParameter(obj);
                var col = parm.Column.Split(',');

                var r = new PriceRepository();
                foreach (var c in col)
                {
                    r.Index(parm.Operation, c, parm.Exchange, parm.Interval);
                }

            }

            private void Company(Entity.Data obj)
            {

                var parm = new DataParameter(obj);
                var col = parm.Column.Split(',');

                var r = new CompanyRepository();
                foreach (var c in col)
                {
                    r.Index(parm.Operation, c);
                }
            }
        }

        public class Ops
        {
            public int Delete(Entity.Data obj)
            {
                int count = 0;
                switch (obj.Object)
                {
                    case "import": count = DeleteImport(obj); break;
                }
                return count;
            }

            private int DeleteImport(Entity.Data obj)
            {
                var parm = new DataParameter(obj);
                var r = new SystemRepository();
                return r.Delete<Entity.Import>(parm.Column, parm.Value);
            }
        }
    }
 
    public class Web
        {
            public void Summary(string symbol)
            {
                var url = "http://finance.yahoo.com/q?s=" + symbol + "&ql=1";
                Common.Utility.HTTP.OpenBrowser(url);
            }

            public void Chart(string symbol)
            {
                var url = "http://finance.yahoo.com/q/ta?s=" + symbol + "&t=1m&l=on&z=l&q=c&p=&a=&c=";
                //var url = "http://finance.yahoo.com/q/bc?s=" + symbol + "+Basic+Chart";
                Common.Utility.HTTP.OpenBrowser(url);
            }

            public void History(string symbol, DateTime startdate, DateTime enddate)
            {
                var url = "http://finance.yahoo.com/q/hp?s=" + symbol +
                            "&a=" + (startdate.Month - 1) +
                            "&b=" + startdate.Day +
                            "&c=" + startdate.Year +
                            "&d=" + (enddate.Month - 1) +
                            "&e=" + enddate.Day +
                            "&f=" + enddate.Year +
                            "&g=d";
                Common.Utility.HTTP.OpenBrowser(url);
            }
        }
}
