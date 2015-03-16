using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;
using Tw.Model.Entity;
using Tw.Model.View;
using Tw.Service.Common;
using Tw.Service.Repository;
using Tw.Service.Helper;
using Util = Tw.Service.Repository.UtilityViewRepository;

namespace Tw.Ui.Console
{
    class Program
    {
        #region variables
        static string prompt = "filter> ";
        static List<string> keywords = new List<string>() {"l", "r", "s", "d","c","v","m","cy","export","ls","test" };
        static Context context = new Context();
        static bool keywordcheck = true;//true does keyword check
        static Dictionary<string, IView> reportcache = new Dictionary<string, IView>();

        #endregion

        #region console

        static void Main(string[] args)
        {
            string line;

            Clear();
            Initialize();

            do
            {
                WritePrompt();
                line = System.Console.ReadLine().ToLower();

                if (!Exit(line))
                    RunComand(line);

            } while (!Exit(line));
        }

        private static void Initialize()
        {
            InitTreeNav();
            Resize();
        }

        static void InitTreeNav()
        {
            context.Clear();
            context.AddNode("filter", "root", true);
            context.AddNode("exp");
            context.AddNode("env");
            context.AddNode("backtest");
            context.AddNode("entry", "backtest");
            context.AddNode("exit", "entry");
            context.AddNode("query");
            context.AddNode("setup");
            context.AddNode("cmd");
            context.AddNode("data");
            context.AddNode("import");
            context.AddNode("metric");
            context.AddNode("study","metric");
            context.AddNode("help");
        }

        private static void Resize()
        {
            var x = Convert.ToInt32(System.Console.LargestWindowWidth*.65);
            var y = Convert.ToInt32(System.Console.LargestWindowHeight * .55);
            System.Console.SetWindowPosition(0, 0);
            System.Console.SetWindowSize(x, y);
        }

        private static void BufferHeight(int count)
        {
            int buffer = 5;
            if (Int16.MaxValue > System.Console.BufferHeight + count + buffer)
                System.Console.BufferHeight += count + buffer;
        }

        private static void WriteConsoleMessage(string message,bool newline=false)
        {
            string nl = newline ? "\n":"";
            System.Console.WriteLine("... " + message + " ..." + nl);
        }

        private static void Clear()
        {
            System.Console.Clear();
            System.Console.WriteLine("enter an expression and Enter key to evaluate.");
            System.Console.WriteLine("or enter '?' and the Enter key for help: \n");
            System.Console.Title = "MODE: " + (Utility.Configuration.IsTest() ? "TEST" : "LIVE");
        }
        
        private static bool Exit(string line)
        {
            bool exit = false;
            if (line.ToLower() == "exit") exit=true;
            return exit;
        }

        private static bool Exists(string line, string value)
        {
            var p = line.IndexOf(value, 0);
            return ((line.Trim().IndexOf(value, 0))==0) ? true : false;
        }

        private static bool AnyExists(string line, string value)
        {
            return ((line.Trim().IndexOf(value, 0))>-1) ? true : false;
        }

        private static void WritePrompt()
        {
            System.Console.Write(prompt);
        }

        interface ICommand
        {
            void Run();
        }

        private static void RunComand(string line)
        {
            if (Exists(line, "?"))//help
                Help(line);
            else if (Exists(line, "cd"))//change dir
                CD(line);
            else if (AnyExists(line, "<-"))//var assignment
                SetVariable(line);
            else if (Exists(line, "clear"))//clear console
                Clear();
            else if (Exists(line, "v"))//display variable/values
                Variable();
            else if (Exists(line, "m"))//modify variable value
                ModifyVariable(line);
            else if (Exists(line, "ls"))
                List();
            else if (Exists(line, "l"))//load object into cache
                Load(line);
            else if (Exists(line, "s"))//save variable/values
                Save();
            else if (Exists(line, "d"))//delete variable/values
                Delete();
            else if (Exists(line, "cy"))//copy an object
                Copy(line);
            else if (Exists(line, "c"))//create an object
                Create(line);
            else if (Exists(line, "export"))
                Export(line);
            else if ((Exists(line, "test")) && (Exists(prompt, "exp")))
                Test();
            else if ((Exists(line, "red")) && (Exists(prompt, "exp")))
                Reduce();
            else if ((Exists(line, "graph")) || (Exists(line, "history")) || (Exists(line, "summary")))//web site data
                DisplayInfo(line);
            else if (Exists(line, "r"))
                Run();
            else
                WriteConsoleMessage("invalid command", true);

        }

        static void Run()
        {
            if (Exists(prompt, "data"))
                Data();
            else if (Exists(prompt, "filter"))
                Filter();
            else if (Exists(prompt, "backtest"))
                BackTest();
            else if (Exists(prompt, "query"))
                Query();
            else if (Exists(prompt, "cmd"))
                Command();
            else if (Exists(prompt, "metric"))
                Metric();
            else
                WriteConsoleMessage("invalid object", true);
        }

        private static string BuildPromptName(string ctx)
        {
            string name = context.GetContextName(ctx);
            return ctx + ((name != null) ? "[" + name + "]" : "");
        }

        private static void BuildPrompt(string listvalue="")
        {
            var nc = context.NodeChain();
            string name = "";

            foreach (var n in nc)
                name += (name == string.Empty) ? BuildPromptName(n) : ("\\" + BuildPromptName(n));

            prompt = name + "> " + listvalue;
        }

        //use: cd search or cd ..
        private static void CD(string line)
        {
            var cmds = line.Split(' ');
            if (cmds.Length == 2)
            {
                var cmd=cmds[1].Trim();
                if(context.Navigate(cmd))
                    BuildPrompt();
                else
                    WriteConsoleMessage("invalid command", true);
                
            }
            else
                WriteConsoleMessage("invalid parameters", true);
        }

        #endregion

        #region crud

        #region create

