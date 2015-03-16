module MongoData

open Microsoft.FSharp.Linq
open Util.Types
open System
open MongoDB.Driver.Builders
open MongoDB.Driver
open MongoDB.Bson
open MongoDB
open System.Linq

//lex and parse
//http://blog.thycoticsolutions.com/2010/09/10/lexing-and-parsing-with-f-part-i/
module Types =
    type Stock =
        val mutable _id:  ObjectId
        val mutable Symbol : string
        val mutable Date : int
        val mutable Open : double
        val mutable High : double
        val mutable Low : double
        val mutable Close : double
        val mutable Volume: int
        new() = {_id=new ObjectId();Symbol="";Date=0;Open=0.0;High=0.0;Low=0.0;Close=0.0;Volume=0 }

    type Symbol =
        val mutable _id:  ObjectId
        val mutable Symbol : string
        val mutable Description : string
        new() = {_id=new ObjectId();Symbol="";Description=""}

    type Definitions = 
        val mutable _id: ObjectId
        val mutable Name: string
        val mutable Expression: string
        new() ={_id=new ObjectId();Name="";Expression=""}


type Query =
    static member private getWeekendDays(periods: int) = 
        let weekends = float periods * 0.142857142857143 
        let addperiods = Convert.ToInt32(Math.Ceiling(weekends)) * 2
        addperiods

    static member private addPeriods(date:int, periods:int) =
        let newperiods = Query.getWeekendDays(periods) + periods
        let tempdate = DateTime.ParseExact(date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture)
        let newDate = tempdate.AddDays(float newperiods)
        let tempdate2 = newDate.Year.ToString("0000") + newDate.Month.ToString("00") + newDate.Day.ToString("00")
        Convert.ToInt32(tempdate2)
    
    static member private createSort (sort:string) =
        let vals = sort.Split(' ')
        if vals.[0]="ASC" then  
            SortBy.Ascending(vals.[1])
        else
            SortBy.Descending(vals.[1])
        
//    static member private Sort (col:MongoCollection, qry:Query, ?sort:string) =
//        match sort with
//        | None -> col
//        | Some sort -> createSort(sort)

    static member private getStockCollection(db:string, col:string) = 
        let ms = MongoServer.Create("mongodb://localhost")
        let db = ms.GetDatabase(db)
        db.GetCollection<Types.Stock>(col)
    
    static member private getSymbolCollection(db:string, col:string) = 
        let ms = MongoServer.Create("mongodb://localhost")
        let db = ms.GetDatabase(db)
        db.GetCollection<Types.Symbol>(col)
    
    static member public Symbols(exch:string, ?sort:string) =
        let col = Query.getSymbolCollection("symbols",exch)
        match sort with
            | None -> col.FindAll()
            | Some sort -> let sortBy = SortBy.Ascending("")
                           col.FindAll().SetSortOrder(sortBy)

    static member public ByDate(exch:string, symbol:string, interval:string, date:int) =
        let col = Query.getStockCollection(exch,interval)
        let qry = Query.And(Query.EQ("Symbol", BsonString symbol),
                            Query.EQ("Date", BsonInt32 date))
        col.Find(qry)//.SetSortOrder(

    static member public ByDateRange(exch:string, interval:string, startdate:int, enddate:int) =
        let col = Query.getStockCollection(exch,interval)
        let qry = Query.And(Query.GTE("Date", BsonInt32 startdate),
                             Query.LTE("Date",BsonInt32 enddate))
        col.Find(qry)

    static member public ByDateRange(exch:string, symbol:string, interval:string, startdate:int, enddate:int) =
        let col = Query.getStockCollection(exch,interval)
        let qry = Query.And(Query.EQ("Symbol", BsonString symbol),
                            Query.GTE("Date", BsonInt32 startdate),
                            Query.LTE("Date",BsonInt32 enddate))
        col.Find(qry)

    static member public ByPeriodsBack(exch:string, interval:string, enddate:int, periods:int) =
        let col = Query.getStockCollection(exch,interval)
        let startdate = Query.addPeriods(enddate,-periods)
        let qry = Query.And(Query.GTE("Date", BsonInt32 startdate),
                             Query.LTE("Date",BsonInt32 enddate))
        col.Find(qry)//.SetSortOrder(SortBy.Ascending("Symbol"))

    static member public ByPeriodsBack(exch:string, symbol: string, interval:string, enddate:int, periods:int) =
        let col = Query.getStockCollection(exch,interval)
        let startdate = Query.addPeriods(enddate,-periods)
        let qry = Query.And(Query.EQ("Symbol", BsonString symbol),
                            Query.GTE("Date", BsonInt32 startdate),
                            Query.LTE("Date",BsonInt32 enddate))
        col.Find(qry)

    static member public ByPeriodsForward(exch:string, interval:string, startdate:int, periods: int) =
        let col = Query.getStockCollection(exch,interval)
        let enddate = Query.addPeriods(startdate,periods)
        let qry = Query.And(Query.LTE("Date", BsonInt32 enddate),
                            Query.GTE("Date",BsonInt32 startdate))
        col.Find(qry)

    static member  public ByPeriodsForward(exch:string, symbol:string, interval:string, startdate:int, periods: int) =
        let col = Query.getStockCollection(exch,interval)
        let enddate = Query.addPeriods(startdate,periods)
        let qry = Query.And(Query.EQ("Symbol", BsonString symbol),
                            Query.LTE("Date", BsonInt32 enddate),
                            Query.GTE("Date",BsonInt32 startdate))
        col.Find(qry)


