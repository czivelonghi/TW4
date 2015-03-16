using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.View
{
    public interface IView
    {
        List<Entity.Company> Company  { get; set; }
        List<Entity.Price> Price  { get; set; }
        List<Entity.Expression> Expression { get; set; }
        List<Entity.Error> Error { get; set; }
        List<string> Debug { get; set; }
        List<string> Timing { get; set; }
        bool Parallel { get; set; }
        int DebugMode { get; set; }
        string Report();
        string Export();
        string ExportType { get; set; }
    }
}
