using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    public class Price
    {
        private string _id = string.Empty;
        private string _exchange = string.Empty;
        private string _symbol = string.Empty;
        private string _interval = string.Empty;
        private int _date;
        private Decimal _open;
        private Decimal _close;
        private Decimal _high;
        private Decimal _low;
        private int _volume;

        public String Id { get { return _id; } set { _id = value; } }
        public String Exchange { get { return _exchange; } set { _exchange = value; } }
        public String Symbol { get { return _symbol; } set { _symbol = value; } }
        public String Interval { get { return _interval; } set { _interval = value; } }
        public int Date { get { return _date; } set { _date = value; } }
        public Decimal Open { get { return _open; } set { _open = value; } }
        public Decimal Close { get { return _close; } set { _close = value; } }
        public Decimal High { get { return _high; } set { _high = value; } }
        public Decimal Low { get { return _low; } set { _low = value; } }
        public int Volume { get { return _volume; } set { _volume = value; } }
    }
}
