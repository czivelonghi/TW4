using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Db4objects;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Tw.Service.Common
{
    public class Utility
    {
        public class Attribute
        {
            public static bool IsBinding(PropertyInfo p )
            {
                bool binding = false;
                
                object[] attributes = p.GetCustomAttributes(true);

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[0].GetType() == typeof(Tw.Model.Entity.BindingAttribute))
                    {
                        binding = true;
                        break;
                    }

                }
                return binding;
            }

            public static object Value(object obj,string col)
            {
                var prop = obj.GetType().GetProperties();
                var column = prop.FirstOrDefault(x => x.Name.ToLower() == col.ToLower());

                object keyvalue = null;

                if (column != null)
                    keyvalue = column.GetValue(obj, null);

                return keyvalue;
            }

        }

        public class Date
        {

            public static string ToString(DateTime date)
            {
                return date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00");
            }

            public static int ToInt(DateTime date)
            {
                return Int32.Parse((date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00")));
            }

            public static DateTime ToDate(string date)
            {
                return DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }

            public static DateTime ToDate(int date)
            {
                return DateTime.ParseExact(date.ToString(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }

            public static int DateAdd(int date, int days)
            {
                var d = ToDate(date).AddDays(days);
                return ToInt(d);
            }

            private static int CalcWeekDays(int period)
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

            private static int CalcStartDate(int period)
            {
                DateTime date = DateTime.Today;

                var startdate = date.AddDays(-CalcWeekDays(period));

                return int.Parse(startdate.ToString("yyyyMMdd"));
            }

            public static int CalcStartDate(int period, int enddate)
            {

                DateTime date = ToDate(enddate.ToString());
                var newdate = date.AddDays(-CalcWeekDays(period));
                return int.Parse(newdate.ToString("yyyyMMdd"));
            }


            public static int PeriodsBetween(int startdate, int enddate)
            {
                DateTime edate = ToDate(enddate.ToString());
                DateTime sdate = ToDate(startdate.ToString());
                return Int32.Parse((edate - sdate).TotalDays.ToString());
            }

        }

        public class IO
        {
            private static string _directory = @"\Data\";
            public enum eFetchType
            {
                WebService,
                Delimited
            }

            public static string MapPath(string path)
            {
                string dir = Directory.GetCurrentDirectory().ToString();
                dir = dir.Replace("\\bin\\Debug", path);//HttpContext.Current.Server.MapPath(@"~\" + path);
                return dir;
            }

            public static ArrayList FetchData(eFetchType fetchType, string file, string delimiter)
            {
                ArrayList al = new ArrayList();

                if (fetchType == eFetchType.Delimited)
                {
                    string path = _directory + file;
                    al = fetchCsvData(MapPath(path), delimiter);
                }

                return al;
            }

            public static long FileSize(string filePath)
            {
                long size = 0;

                FileInfo f = new FileInfo(IO.MapPath(filePath));

                if (f.Exists)
                    size = f.Length;

                return size;
            }

            public static void DeleteFile(string file)
            {
                FileInfo f = new FileInfo(MapPath(_directory) + file);

                if (!f.Exists)
                    throw new Exception("File does not exist");

                f.Delete();

            }

            public static ArrayList FileList()
            {
                ArrayList al = new ArrayList();

                DirectoryInfo d = new DirectoryInfo(MapPath(_directory + "Files"));
                FileInfo[] files = d.GetFiles();

                foreach (FileInfo file in files)
                {
                    al.Add(file.Name);
                }

                return al;
            }

            private static ArrayList fetchCsvData(string path, string delimiter)
            {
                ArrayList al = new ArrayList();

                try
                {
                    StreamReader r = System.IO.File.OpenText(path);

                    while (!r.EndOfStream)
                    {
                        string[] values = r.ReadLine().Split(delimiter.ToCharArray());
                        al.Add(values);
                    }

                    r.Close();
                }
                catch (Exception e)
                {

                }

                return al;
            }
        }

        public class FTP
        {
            public static StreamReader WebResponse(string url)
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();

                return new StreamReader(response.GetResponseStream());
            }

            public static StreamReader WebResponse(string url, string username, string password)
            {
                StreamReader reader = null;

                try
                {
                    WebRequest request = WebRequest.Create(url);//"ftp://ftp.test.net/test.txt"
                    request.Credentials = Credentials(username, password);
                    WebResponse response = request.GetResponse();

                    reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
                }
                catch (Exception e)
                {
                    //log error
                }

                return reader;
            }

            private static NetworkCredential Credentials(string username, string password)
            {
                NetworkCredential credentials = new NetworkCredential();
                credentials.UserName = username;//@"domain\username";
                credentials.Password = password;

                return credentials;
            }
        }

        public class Conversion
        {
            public static double ConvertSize(double bytes, string type)
            {
                try
                {
                    const int CONVERSION_VALUE = 1024;
                    switch (type)
                    {
                        case "BY": return bytes;
                        case "KB": return (bytes / CONVERSION_VALUE);
                        case "MB": return (bytes / CalculateSquare(CONVERSION_VALUE));
                        case "GB": return (bytes / CalculateCube(CONVERSION_VALUE));
                        default: return bytes;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }

            private static double CalculateSquare(Int32 number)
            {
                return Math.Pow(number, 2);
            }

            private static double CalculateCube(Int32 number)
            {
                return Math.Pow(number, 3);
            }

            public static System.Data.DataSet ToDataSet<T>(List<T> list)
            {

                Type type = list.GetType();
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                
                //create datatable
                var dt = new System.Data.DataTable(type.Name.ToString());

                //delete --

                //push changes DT
                foreach(var item in list)
                {
                    //id exists then update else create
                    
                }

                System.Data.DataSet ds = new System.Data.DataSet("");
                ds.Tables.Add(dt);

                return ds;
            }

            public static IEnumerable<T> ToSequence<T>(List<T> objs)
            {
                foreach (var x in objs)
                {
                    yield return x;
                }
            }
        }

        public class Dynamic
        {

            public static Assembly CreateAssembly(string assemblyName)
            {
                return Assembly.LoadFrom(assemblyName);
            }

            public static Type InstanceType(string assemblyName, string className)
            {
                var o = CreateInstance(assemblyName, className);
                return o.GetType();
            }

            public static T GetInstance<T>(string type)
            {
                return (T)Activator.CreateInstance(Type.GetType(type));
            }

            public static object CreateInstance(string assemblyName, string className)
            {

                object result = null;

                Assembly assembly = Assembly.LoadFrom(assemblyName);

                foreach (Type type in assembly.GetTypes())
                {

                    if (type.IsClass == true)
                    {

                        if (type.Name == className)
                        {
                            result = Activator.CreateInstance(type);
                            break;
                        }
                    }

                }

                return result;
            }

            public static object InvokeProcess(string assemblyName, string interfaceName, string process, object[] arguments)
            {
                object result = null;

                Assembly assembly = Assembly.LoadFrom(assemblyName);

                foreach (Type type in assembly.GetTypes())
                {

                    if (type.IsClass == true)
                    {

                        if (type.GetInterface(interfaceName) != null)
                        {
                            result = type.InvokeMember(process,
                                                        BindingFlags.Default | BindingFlags.InvokeMethod,
                                                        null,
                                                        assembly,
                                                        arguments);
                            break;
                        }
                    }

                }

                return result;
            }

        }

        public class HTTP
        {
            public static void OpenBrowser(string url)
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        public class Timing
        {
            public static class ProcessorTime
            {
                static TimeSpan begin;
                static TimeSpan end;
                public static void Start()
                {
                    begin = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
                }

                public static void Stop()
                {

                    end = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
                }

                public static string Results()
                {
                    return "Measured time: " + (end - begin).TotalSeconds + " sec.";
                }
            }

            public static class StopWatch
            {
                static DateTime begin;
                static DateTime end;
                public static void Start()
                {
                    begin = DateTime.UtcNow;
                }

                public static void Stop()
                {
                    end =  DateTime.UtcNow;
                }

                public static string Results()
                {
                    return "Measured time: " + (end - begin).TotalSeconds + " sec.";
                }
            }
        }

        public class File
        {
            public static string TempFile(string ext="txt")
            {
                var file = System.IO.Path.GetRandomFileName();
                file = file.Substring(0, file.Length - 4) + "." + ext;
                var temp = System.IO.Path.GetTempPath() + "\\" + file;
                return temp;
            }

            public static void Export(string contents,string ext = "txt")
            {
                var path = TempFile(ext);
                System.IO.File.WriteAllText(path, contents);
                System.Diagnostics.Process.Start(path);
            }

        }

        public class Misc
        {
            public static string Clean(string text)
            {
                return text.Trim();
            }

            public static int NVL(string value, int defValue)
            {
                if (value != String.Empty)
                    return Int32.Parse(value.ToString());
                else
                    return defValue;
            }

        }

        public class MemoryMap
        {
            public static string WriteObject(object obj)
            {
                var mmfile = File.TempFile("MMF");
                WriteObjectToMMF(mmfile, obj);
                return mmfile;
            }

            public static object ReadObject(string mmfile)
            {
                return ReadObjectFromMMF(mmfile);
            }

            //public static void MapMemory()
            //{
            //    using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 10000))
            //    {

            //        bool mutexCreated;
            //        Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
            //        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            //        {
            //            BinaryWriter writer = new BinaryWriter(stream);
            //            writer.Write(1);
            //        }
            //        mutex.ReleaseMutex();

            //        //Console.WriteLine("Start Process B and press ENTER to continue.");
            //        //Console.ReadLine();

            //        //Console.WriteLine("Start Process C and press ENTER to continue.");
            //        //Console.ReadLine();

            //        //mutex.WaitOne();
            //        //using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            //        //{
            //        //    BinaryReader reader = new BinaryReader(stream);
            //        //    Console.WriteLine("Process A says: {0}", reader.ReadBoolean());
            //        //    Console.WriteLine("Process B says: {0}", reader.ReadBoolean());
            //        //    Console.WriteLine("Process C says: {0}", reader.ReadBoolean());
            //        //}
            //        //mutex.ReleaseMutex();
            //    }

            //}


            #region Generic MMF read/write object functions

            static void WriteObjectToMMF(string mmfFile, object objectData)
            {
                byte[] buffer = ObjectToByteArray(objectData);

                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(mmfFile, FileMode.Create, null, buffer.Length))
                {
                    using (MemoryMappedViewAccessor mmfWriter = mmf.CreateViewAccessor(0, buffer.Length))
                    {
                        mmfWriter.WriteArray<byte>(0, buffer, 0, buffer.Length);
                    }
                }
            }

            static object ReadObjectFromMMF(string mmfFile)
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(mmfFile, FileMode.Open))
                {
                    using (MemoryMappedViewAccessor mmfReader = mmf.CreateViewAccessor())
                    {
                        byte[] buffer = new byte[mmfReader.Capacity];
                        mmfReader.ReadArray<byte>(0, buffer, 0, buffer.Length);
                        return ByteArrayToObject(buffer);
                    }
                }
            }

            #endregion

            #region Object/Binary serialization

            static object ByteArrayToObject(byte[] buffer)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();    // Create new BinaryFormatter
                MemoryStream memoryStream = new MemoryStream(buffer);       // Convert byte array to memory stream, set position to start
                return binaryFormatter.Deserialize(memoryStream);           // Deserializes memory stream into an object and return
            }

            static byte[] ObjectToByteArray(object inputObject)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();    // Create new BinaryFormatter
                MemoryStream memoryStream = new MemoryStream();             // Create target memory stream
                binaryFormatter.Serialize(memoryStream, inputObject);       // Convert object to memory stream
                return memoryStream.ToArray();                              // Return memory stream as byte array
            }

            #endregion

        }

        public class Configuration
        {
            static string ConfigValue(string key)
            {
                ConfigurationFileMap fileMap = new ConfigurationFileMap(@"C:\Development\ClientServer\TW4\Tw.Ui.Console\app.config"); //Path to your config file
                var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
                string value = configuration.AppSettings.Settings[key].Value;
                return value;
            }
            // AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE","")
            public static bool IsTest()
            {
                //AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", @"C:\Development\ClientServer\TW4\Tw.Service\bin\Debug\app.config");
                //var con = ConfigurationManager.AppSettings["connection"];
                //return (ConfigValue("connection") == "testserver");
                return true;
            }

            public static string Connection(string key)
            {
                //AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", @"C:\Development\ClientServer\TW4\Tw.Service\bin\Debug\app.config");
                //var con = ConfigurationManager.AppSettings["connection"];
                //var servers = ConfigurationManager.GetSection(con) as System.Collections.Specialized.NameValueCollection;
                //if (servers != null)
                //    return servers.GetValues(key).FirstOrDefault();
                //else
                //    return "";

                var path = @"C:\data\test\" + key + ".yap";
                return path;
            }
        }
     
        public class Helper
        {
            public static string Reduce(string expression)
            {
                var r = new Tw.Service.Repository.ExpressionRepository();
                r.Load();
                IEnumerable<Tw.Model.Entity.Expression> expressions = Common.Utility.Conversion.ToSequence<Tw.Model.Entity.Expression>(r.Expression);
                return Reducer.ReduceFromSeq(expression, expressions);
            }

            public static int LargestParam(string expression)
            {
                string exp = Reduce(expression);
                var m = System.Text.RegularExpressions.Regex.Split(exp, @"\D+");
                return m.Select(x => (x == string.Empty) ? 0 : Int32.Parse(x)).Max();
            }

            public static int LargestBacktestParam(Tw.Model.Entity.BackTest b)
            {
                var n = 0;

                foreach (var e in b.Entry)
                {
                    var n2 = LargestParam(e.Expression);
                    n = (n2 > n) ? n2 : n;
                    foreach (var x in e.Exit)
                    {
                        var n3 = LargestParam(e.Expression);
                        n = (n3 > n) ? n3 : n;
                    }
                }

                return n;

            }

            public static int LargestMetricParam(Tw.Model.Entity.Metric m)
            {
                var n = 0;

                foreach (var e in m.Study)
                {
                    var n2 = LargestParam(e.Expression);
                    n = (n2 > n) ? n2 : n;
                }

                return n;

            }
        }
    }
}
