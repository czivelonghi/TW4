using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//http://json2csharp.com/#
namespace YahooData
{
    public class Option
    {
        public string symbol { get; set; }
        public string type { get; set; }
        public string strikePrice { get; set; }
        public string lastPrice { get; set; }
        public string change { get; set; }
        public string changeDir { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }
        public string vol { get; set; }
        public string openInt { get; set; }
    }

    public class OptionsChain
    {
        public string expiration { get; set; }
        public string symbol { get; set; }
        public List<Option> option { get; set; }
    }

    public class Results
    {
        public OptionsChain optionsChain { get; set; }
    }

    public class Query
    {
        public int count { get; set; }
        public string created { get; set; }
        public string lang { get; set; }
        public Results results { get; set; }
    }

    public class RootObject
    {
        public Query query { get; set; }
    }
    
}
