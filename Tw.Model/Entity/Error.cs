using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    public class Error
    {
        public Error(String source, String description)
        {
            this.Source = source;
            this.Description = description;
        }
        public String Source { get; set; }
        public String Description { get; set; }
    }
}