        private static void Create(string line)
        {
            string[] values = ParseCommand(line);

            if(values.Length>0)
            {
                switch (context.ActiveNodeName())
                {
                    case "help": context.Create<Help>("help", values[0]); break;
                    case "exp": context.Create<Expression>("exp", values[0]); break;
                    case "filter": context.Create<Filter>("filter", values[0]); break;
                    case "backtest": context.Create<BackTest>("backtest", values[0]); break;
                    case "entry": context.Create<Entry>("entry", values[0]); break;
                    case "exit": context.Create<Exit>("exit", values[0]); break;
                    case "price": context.Create<Price>("price", values[0]); break;
                    case "cmd": context.Create<Command>("cmd", values[0]); break;
                    case "query": context.Create<Query>("query", values[0]); break;
                    case "data": context.Create<Data>("data", values[0]); break;
                    case "import": context.Create<Import>("import", values[0]); break;
                    case "metric": context.Create<Metric>("metric", values[0]); break;
                    case "study": context.Create<Study>("study", values[0]); break;
                    case "env": context.Create<Tw.Model.Entity.Environment>("env", values[0]); break;
                }
                BuildPrompt();
            }
            else
                WriteConsoleMessage("invalid name");

        }

        private static void Copy(string line)
        {
            
            if (!ConfirmAction("copy")) return;
            
            string[] values = ParseCommand(line);
            
            if (values.Length > 0)
            {
                switch (context.ActiveNodeName())
                {
                    case "help": context.Copy<Help>("help", values[0]); break;
                    case "exp": context.Copy<Expression>("exp", values[0]); break;
                    case "filter": context.Copy<Filter>("filter", values[0]); break;
                    case "backtest": context.Copy<BackTest>("backtest", values[0]); break;
                    case "entry": context.Copy<Entry>("entry", values[0]); break;
                    case "exit": context.Copy<Exit>("exit", values[0]); break;
                    case "price": context.Copy<Price>("price", values[0]); break;
                    case "query": context.Copy<Query>("query", values[0]); break;
                    case "cmd": context.Copy<Command>("cmd", values[0]); break;
                    case "data": context.Copy<Data>("data", values[0]); break;
                    case "import": context.Copy<Import>("import", values[0]); break;
                    case "metric": context.Copy<Metric>("metric", values[0]); break;
                    case "study": context.Copy<Study>("study", values[0]); break;
                    case "env": context.Copy<Tw.Model.Entity.Environment>("env", values[0]); break;
                }

                Save(false);//auto save
                BuildPrompt();
            }
            else
                WriteConsoleMessage("invalid name");

        }
        #endregion

        #region load
        private static void Load(string line)
        {
            string[] values = ParseCommand(line);

            if (values.Length > 0)
            {
                switch (context.ActiveNodeName())
                {
                    case "help": LoadContext<Help>("help", values[0]); break;
                    case "exp": LoadContext<Expression>("exp", values[0]); break;
                    case "filter": LoadContext<Filter>("filter", values[0]); break;
                    case "backtest": LoadContext<BackTest>("backtest", values[0]); break;
                    case "entry": LoadContext<Entry>("entry", values[0]); break;
                    case "exit": LoadContext<Exit>("exit", values[0]); break;
                    case "query": LoadContext<Query>("query", values[0]); break;
                    case "cmd": LoadContext<Command>("cmd", values[0]); break;
                    case "data": LoadContext<Data>("data", values[0]); break;
                    case "import": LoadContext<Import>("import", values[0]); break;
                    case "env": LoadContext<Tw.Model.Entity.Environment>("env", values[0]); break;
                    case "metric": LoadContext<Tw.Model.Entity.Metric>("metric", values[0]); break;
                    case "study": LoadContext<Tw.Model.Entity.Study>("study", values[0]); break;
                }
            }
            else
                WriteConsoleMessage("Invalid name");

        }
               
        private static void LoadContext<T>(string ctx,string name)
        {
            var r = new ViewHelper();
            object o=default(T);

            switch (ctx)
            {
                case ("help"): o = r.Help(name); break;
                case ("exp"): o = r.Expression(name); break;
                case ("env"): o = r.Environment(name); break;
                case ("filter"): o = r.Filter(name); break;
                case ("query"): o = r.Query(name); break;
                case ("backtest"): o = r.BackTest(name); break;
                case ("cmd"): o = r.Command(name); break;
                case ("data"): o = r.Data(name); break;
                case ("import"): o = r.Import(name); break;
                case ("metric"): o = r.Metric(name); break;//
                case ("study"): o = r.Study(context.GetContextName("metric"), name); break;
                case ("entry"): o = r.Entry(context.GetContextName("backtest"),name) ; break;
                case ("exit"): o = r.Exit(context.GetContextName("backtest"),context.GetContextName("entry"), name); break;
            }
            if (o != null)
            {
                context.Load(ctx, (T)o);
                BuildPrompt();
            }
            else
                WriteConsoleMessage("invalid name");
        }

        #endregion

        #region save
        private static void Save(bool warn = true)
        {

            bool success = false;

            if ((warn) && (!ConfirmAction("save"))) return;

            switch (context.ActiveNodeName())
            {
                case "help": success = SaveHelp(); break;
                case "exp": success = SaveExpression(); break;
                case "env": success = SaveEnvironment(); break;
                case "filter": success = SaveFilter(); break;
                case "backtest": success = SaveBacktest(); break;
                case "query": success = SaveQuery(); break;
                case "cmd": success = SaveCommand(); break;
                case "entry": success = SaveEntry(); break;
                case "exit": success = SaveExit(); break;
                case "import": success = SaveExit(); break;
                case "data": success = SaveData(); break;
                case "metric": success = SaveMetric(); break;
                case "study": success = SaveStudy(); break;
            }

            WriteStatus(success);
        }

        private static bool SaveHelp()
        {
            var p = new PersistanceHelper();
            return p.Save<Help>(context.GetObject<Help>("help"));
        }

        private static bool SaveImport()
        {
            var p = new PersistanceHelper();
            return p.Save<Import>(context.GetObject<Import>("import"));
        }

