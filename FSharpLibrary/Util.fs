module Util

open System
open System.Xml
open System.Net
open Microsoft.FSharp.Control.WebExtensions
open System.IO
open System.Diagnostics 

module PerformanceTesting =
    let Time func =
        let stopwatch = new Stopwatch()
        stopwatch.Start()
        func()
        stopwatch.Stop()
        stopwatch.Elapsed.TotalMilliseconds

    let GetAverageTime timesToRun func = 
        Seq.initInfinite (fun _ -> (Time func))
        |> Seq.take timesToRun
        |> Seq.average

    let TimeOperation timesToRun =
        GC.Collect()
        GetAverageTime timesToRun

    let TimeOperations funcsWithName =
        let randomizer = new Random(int DateTime.Now.Ticks)
        funcsWithName
        |> Seq.sortBy (fun _ -> randomizer.Next())
        |> Seq.map (fun (name, func) -> name, (TimeOperation 100000 func))

    let TimeOperationsAFewTimes funcsWithName =
        Seq.initInfinite (fun _ -> (TimeOperations funcsWithName))
        |> Seq.take 50
        |> Seq.concat
        |> Seq.groupBy fst
        |> Seq.map (fun (name, individualResults) -> name, (individualResults |> Seq.map snd |> Seq.average))

module Web =
    //http://jarloo.com/tutorials/get-yahoo-finance-api-data-via-yql/ or //http://jarloo.com/code/api-code/get-historical-stock-data/
    let FetchUrl(name, url:string) =
        let uri = new System.Uri(url)
        let webClient = new System.Net.WebClient()
        webClient.DownloadString(uri)

module Conversion =
    let Integer (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then intvalue
       else 0

    let Float (str: string) =
       let mutable floatvalue = 0.0
       if System.Double.TryParse(str, &floatvalue) then floatvalue
       else 0.0

    //2010-12-31
    let ShortDate (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str.Replace("-",""), &intvalue) then intvalue
       else 0

    //10:27 AM
    let Time (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str.Replace("-",""), &intvalue) then intvalue
       else 0

module Types =

//    type Stock = Class
//        val mutable Symbol : string
//        val mutable Date : int
//        val mutable Open : double
//        val mutable High : double
//        val mutable Low : double
//        val mutable Close : double
//        val mutable Volume: int
//        new() = {Symbol="";Date=0;Open=0.0;High=0.0;Low=0.0;Close=0.0;Volume=0}
//    end

    type RealTimeQuote =
        {
        symbol: string;
        lastTrade: string;
        bid: float;
        ask: float;
        volume: int;
        }

    type HistoryQuote2 = class
        val Period : int
        val Symbol : string
        val High : float
        val Low : float
        val Open : float
        val Close : float
        val Volume : int
    end

    type HistoryQuote =
        {
        Symbol: string;
        Period: int;
        Open: float;
        High: float;
        Low: float;
        Close: float;
        Volume: int;
        }

    type OptionQuote =
        {
        symbol: string;
        optionType: string;
        strikePrice: float;
        openInterest: float;
        bid: float;
        ask: float;
        volume: int;
        }

module IO = 
    let private recordToCSV(record: obj) =
        match record with
        | :? Types.HistoryQuote as h -> h.Symbol + "," + string h.Period + "," +  string h.Open  + "," + string h.High + "," + string h.Low + "," + string h.Close + "," + string h.Volume
        | :? Types.OptionQuote as o -> string o.optionType + "," + string o.symbol + "," +  string o.strikePrice + "," + string o.bid + "," +  string o.ask + "," +  string o.openInterest + "," + string o.volume
        | _ -> ""

    let private SeqToCSV (values: seq<'a>) = 
        (System.Text.StringBuilder(), values) ||>
        Seq.fold (fun sb value -> sb.AppendLine(recordToCSV(value)))
    
    let public DeleteFile(path: string) =
        if File.Exists(path) then
            File.Delete(path)

    let public AppendToFile (path:string, values: seq<'a>) =
        let sb = SeqToCSV values
        File.AppendAllText(path,sb.ToString())
        Seq.length values

module Xml = 
    //match xml element
    let (|Elem|_|) name (inp : System.Xml.XmlNode) =
        if inp.Name = name then Some(inp)
        else None

    //get xmlelement attribute
    let (|Attributes|) (inp : System.Xml.XmlNode) = inp.Attributes

    //get child nodes
    let (|ChildElement|) nodeName (inp : System.Xml.XmlNode) = inp.SelectSingleNode(nodeName)

    //match attr
    let (|Attr|) attrName (inp : System.Xml.XmlAttributeCollection) =
        match inp.GetNamedItem(attrName) with
        | null -> failwithf "Attribute %s not found" attrName
        | attr -> attr.Value

    //elem value
    let (|ElemValue|) (inp : System.Xml.XmlNode) = inp.Value

    //parent node
    let (|Parent|) (inp : System.Xml.XmlNode) =
        inp.ParentNode

    let ParseXml xpath xml =
        let doc = new System.Xml.XmlDocument() in doc.LoadXml(xml);
            doc.SelectNodes(xpath)
            |> Seq.cast<System.Xml.XmlElement>

    let (|ChildText|) elemName cast (inp : System.Xml.XmlNode) =
        match inp.SelectSingleNode(elemName) with
        | null -> failwithf "Child node %s not found" elemName
        | node -> cast node.InnerText

    