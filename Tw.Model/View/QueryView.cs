using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.Entity;
using Tw.Model.View;

namespace Tw.Model.View
{
    public class QueryView
    {
        public HashSet<string> Timings;

        public List<CompanyView> CompanyView = new List<CompanyView>();

        public List<Company> Company = new List<Company>();

        public List<Tw.Model.Entity.Environment> Environment = new List<Tw.Model.Entity.Environment>();

        public List<Expression> Expression = new List<Expression>();

        public List<Price> Price = new List<Price>();

        public List<Filter> Filter = new List<Filter>();

        public List<BackTest> BackTest = new List<BackTest>();

        public List<Query> Query = new List<Query>();

    }
}
