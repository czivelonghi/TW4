using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Entity
{
    [Serializable()]
    public class Trade
    {
        private string _id;
        private string _parentid;
        private string _symbol;
        private string _study;//in/out sample (BackTest), paper(forwardtest),live(setup or real trading)
        private string _description;
        private string _tradetype; //buy/sell
        private string _positiontype; //long/short
        private int _shares;
        private decimal _price;
        private decimal _commission;
        private int _date=0;
        private int _time = 0;
        private string _expression;
        private decimal _balance;
        private decimal _total;

        public string ID { get { return _id; } set { _id = value; } }
        public string ParentID { get { return _parentid; } set { _parentid = value; } }
        public string Symbol { get { return _symbol; } set { _symbol = value; } }
        public string Study { get { return _study; } set { _study = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string TradeType { get { return _tradetype; } set { _tradetype = value; } }
        public string PositionType { get { return _positiontype; } set { _positiontype = value; } }
        public int Shares { get { return _shares; } set { _shares = value; } }
        public decimal Price { get { return _price; } set { _price = value; } }
        public decimal Commission { get { return _commission; } set { _commission = value; } }
        public int Date { get { return _date; } set { _date = value; } }
        public int Time { get { return _time; } set { _time = value; } }
        public string Expression { get { return _expression; } set { _expression = value; } }
        public decimal Balance { get { return _balance; } set { _balance = value; } }
        public decimal Total { get { return _total; } set { _total = value; } }

        public Trade() { }

    }

}
