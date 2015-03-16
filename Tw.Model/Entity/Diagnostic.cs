using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    //type: timing, object: price, event: query, description: execution time 100 ms per 100 rows
    //type: log, object: search, event: evaluate, description: exception bla bla
    class Diagnostic
    {
        private string _type = string.Empty;
        private string _name = string.Empty;
        private string _object = string.Empty;
        private string _event = string.Empty;
    }
}