        private static bool SaveExpression()
        {
            var p = new PersistanceHelper();
            return p.Save<Expression>(context.GetObject<Expression>("exp"));
        }

        private static bool SaveEnvironment()
        {
            var p = new PersistanceHelper();
            return p.Save<Tw.Model.Entity.Environment>(context.GetObject<Tw.Model.Entity.Environment>("env"));
        }

        private static bool SaveFilter()
        {
            var p = new PersistanceHelper();
            return p.Save<Filter>(context.GetObject<Filter>("filter"));
        }

        private static bool SaveBacktest()
        {
            var p = new PersistanceHelper();
            return p.Save<BackTest>(context.GetObject<BackTest>("backtest"));
        }

        private static bool SaveQuery()
        {
            var p = new PersistanceHelper();
            return p.Save<Query>(context.GetObject<Query>("query"));
        }

        private static bool SaveData()
        {
            var p = new PersistanceHelper();
            return p.Save<Data>(context.GetObject<Data>("data"));
        }

        private static bool SaveMetric()
        {
            var p = new PersistanceHelper();
            return p.Save<Metric>(context.GetObject<Metric>("metric"));
        }

        private static bool SaveStudy()
        {
            var t = context.GetObject<Metric>("metric");
            var e = context.GetObject<Study>("study");

            t.Study.RemoveAll(a => a.Id == e.Id);
            t.Study.Add(e);

            var p = new PersistanceHelper();
            return p.Save<Metric>(t);
        }

        private static bool SaveCommand()
        {
            var p = new PersistanceHelper();
            return p.Save<Command>(context.GetObject<Command>("cmd"));
        }

        private static bool SaveEntry()
        {
            var t = context.GetObject<BackTest>("backtest");
            var e = context.GetObject<Entry>("entry");

            t.Entry.RemoveAll(a => a.Id == e.Id);
            t.Entry.Add(e);
            
            var p = new PersistanceHelper();
            return p.Save<BackTest>(t);
        }

        private static bool SaveExit()
        {
            var t = context.GetObject<BackTest>("backtest");
            var e = context.GetObject<Entry>("entry");
            var x = context.GetObject<Exit>("exit");

            if (!t.Entry.Exists(a => a.Id == e.Id)) 
                t.Entry.Add(e);

            e.Exit.RemoveAll(b => b.Id == x.Id);
            e.Exit.Add(x);

            t.Entry.RemoveAll(b => b.Id == e.Id);
            t.Entry.Add(e);

            var p = new PersistanceHelper();
            return p.Save<BackTest>(t);
        }

        #endregion

        #region delete
        private static void Delete()
        {

            int count = 0;

            if (!ConfirmAction("delete")) return;

            switch (context.ActiveNodeName())
            {
                case "help": count = DeleteHelp(); break;
                case "exp": count = DeleteExpression(); break;
                case "env": count = DeleteEnvironment(); break;
                case "backtest": count = DeleteBackTest(); break;
                case "cmd": count = DeleteCommand(); break;
                case "entry": count = DeleteEntry(); break;
                case "exit": count = DeleteExit(); break;
                case "filter": count = DeleteFilter(); break;
                case "data": count = DeleteData(); break;
                case "query": count = DeletQuery(); break;
                case "import": count = DeleteImport(); break;
                case "metric": count = DeleteMetric(); break;
                case "study": count = DeleteStudy(); break;
            }

            if (count>0)
            {
                context.ClearActiveContext();
                BuildPrompt();
            }

            WriteStatus(count>0);
        }

        private static int DeleteHelp()
        {
            var r = new PersistanceHelper();
            return r.Delete<Help>(context.GetObject<Help>("help"));
        }

        private static int DeleteMetric()
        {
            var r = new PersistanceHelper();
            return r.Delete<Metric>(context.GetObject<Metric>("metric"));
        }

        private static int DeleteStudy()
        {
            var r = new PersistanceHelper();
            return r.Delete<Study>(context.GetObject<Study>("study"));
        }

        private static int DeleteImport()
        {
            var r = new PersistanceHelper();
            return r.Delete<Import>(context.GetObject<Import>("import"));
        }

        private static int DeleteEnvironment()
        {
            var r = new PersistanceHelper();
            return r.Delete <Tw.Model.Entity.Environment> (context.GetObject<Tw.Model.Entity.Environment>("env"));
        }

        private static int DeleteData()
        {
            var r = new PersistanceHelper();
            return r.Delete<Data>(context.GetObject<Data>("data"));
        }

        private static int DeleteExpression()
        {
            var r = new PersistanceHelper();
            return r.Delete<Expression>(context.GetObject<Expression>("exp"));
        }

        private static int DeleteFilter()
        {
            var r = new PersistanceHelper();
            return r.Delete<Filter>(context.GetObject<Filter>("filter"));
        }

        private static int DeletData()
        {
            var r = new PersistanceHelper();
            return r.Delete<Data>(context.GetObject<Data>("data"));
        }

        private static int DeletQuery()
        {
            var r = new PersistanceHelper();
            return r.Delete<Query>(context.GetObject<Query>("query"));

        }
        private static int DeleteBackTest()
        {
            var r = new PersistanceHelper();
            return r.Delete<BackTest>(context.GetObject<BackTest>("backtest"));
        }

        private static int DeleteCommand()
        {
            var r = new PersistanceHelper();
            return r.Delete<Command>(context.GetObject<Command>("cmd"));
        }

        private static int DeleteEntry()
        {
            var r = new PersistanceHelper();
            var t = context.GetObject<BackTest>("backtest");
            var e = context.GetObject<Entry>("entry");
            t.Entry.RemoveAll(a => a.Id == e.Id);
            return r.Save<BackTest>(t) ? 1 : 0;
        }

