module YahooData

open System.Xml
open Util
open Util.Types
open Util.Xml
open Util.Conversion
open Util.Web

//parse xml values
let parseOption element symbol =
    match element with
    | Elem "option" (Attributes (Attr "type" t) &
                     ChildText "strikePrice" Float p &
                     ChildText "bid" Float b &
                     ChildText "ask" Float a &
                     ChildText "openInt" Float i &
                     ChildText "vol" Integer v
                     ) -> {symbol=symbol;optionType=t; strikePrice=p; bid=b; ask=a; openInterest=i; volume=v}
    | _ -> failwith "Unknown element"
            
let parseHistory element symbol =
    match element with
    | Elem "quote" (ChildText "Date" ShortDate p &
                    ChildText "Open" Float o &
                    ChildText "High" Float h &
                    ChildText "Low" Float l &
                    ChildText "Close" Float c &
                    ChildText "Volume" Integer v
                    ) -> {Symbol=symbol; Period=p; Open=o; High=h; Low=l; Close=c; Volume=v}        
    | _ -> failwith "Unknown element"

let parseQuote element symbol =
    match element with
    | Elem "quote" (ChildText "LastTradeTime" string p &
                    ChildText "AskRealtime" Float a &
                    ChildText "BidRealtime" Float b &
                    ChildText "Volume" Integer v
                    ) -> {symbol=symbol; lastTrade=p; bid=b; ask=a; volume=v}        
    | _ -> failwith "Unknown element"

          
//http://query.yahooapis.com/v1/public/yql?q=select * from yahoo.finance.options where symbol in ("ibm")&env=store://datatables.org/alltableswithkeys
//query yahoo:count="1" yahoo:created="2011-08-22T18:16:52Z" yahoo:lang="en-US"><results><optionsChain symbol="ibm"><option symbol="IBM110826C00135000" type="C"><strikePrice>135</strikePrice><lastPrice>26.40</lastPrice><change>0</change><changeDir/><bid>23.95</bid><ask>24.95</ask><vol>9</vol><openInt>9</openInt></option>
let FetchOptionQuote symbol =
    let uri = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.options%20where%20symbol%20in%20%28%22" + symbol + "%22%29&env=store://datatables.org/alltableswithkeys";
    let xml = FetchUrl("yahoo",uri)
    let elem  = ParseXml "//optionsChain/*" xml
    Seq.map(fun f -> parseOption f symbol) elem
    
//real time: http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20%28%22ibm%22%29&env=store://datatables.org/alltableswithkeys
let FetchRealtimeQuote symbol =
    let uri = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20%28%22" + symbol + "%22%29&env=store://datatables.org/alltableswithkeys";
    let xml = FetchUrl("yahoo",uri)
    let elem  = ParseXml "//results/*" xml
    Seq.map(fun f -> parseQuote f symbol) elem
    
//select * from csv where url='http://download.finance.yahoo.com/d/quotes.csv?s=YHOO,GOOG,AAPL&f=sl1d1t1c1ohgv&e=.csv' and columns='symbol,price,date,time,change,col1,high,low,col2'
//http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20%3D%20%22YHOO%22%20and%20startDate%20%3D%20%222009-09-11%22%20and%20endDate%20%3D%20%222010-03-10%22&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys
let FetchHistoricalQuote (symbol:string, startdate:string, enddate:string) =
    let sd = System.Convert.ToDateTime(startdate)
    let ed = System.Convert.ToDateTime(enddate)
    let newSD = sd.Year.ToString() + "-" + sd.Month.ToString("00") + "-" + sd.Day.ToString("00")
    let newED = ed.Year.ToString() + "-" + ed.Month.ToString("00") + "-" + ed.Day.ToString("00")
    
    let uri = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20%3D%20%22" + symbol + "%22%20and%20startDate%20%3D%20%22" + newSD + "%22%20and%20endDate%20%3D%20%22" + newED + "%22&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
    let xml = FetchUrl("yahoo",uri)
    let elem  = ParseXml "//results/*" xml
    Seq.map(fun f -> parseHistory f symbol) elem

//    let tempDate = new System.DateTime(sd.Year,sd.Month,sd.Day)
//    let ed = tempDate.AddDays(-periods)
    //let formatedSD = System.String.Format("{0:####-##-##}", startdate)
