module Sqlite

open Util.Types
open System.Data.Linq
open System.Data.Linq.Mapping
open System.Data.SQLite
open System.Data.Common

open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.Query
open System.IO
open System

//http://fssnip.net/94
[<Table(Name="Symbol")>]
type Symbol (Exchange:string, Symbol:string, Name: string)=
     let mutable m_Exchange = Exchange
     let mutable m_Symbol = Symbol
     let mutable m_Company = Name
     
     new() = Symbol(null, null, null)
 
     [<Column(Name="Exchange")>] 
     member this.Exchange
        with get() = m_Exchange
         and set(value) = m_Exchange <- value

     [<Column(Name="Symbol")>]
     member this.Symbols
        with get() = m_Symbol
         and set(value) = m_Symbol <- value

     [<Column(Name="Company")>]
     member this.Company
        with get() = m_Company
         and set(value) = m_Company <- value

[<Table(Name="Price")>]
type Price (Symbol:string, Date:int, Open:float, High:float, Low:float, Close:float, Volume:int)=
     let mutable m_Symbol = Symbol
     let mutable m_Date = Date
     let mutable m_Open = Open
     let mutable m_High = High
     let mutable m_Low = Low
     let mutable m_Close = Close
     let mutable m_Volume = Volume
 
     new() = Price(null, 0, 0.0, 0.0, 0.0, 0.0, 0)
 
     [<Column(Name="Symbol")>]
     member this.Symbol
        with get() = m_Symbol
         and set(value) = m_Symbol <- value
 
     [<Column(Name="PriceDate")>]
     member this.Date
         with get() = m_Date
         and set(value) = m_Date <- value

     [<Column(Name="OpenPrice")>]
     member this.Open
        with get() = m_Open
         and set(value) = m_Open <- value

     [<Column(Name="HighPrice")>]
     member this.High
        with get() = m_High
         and set(value) = m_High <- value

     [<Column(Name="LowPrice")>]
     member this.Low
        with get() = m_Low
         and set(value) = m_Low <- value

     [<Column(Name="ClosePrice")>]
     member this.Close
        with get() = m_Close
         and set(value) = m_Close <- value

     [<Column(Name="Volume")>]
     member this.Volume
        with get() = m_Volume
         and set(value) = m_Volume <- value

//let DBContext = 
//    let p = System.IO.Directory.GetCurrentDirectory().ToString()
//    let path = p.Replace(@"\bin\Debug",@"\Data\Sqlite\StockData.db")
//    let connString = System.String.Format("Data Source={0};UTF8Encoding=True;Version=3", path)
//    let conn = new  System.Data.SQLite.SQLiteConnection(connString)
//    let db = new DataContext(conn)
//    let data = db.GetTable<Symbols>()
//    let d = seq {for r in data do
//                    if r.Exchange = "NYSE" then
//                        yield new Symbols(r.Exchange,r.Symbols,r.Company)}
//    d |> Seq.map(fun x -> printf "%s" x.Symbols) |> ignore
    
//let Prices (exchange: string, period: int, symbol: string) =
//    let db = DBContext
//    let data = db.GetTable<Price>()
//    let startDate = 20110101
//    seq {for p in data do
//         if p.Date > startDate then
//            yield p}
//
//let Symbols (exchange: string) =
//    let db = DBContext
//    let data = db.GetTable<Symbols>()
//    seq {for p in data do
//         if p.Exchange = exchange then
//            yield p}
                
//let data =  seq [new Stock(Symbol="A");new Stock(Symbol="B")]     

type Data = 
    [<DefaultValue>]
    static val mutable private m_path : string
    static member private SetPath =
        let p = System.IO.Directory.GetCurrentDirectory().ToString()
        let path = p.Replace(@"\bin\Debug",@"\Data\Sqlite\StockData.db").Replace(@"\bin\Release",@"\Data\Sqlite\StockData.db")
        Data.m_path <- System.String.Format("Data Source={0};UTF8Encoding=True;Version=3", path)

    static member private ExecuteReader(sql:string) = 
        let cmd = new SQLiteCommand(sql, Data.m_cn)
        cmd.ExecuteReader()

    [<DefaultValue>]
    static val mutable private m_cn : SQLiteConnection

    [<DefaultValue>]
    static val mutable private m_prices : seq<Price>
    static member Prices with get() = Data.m_prices

    [<DefaultValue>]
    static val mutable private m_symbols : seq<Symbol>
    static member Symbols with get() = Data.m_symbols  

    static member IsConnected = 
        if Data.m_cn = null then false
        else
            let s = Data.m_cn.State
            match s.ToString() with
                | "Open" -> true
                | _ -> false

    static member Connect = 
        if Data.IsConnected=false then 
            Data.SetPath
            Data.m_cn <- new SQLiteConnection(Data.m_path)
            Data.m_cn.Open() |> ignore
        
    static member Disconnect = 
        Data.m_cn.Close()

    static member ExecuteScalar(sql: string) = 
        Data.Connect
        use cmd =  Data.m_cn.CreateCommand()
        cmd.CommandText <- sql
        cmd.ExecuteScalar()

    [<DefaultValue>]
    static val mutable private m_maxdate : DateTime
    static member private InitMaxDate =
        let d = Data.ExecuteScalar("select value from environment where name='last_price_date' and active=1")
        let dt= d.ToString()
        Data.m_maxdate <- DateTime.ParseExact(dt,"yyyyMMdd",Globalization.CultureInfo.InvariantCulture)

    static member Initialize = 
        Data.Connect
        Data.InitMaxDate

    static member LoadSymbols(exchange : string) = 
        Data.m_symbols <- seq{use rdr = Data.ExecuteReader("select * from Symbols s where s.Exchange='" + exchange.ToUpper() + "'")
                              while rdr.Read() do
                                yield new Symbol(rdr.GetString 0, rdr.GetString 1, rdr.GetString 2)}

    static member LoadPrices(exchange: string, symbol: string, periods: float) =
        let startDate = Data.m_maxdate.AddDays -periods
        let sql = "select * from " + exchange + "Daily s where s.Symbol='" + symbol.ToUpper() +
                  "' and " + "PriceDate >= " + Common.Format.DateToNumber startDate + " order by pricedate desc"
        Data.m_prices <- seq{use cmd = new SQLiteCommand(sql, Data.m_cn)
                             use rdr = cmd.ExecuteReader()
                             while rdr.Read() do
                                yield new Price(rdr.GetString 0, rdr.GetInt32 1, rdr.GetDouble 2, rdr.GetDouble 3, rdr.GetDouble 4,rdr.GetDouble 5, rdr.GetInt32 6)}
        //Data.m_prices |> Seq.sortBy(fun x-> x.Date)