        private static int DeleteExit()
        {
            var r = new PersistanceHelper();
            var t = context.GetObject<BackTest>("backtest");
            var e = context.GetObject<Entry>("entry");
            var x = context.GetObject<Exit>("exit");
            e.Exit.RemoveAll(a => a.Id == x.Id);
            return r.Save<BackTest>(t) ? 1 : 0;
        }

        #endregion

        #region property variable

        private static void Variable()
        {

            var obj = context.GetActiveObject();
            if (obj != null)
            {
                WriteConsoleMessage("variables");
                var prop = obj.GetType().GetProperties();
                foreach (var p in prop)
                {
                    if (Utility.Attribute.IsBinding(p))
                        System.Console.WriteLine(Pad(p.Name.ToLower() + ":",15) + p.GetValue(obj, null));
                }
            }
            else
                WriteConsoleMessage("no object(s) found");

            System.Console.WriteLine("\n");
        }


        private static void SetVariable(string line)
        {
            string[] values = ParseCommand(line);

            var obj = context.GetActiveObject();

            if (obj == null)
                WriteConsoleMessage("no object(s) found, please create one before updating variables.");
            else
            {
                values = System.Text.RegularExpressions.Regex.Split(line, "<-");
                var props = obj.GetType().GetProperties();
                var p = props.FirstOrDefault(x => x.Name.ToLower() == values[0].Trim().ToLower());
                if (p == null)
                    WriteConsoleMessage("no property(s) found.");
                else
                    p.SetValue(obj, Convert.ChangeType(values[1].Trim(), p.PropertyType), null);

                BuildPrompt();
            }

        }

        private static void SendKeys(string name, object value)
        {
            value = value.ToString().Replace("(", "{(}");
            value = value.ToString().Replace(")", "{)}");
            value = value.ToString().Replace("+", "{+}");
            value = value.ToString().Replace("^", "{^}");
            value = value.ToString().Replace("~", "{~}");
            value = value.ToString().Replace("%", "{%}");
            System.Windows.Forms.SendKeys.SendWait(name + "<-" + value);
        }

        private static void ModifyVariable(string line)
        {
            string[] values = ParseCommand(line);
            var obj = context.GetActiveObject();
            object value =null;

            if (obj == null)
                WriteConsoleMessage("no object(s) found, please create one before updating variables.");
            else
            {
                var props = obj.GetType().GetProperties();
                var p = props.FirstOrDefault(x => x.Name.ToLower() == values[0].ToLower());
                if (p == null)
                    WriteConsoleMessage("no property(s) found.");
                else
                    value = p.GetValue(obj, null);

                if (value != null)
                {
                    SendKeys(p.Name.ToLower(),value);
                }
            }

        }

        #endregion

        #endregion

        #region common

        private static string[] ExcludeKeyWord(string[] values)
        {
            List<string> new_value = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                var v = Clean(values[i]);
                if(keywordcheck==false)
                    new_value.Add(v);
                else if ((!keywords.Contains(v)) && (!context.NodeExists(v)))
                    new_value.Add(v);
            }

            return new_value.ToArray();
        }

        private static string NVL(string[] values, int pos, string defaultValue)
        {
            if ((values != null) && (values.Length > pos))
                return values[pos];
            else
                return defaultValue;
        }

        //format: save exp test (1>2)&(2>3) "test 123"
        private static string[] ParseCommand(string line)
        {
            var v = line.Split(' ');
            return ExcludeKeyWord(v);
        }

        private static bool ConfirmAction(string action)
        {
            bool confirmed=false;

            var name = context.ActiveContextName();

            if (name != null)
            {
                string confirm = action.ToLower() + " " + context.ActiveNodeName(); 
                confirm += " '" + name + "'";

                System.Console.Write(confirm + ": y/n? ");
                var yesno = System.Console.ReadLine().ToLower().Trim();

                if ((yesno.Length == 1) && (yesno == "y"))
                    confirmed = true;
                else
                    confirmed = false;
            }
            
            return confirmed;

        }

        private static void WriteStatus(bool success)
        {
            if (success)
                WriteConsoleMessage("success", true);
            else
                WriteConsoleMessage("error", true);
        }

        private static string Clean(string value)
        {
            return value.ToLower().Replace(" ", "");
        }

        private static string Pad(object value, int width, bool right = true)
        {
            if (right)
                return ((value != null) ? value.ToString() : "").PadRight(width);
            else
                return ((value != null) ? value.ToString() : "").PadLeft(width);
        }

        #endregion

        #region list

        private static void ListFunction()
        {
            var query = new ViewHelper();
            var results = query.Expression();

            var builtin = results.Where(x => x.Type != "user_defined");
            BufferHeight(builtin.Count());
            /*
            type  func     description      example 
            data  o(n1)     open price      o(0) would return 1st day or most recent open price.
            data  c(n1)     close price     c(0) would return 1st day or most recent close price.
            h = high  price
            l = low 
              d = date
              v = volume.
            functions:
              max
             * */
            System.Console.WriteLine(Pad("type",10) + Pad("func",10) + "description");

            results.ForEach(delegate(Expression value)
            {
                System.Console.WriteLine(String.Format(Pad(value.Name,25) + value.Value));
            });

            WriteConsoleMessage(results.Count + " record(s) found", true);

        }

        private static void ListStudy()
        {
            var results = context.GetObject<Metric>("metric").Study;

            if (results.Count() > 0)
            {
                BufferHeight(results.Count * 11);

                System.Console.WriteLine(Pad("name", 25) +
                         Pad("active", 10) +
                         Pad("expression", 50) +
                         "description");

                results.ForEach(delegate(Study value)
                {
                    System.Console.WriteLine(Pad(value.Name, 25) +
                                                Pad(value.Active, 10) +
                                                Pad(value.Expression, 50) +
                                                value.Description);
                });

            }

            WriteConsoleMessage(results.Count + " record(s) found", true);

        }

        private static void ListMetric()
        {
            var query = new ViewHelper();
            var results = query.Metric();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count * 11);

