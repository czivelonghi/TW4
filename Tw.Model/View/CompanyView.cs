using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.Entity;

namespace Tw.Model.View
{
    public class CompanyView
    {
        public Company Company;
        public List<Price> Price { get; set; }
    }
}
