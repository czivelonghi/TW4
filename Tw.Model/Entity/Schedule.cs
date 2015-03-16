using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    public class Schedule
    {
        int _active = 1;
        string _id;
        string _name = String.Empty;
        string _setup = String.Empty;
        string _interval = "d";//d,w,,m,hh,mm,ss

        public Schedule(string name)
        {
            _name = name;
            _id = Guid.NewGuid().ToString().Replace("-", "");
        }

        public string Id { get { return _id; } set { _id = value; } }
        [Binding()]
        public String Name { get { return _name; } set { _name = value; } }
        [Binding()]
        public int Active { get { return _active; } set { _active = value; } }
        [Binding()]
        public String Setup { get { return _setup; } set { _setup = value; } }
        [Binding()]
        public String Interval { get { return _interval; } set { _interval = value; } }
    }
}