                System.Console.WriteLine(Pad("name", 25) +
                         Pad("exchange", 30) +
                         Pad("type", 10) +
                         Pad("groupby", 10) +
                         Pad("interval", 10) +
                         Pad("periodstart", 15) +
                         Pad("periodend", 15) +
                         Pad("study*", 10) +
                         "description");

                results.ForEach(delegate(Metric value)
                {
                    System.Console.WriteLine(Pad(value.Name, 25) +
                                                Pad(value.Exchange, 30) +
                                                Pad(value.Type, 10) +
                                                Pad(value.GroupBy, 10) +
                                                Pad(value.Interval, 10) +
                                                Pad(value.PeriodStart, 15) +
                                                Pad(value.PeriodEnd, 15) +
                                                Pad(value.Study.Count, 10) +
                                                value.Description);
                });

            }

            WriteConsoleMessage(results.Count + " record(s) found", true);

        }

        private static void ListData()
        {
            var query = new ViewHelper();
            var results = query.Data();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count*11);

                System.Console.WriteLine(Pad("name", 25) +
                                           Pad("object", 15) +
                                           Pad("operation", 15) +
                                           "description");

                results.ForEach(delegate(Data value)
                {
                    System.Console.WriteLine(Pad(value.Name, 25) +
                                             Pad(value.Object, 15) +
                                             Pad(value.Operation, 15) +
                                             value.Description);
                });
            }
            WriteConsoleMessage(results.Count + " record(s) found", true);
        }

        private static void ListExpression()
        {
            var query = new ViewHelper();
            var results = query.Expression();
            var sorted = results.OrderBy(x => x.Category).ToList();

            if (sorted.Count() > 0)
            {
                BufferHeight(results.Count);

                System.Console.WriteLine(Pad("category", 15) +
                                         Pad("name",25) +
                                         "description");

                sorted.ForEach(delegate(Expression value)
                {
                    System.Console.WriteLine(Pad(value.Category, 15) +
                                             Pad(value.Name,25) +
                                             value.Description);
                });
            }
            WriteConsoleMessage(sorted.Count + " record(s) found", true);

        }

        private static void ListEnvironment()
        {
            var query = new ViewHelper();
            var results = query.Environment();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count());

                System.Console.WriteLine(Pad("active",10) +
                                         Pad("name",20) +
                                         "value");

                results.ForEach(delegate(Tw.Model.Entity.Environment v)
                {
                    System.Console.WriteLine(Pad(v.Active,10) +
                                             Pad(v.Name,20) +
                                             v.Value);
                });
            }
            WriteConsoleMessage(results.Count + " record(s) found", true);
        }

        private static void ListCommand()
        {
            var query = new ViewHelper();
            var results = query.Command();
            var sorted = results.OrderBy(x => x.Name).ToList();

            if (sorted.Count() > 0)
            {
                BufferHeight(results.Count());

                System.Console.WriteLine(Pad("name", 15) +
                                         Pad("exporttype", 15) +
                                         Pad("expression", 25) +
                                         "description");

                sorted.ForEach(delegate(Tw.Model.Entity.Command v)
                {
                    System.Console.WriteLine(Pad(v.Name, 15) +
                                             Pad(v.ExportType, 15) +
                                             Pad(v.Expression, 25) +
                                             v.Description);
                });
            }
            WriteConsoleMessage(sorted.Count + " record(s) found", true);
        }

        private static void ListImport()
        {
            var query = new ViewHelper();
            var results = query.Import();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count());

                System.Console.WriteLine(Pad("object", 15) +
                                         Pad("category", 15) +
                                         Pad("name", 25) +
                                         Pad("file", 25) +
                                         Pad("count", 15) +
                                         Pad("status", 15) +
                                         Pad("date", 15) +
                                        "description");

                results.ForEach(delegate(Tw.Model.Entity.Import v)
                {
                    System.Console.WriteLine(Pad(v.Object, 15) +
                                             Pad(v.Category, 15) +
                                             Pad(v.Name, 25) +
                                             Pad(v.File, 25) +
                                             Pad(v.Count, 15) +
                                             Pad(v.Status, 15) +
                                             Pad(v.Date, 15) +
                                             v.Description);
                });
            }
            WriteConsoleMessage(results.Count + " record(s) found", true);
        }

        private static void ListHelp()
        {
            var query = new ViewHelper();
            var results = query.Help();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count());

                System.Console.WriteLine(Pad("name", 10) +
                                         Pad("category", 10) +
                                         Pad("use", 25) +
                                         Pad("example", 25) +
                                         "description");

                results.ForEach(delegate(Tw.Model.Entity.Help v)
                {
                    System.Console.WriteLine(Pad(v.Name, 10) +
                                             Pad(v.Category, 25) +
                                             Pad(v.Use, 25) +
                                             Pad(v.Example, 25) +
                                             v.Description);
                });
            }
            WriteConsoleMessage(results.Count + " record(s) found", true);
        }

        private static void ListFilter()
        {
            var query = new ViewHelper();
            var results = query.Filter();
            var sorted = results.OrderBy(x => x.Name).ToList();

            if (sorted.Count() > 0)
            {
                BufferHeight(sorted.Count());

                System.Console.WriteLine(Pad("name",15) +
                                         Pad("exchange",20) +
                                         Pad("interval",10) + 
                                         Pad("period",10) +
                                         "description");

                sorted.ForEach(delegate(Tw.Model.Entity.Filter v)
                {
                    System.Console.WriteLine(Pad(v.Name,15) + 
                                             Pad(v.Exchange,20) + 
                                             Pad(v.Interval,10) + 
                                             Pad(v.Period,10) +
                                             v.Description);
                });
            }
            WriteConsoleMessage(sorted.Count + " record(s) found", true);
        }

        private static void ListBackTest()
        {
            var query = new ViewHelper();
            var results = query.BackTest();
            var sorted = results.OrderBy(x => x.Name).ToList();

            if (sorted.Count() > 0)
            {
                BufferHeight(sorted.Count());

                System.Console.WriteLine(Pad("name", 15) +
                                         Pad("interval", 10) +
                                         Pad("startdate", 10) +
                                         Pad("enddate", 10) +
                                         Pad("capital", 10) +
                                         Pad("risk", 10) +
                                         Pad("max pos.", 10) +
                                         Pad("commission", 15) +
                                         Pad("entry*", 10) +
                                         "description");

                sorted.ForEach(delegate(Tw.Model.Entity.BackTest v)
                {
                    System.Console.WriteLine(Pad(v.Name, 15) +
                                             Pad(v.PeriodInterval, 10) +
                                             Pad(v.PeriodStart, 10) +
                                             Pad(v.PeriodEnd, 10) +
                                             Pad(v.Capital, 10) +
                                             Pad(v.Risk, 10) +
                                             Pad(v.MaxPosition, 10) +
                                             Pad(v.Commission, 15) +
                                             Pad(v.Entry.Count, 10) +
                                             v.Description);
                });
            }
            WriteConsoleMessage(sorted.Count + " record(s) found", true);
        }

        private static void ListEntry()
        {
            var results = context.GetObject<BackTest>("backtest");

            if (results.Entry.Count > 0)
            {
                BufferHeight(results.Entry.Count());

                System.Console.WriteLine(Pad("active", 10) +
                                         Pad("name", 10) +
                                         Pad("entrytype", 10) +
                                         Pad("risktype", 10) +
                                         Pad("risk", 10) +
                                         Pad("stoploss", 10) +
                                         Pad("timelimit", 10) +
                                         Pad("exit*", 10) +
                                         "description");

                results.Entry.ForEach(delegate(Tw.Model.Entity.Entry v)
                {
                    System.Console.WriteLine(Pad(v.Active, 10) +
                                             Pad(v.Name, 10) +
                                             Pad(v.EntryType, 10) +
                                             Pad(v.RiskType, 10) +
                                             Pad(v.Risk, 10) +
                                             Pad(v.StopLoss, 10) +
                                             Pad(v.TimeLimit, 10) +
                                             Pad(v.Exit.Count, 10) +
                                             Pad(v.Description, 0)
                                             );
                });
            }
            WriteConsoleMessage(results.Entry.Count + " record(s) found", true);
        }

        private static void ListExit()
        {
            var results = context.GetObject<Entry>("entry");
            
            if (results.Exit.Count() > 0)
            {
                BufferHeight(results.Exit.Count());

                System.Console.WriteLine(Pad("active", 10) +
                                         Pad("name", 15) +
                                         Pad("risktype", 10) +
                                         Pad("risk", 10) +
                                         "description");

                results.Exit.ForEach(delegate(Tw.Model.Entity.Exit v)
                {
                    System.Console.WriteLine(Pad(v.Active, 10) +
                                             Pad(v.Name, 15) +
                                             Pad(v.RiskType, 10) +
                                             Pad(v.Risk, 10) +
                                             Pad(v.Description, 0)
                                             );
                });
            }
            WriteConsoleMessage(results.Exit.Count + " record(s) found", true);
        }

        private static void ListQuery()
        {
            var query = new ViewHelper();
            var results = query.Query();

            if (results.Count() > 0)
            {
                BufferHeight(results.Count());

                System.Console.WriteLine(Pad("name", 15) +
                                         Pad("object", 15) +
                                         Pad("exp1", 20) +
                                         Pad("exp2", 20) +
                                         Pad("exp3", 20) +
                                         "exp4");

                results.ForEach(delegate(Tw.Model.Entity.Query v)
                {
                    System.Console.WriteLine(Pad(v.Name, 15) +
                                             Pad(v.Object, 15) +
                                             Pad(v.Exp1, 20) +
                                             Pad(v.Exp2, 20) +
                                             Pad(v.Exp3, 20) +
                                             v.Exp4
                                             );
                });
            }
            WriteConsoleMessage(results.Count + " record(s) found", true);
        }

        //example: li *
        private static void List()
        {
            WriteConsoleMessage("running");

            switch(context.ActiveNodeName())
            {
                case "help": ListHelp(); break;
                case "filter": ListFilter(); break;
                case "env": ListEnvironment(); break;
                case "exp": ListExpression(); break;
                case "func": ListFunction(); break;
                case "backtest": ListBackTest(); break;
                case "entry": ListEntry(); break;
                case "exit": ListExit(); break;
                case "query": ListQuery(); break;
                case "cmd": ListCommand(); break;
                case "import": ListImport(); break;
                case "data": ListData(); break;
                case "metric": ListMetric(); break;
                case "study": ListStudy(); break;
                default: WriteConsoleMessage("object not found"); break;
            }
        }
        #endregion

        #region execute

        #region help

        private static void Help(string line)
        {
            //? cd
            var cmds = line.Split(' ');
            var v = new ViewHelper();

            //?
            if (cmds.Length == 1)
            {
                var help = v.Help().OrderBy(x => x.Category).ThenBy(y => y.Name);
                foreach (var o in help)
                {
                    System.Console.WriteLine(Pad(o.Name, 10) + o.Description);
                }
                System.Console.WriteLine("\n");
            }
            else
            {
                var o = v.Help(cmds[1].ToLower());
                if (o != null)
                {
                    System.Console.WriteLine(Pad("description:", 20) + o.Description + System.Environment.NewLine +
                                             Pad("use:", 20) + o.Use + System.Environment.NewLine +
                                             Pad("example:", 20) + o.Example + System.Environment.NewLine);
                }
                else
                    WriteConsoleMessage("command not found");
            }

        }
        #endregion

        #region data

        private static void DataImport(Data o)
        {
            var r = new Util.Data.Import();
            r.ImportToDB(o);
        }

        private static void DataIndex(Data o)
        {
            var r = new Util.Data.Index();
            r.AlterIndex(o);
        }

        private static void DataExport(Data o)
        {
            var r = new Util.Data.Export();
            r.ExportToFile(o);
        }

        private static void DeleteData(Data o)
        {
            var r = new Util.Data.Ops();
            var count = r.Delete(o);
            System.Console.WriteLine(String.Format("{0} {1} records deleted",count, o.Object));
        }

        private static void Data()
        {
            var o = context.GetObject<Data>("data");
            
            if(o.Operation=="import")
            {
                DataImport(o);
            }
            else if (o.Operation == "index")
            {
                DataIndex(o);
            }
            else if (o.Operation == "export")
            {
                DataExport(o);
            }
            else if (o.Operation == "delete")
            {
                DeleteData(o);
            }
        }

        private static void Export<T>(List<T> objects) where T : class
        {
            WriteConsoleMessage("running");

            var export = new Util.Data.Export();
            var file = System.IO.Path.GetRandomFileName();
            file = file.Substring(0, file.Length - 4) + ".csv";
            var temp = System.IO.Path.GetTempPath() + "\\" + file;
            export.ExportToFile<T>(temp,objects);

            WriteConsoleMessage("object exported");
        }

        #endregion

        #region Run

        private static void Command()
        {
            var o = context.GetObject<Command>("cmd");
            var r = new Tw.Service.Repository.CommandRepository();
            r.Run(o, System.Console.Out);
            var report = r.Report();
            BufferHeight(LineCount(report));
            System.Console.WriteLine(report);
        }

        private static void Metric()
        {
            WriteConsoleMessage("running");

            var o = context.GetObject<Metric>("metric");
            var r = new AnalysisRepository();
            var v = new MetricView();
            var q = new ViewHelper();

            v = r.Metric(o);

            BufferHeight(v.RowCount() + 11);
            System.Console.WriteLine(v.Report());

            if (v.RowCount() > 0 && v.ExportType != "")
                Tw.Service.Common.Utility.File.Export(v.Export(), v.ExportType);
            
        }

        private static void BackTest()
        {
            WriteConsoleMessage("running");

            var o = context.GetObject<BackTest>("backtest");
            var r = new AnalysisRepository();
            var v = new BackTestView();
            var q = new ViewHelper();
            var c = o.SampleData.Split(',');

            if (c.Length > 0)
            {
                var x = q.Company(c).Company;
                v = r.BacktTest(x, o);
                CacheView("backtest", v);
            }

            BufferHeight(v.Result.Count() + v.Error.Count);
            System.Console.WriteLine(v.Report());
            v.Xls();
                
        }

        private static void Filter()
        {
            WriteConsoleMessage("running");

            var r = new AnalysisRepository();
            var v = new FilterView();

            try
            {
                var o = context.GetObject<Filter>("filter");
                v = r.Filter(o);
                CacheView("filter", v);
            }
            catch (Exception e)
            {
                v.Error.Add(new Error("FilterResults", e.Message));
                System.Console.WriteLine(String.Format("error '{0}' occurred !!!", e.Message));
            }

            BufferHeight(v.Company.Count() + v.Error.Count);
            System.Console.WriteLine(v.Report());
            
        }

        #endregion

        #region query

        private static void Query()
        {
            WriteConsoleMessage("running");

            var q = context.GetObject<Query>("query");
            var qo = BuildQueryObject(q);
            switch (q.Object)
            {
                case "price": QueryPrice(qo); break;
                case "company": QueryCompany(qo); break;
                case "chart": break;
                case "summary": break;
                case "history": break;
            }
        }

        class QueryObject
        {
            public string Name { get; set; }
            public string Op { get; set; }
            public string Value { get; set; }
        }

        static void AppendQueryObject(List<QueryObject> qo, string exp)
        {
            if((exp != "")&&(exp!=null))
            {
                string ops = "(<=|>=|!=|=|>|<)";
                var item = System.Text.RegularExpressions.Regex.Split(exp, ops);
                qo.Add( new QueryObject() { Name = item[0], Op = item[1], Value = item[2] });
            }
        }

        static List<QueryObject> BuildQueryObject(Query q)
        {
            List<QueryObject> QO = new List<QueryObject>();

            AppendQueryObject(QO, q.Exp1);
            AppendQueryObject(QO, q.Exp2);
            AppendQueryObject(QO, q.Exp3);
            AppendQueryObject(QO, q.Exp4);

            return QO;
        }

        //query company nyse
        private static void QueryCompany(List<QueryObject> qo)
        {
            var v = new ViewHelper();
            var r = QueryCompanyResults(qo);

            BufferHeight(r.Count);

            System.Console.WriteLine(Pad("exchange",10) +
                                     Pad("symbol",10) +
                                     "company");

            r.ForEach(delegate(Company c)
            {
                System.Console.WriteLine(Pad(c.Exchange,10) +
                                         Pad(c.Symbol,10) +
                                         c.Name);
            });

            WriteConsoleMessage(r.Count + " record(s) found", true);
        }

        private static List<Price> QueryPriceResults(List<QueryObject> qo)
        {
            var query = new ViewHelper();
            var prices = new List<Price>();
            //parse exp : q.Exp1
            string exchange="";
            string symbol="";
            int period = 0;
            int enddate = 0;
            int startdate = 0;
            
            foreach(var q in qo)
            {
                switch(q.Name.ToLower())
                {
                    case "exchange": exchange = q.Value; break;
                    case "symbol": symbol = q.Value; break;
                    case "period": period = Int32.Parse(q.Value); break;
                    case "enddate": enddate = Int32.Parse(q.Value); break;
                    case "startdate": startdate = Int32.Parse(q.Value); break;
                }
            }

            //combos: enddate/startdate, enddate+period,exchange,
            if ((enddate > 0) && (startdate>0))
                prices = query.PriceBySymbol(exchange, symbol, startdate, enddate);
            else if ((enddate == 0) && (startdate > 0))
                prices = query.PriceBySymbol(exchange, symbol, startdate);

            return prices;
        }

        private static List<Company> QueryCompanyResults(List<QueryObject> qo)
        {
            var query = new ViewHelper();
            //parse exp : q.Exp1
            string exch = "";
            foreach (var q in qo)
            {
                switch (q.Name.ToLower())
                {
                    case "exchange": exch = q.Value; break;
                }
            }

            return query.Company(exch);
        }

        private static void QueryPrice(List<QueryObject> qo)
        {
            var prices = QueryPriceResults(qo);

            BufferHeight(prices.Count());

            System.Console.WriteLine(Pad("symbol",10) + Pad("date",10) + Pad("open",10) + Pad("high",10) + Pad("low",10) + Pad("close",10) + "volume");

            prices.ForEach(delegate(Price v)
            {
                System.Console.WriteLine(Pad(v.Symbol,10) +
                                         Pad(v.Date,10) +
                                         Pad(v.Open,10) +
                                         Pad(v.High,10) +
                                         Pad(v.Low,10) +
                                         Pad(v.Close, 10) +
                                         v.Volume.ToString());
            });

            WriteConsoleMessage(prices.Count() + " record(s) found", true);
        }
        #endregion


        #region misc

        static void CacheView(string key, IView v)
        {
            reportcache.Remove(key);
            reportcache.Add(key, v);
        }

        private static string[] Prompt()
        {

            var dlg = new System.Windows.Forms.OpenFileDialog();

            dlg.Filter = "CSV Files (.csv)|*.txt|All Files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.Multiselect = true;

            dlg.ShowDialog();

            return dlg.FileNames;
        }

        private static int LineCount(string text)
        {
            var RE = new System.Text.RegularExpressions.Regex("\n", System.Text.RegularExpressions.RegexOptions.Multiline);
            var matches = RE.Matches(text);
            return matches.Count;
        }

        private static void Reduce()
        {
            var o = context.GetObject<Expression>("exp");

            if ((o != null) && (o.Value != null))
            {
                var r = new Tw.Service.Repository.AnalysisRepository();
                var results = "reduction: ".PadRight(15) + r.Reduce(o.Value);

                System.Console.WriteLine(results, true);
            }
            else
                System.Console.WriteLine("invalid object", true);
        }

        private static void Test()
        {
            var o = context.GetObject<Expression>("exp");

            if ((o != null) && (o.Value!=null))
            {
                var r = new Tw.Service.Repository.AnalysisRepository();
                var red = r.Reduce(o.Value);
                var results = Pad("reduction: ", 15) + red;
                results += System.Environment.NewLine + Pad("value: ", 15) + r.Test(red);

                System.Console.WriteLine(results, true);
            }
            else
                System.Console.WriteLine("invalid object", true);
            
        }

        private static void Export(string line)
        {
            var v=new ViewHelper();
            
            switch(context.ActiveNodeName())
            {
                case "filter": Export<Filter>(v.Filter()); break;
                case "env": Export<Tw.Model.Entity.Environment>(v.Environment()); break;
                case "exp": Export<Expression>(v.Expression()); break;
                case "backtest": Export<BackTest>(v.BackTest()); break;
                case "query": Export<Query>(v.Query()); break;
                case "entry": Export<Query>(v.Query()); break;
                case "exit": Export<Query>(v.Query()); break;
                default: WriteConsoleMessage("object not found"); break;
            }

        }

        #endregion

        #region web display
        private static void DisplayInfo(string line)
        {
            var web = new Util.Web();
            var cmd = line.Split(' ');
            var request = cmd[0];
            var symbol = cmd[1].Trim();
            var startdate = DateTime.Now.AddDays(-30);
            var enddate = DateTime.Now;

            if (cmd.Length == 4)
            {
                startdate = DateTime.ParseExact(cmd[2], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                enddate = DateTime.ParseExact(cmd[3], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            if (Exists(request, "summary"))
                web.Summary(symbol);
            else if (Exists(request, "graph"))
            {
                switch (context.ActiveNodeName())
                {
                    case "backtest": DisplayBackTestChart(symbol); break;
                    case "filter": DisplayFilterChart(symbol); break;
                }
            }
            else if (Exists(request, "history"))
                web.History(symbol, startdate, enddate);

        }

        static void DisplayFilterChart(string symbol)
        {
            var o = (FilterView)reportcache["filter"];
            var company = o.Company.FirstOrDefault(x => x.Symbol.ToLower() == symbol);

            if (company == null)
            {
                System.Console.WriteLine("missing symbol...");
                return;
            }

            int period = o.Period;
            if(period<60) period=60;//set minimum amount

            int enddate = Tw.Service.Common.Utility.Date.ToInt(DateTime.Today);
            int startdate = Tw.Service.Common.Utility.Date.CalcStartDate(period, enddate);

            var mmv = new MemoryMapView()
            {
                Caller = "filter",
                Action = "candlechart",
                StartDate = startdate,
                EndDate = enddate,
                Interval = o.Interval,
                Company = company
            };

            DisplayChart(Utility.MemoryMap.WriteObject(mmv));
        }

        static void DisplayBackTestChart(string symbol)
        {
            var o = (BackTestView)reportcache["backtest"];
            var result = o.Result.FirstOrDefault(x => x.Company.Symbol.ToLower() == symbol);

            if (result == null)
            {
                System.Console.WriteLine("missing symbol...");
                return;
            }

            int buffer =Tw.Service.Common.Utility.Helper.LargestBacktestParam(o.BackTest);
            var periodstart = Tw.Service.Common.Utility.Date.DateAdd(o.BackTest.PeriodStart, -buffer);

            var mmv = new MemoryMapView()
            {
                Caller = "backtest",
                Action = "candlechart",
                StartDate = periodstart,
                EndDate = o.BackTest.PeriodEnd,
                Interval = o.BackTest.PeriodInterval,
                Company = result.Company,
                Trade = result.Trade
            };

            DisplayChart(Utility.MemoryMap.WriteObject(mmv));
        }

        static void DisplayChart(string mmfile)
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("Tw.Ui.Console", "Tw.Ui.Chart");
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = mmfile;
            p.Start();
        }

        #endregion

        #endregion

    }
}