//http://www.mongodb.org/display/DOCS/CSharp+Driver+Tutorial#CSharpDriverTutorial-Updatemethod
    
//    let test (exch:string) = 
//        let ms = MongoServer.Create("mongodb://localhost")
//        let db = ms.GetDatabase("stocks")
//        let document = db.GetCollection<Stock>(exch)
//        let data = db.["movies"].AsQueryable() :> IQueryable<Stock>
//        query <@ seq { for i in data do
//                        yield Stock } @>

//let titles =
//  query <@ seq { for movie in movies do
//                   yield movie } @>

//    let query = new Document
//        "$or", new Document[]  new Document("Age", 21), new Document("Age", 35) } }

//
//let q3 =
//    let m = new Mongo()
//    let connect = m.Connect()
//    let db = m.GetDatabase("stocks")
//    let document = db.GetCollection<Stock>("nyseday")
//    query <@ seq { for i in document.Count do
//                       if i.Ticker = "A" then
//                            yield (i.ContactName, j.FirstName, j.LastName) } @>

// 
// q3 |> Seq.toList
//

        //let c = col :> MongoCollection
        //let nameValue = sort.ToString().Split(' ')//ASC Symbol
        
//        
//        if(nameValue.[0]="ASC") then
//            query.SortBy.Ascending(nameValue.[1])
//        else
//            qry SortBy.Descending(nameValue.[1])

// Query col where sort
//module Query2 =
//    let private getStockCollection(db:string, col:string) = 
//        let ms = MongoServer.Create("mongodb://localhost")
//        let db = ms.GetDatabase(db)
//        db.GetCollection<Stock>(col)
//
//    let QueryDB (db : string, col : string, where:string, sort: string) =
//        let c = getStockCollection(db,col)
//        c Where 
//    
//    let Where (col : MongoCollection, where: string) =
//        let qry = Query.And(Query.GTE("Date", BsonInt32 startdate),
//                        Query.LTE("Date",BsonInt32 enddate))
//
//    let QueryElem (qry: Query, item: string)
//        let qe = Query.GTE(
//        Query.And(
//
//    let Sort (col : MongoCollection, sort: string) =
//        let vals = sort.ToString().Split(' ')
//        match vals.[0] with
//        | "ASC" -> SortBy.Ascending(vals.[1])
//        | "DESC" -> SortBy.Descending(vals.[1])
//        | _ -> failwith("invalid sort")