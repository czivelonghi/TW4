module RavenDB
//
//open Raven
//open Raven.Client
//open Raven.Client.Embedded
//open Raven.Database
//
//open System.Data.Linq
//open System.Data.Linq.Mapping
//open System.Data.Common
//
//open Microsoft.FSharp.Linq
//open Microsoft.FSharp.Linq.Query
//open System.IO
//open System
//
//type Price = {
//    mutable Symbol : string
//    mutable PriceDate      : int
//    mutable OpenPrice      : double
//    mutable HighPrice      : double
//    mutable LowPrice       : double
//    mutable ClosePrice     : double
//    mutable Volume         : int
//}
//
//[<Table(Name="Symbol")>]
//type Symbol (Exchange:string, Symbol:string, Name: string)=
//    let mutable m_Exchange = Exchange
//    let mutable m_Symbol = Symbol
//    let mutable m_Company = Name
//     
//    new() = Symbol(null, null, null)
// 
//    [<Column(Name="Exchange")>] 
//    member this.Exchange
//        with get() = m_Exchange
//            and set(value) = m_Exchange <- value
//
//    [<Column(Name="Symbol")>]
//    member this.Symbols
//        with get() = m_Symbol
//            and set(value) = m_Symbol <- value
//
//    [<Column(Name="Company")>]
//    member this.Company
//        with get() = m_Company
//            and set(value) = m_Company <- value
//
////http://daniellang.net/searching-on-string-properties-in-ravendb/
//type Data = 
//    [<DefaultValue>]
//    static val mutable private m_instance : EmbeddableDocumentStore
//    static member private Initialize2 = 
//        let docStore = (new EmbeddableDocumentStore(RunInMemory = true)).Initialize()
//        docStore
//        //Raven "myStore" docStore
//
//    static member Initialize = 
//        let p = System.IO.Directory.GetCurrentDirectory().ToString()
//        let path = p.Replace(@"\bin\Debug",@"\Data\RavenDB").Replace(@"\bin\Release",@"\Data\RavenDB")
//        //"path/to/database/directory"
//        Data.m_instance <- (new EmbeddableDocumentStore( DataDirectory = path ))
//        Data.m_instance.Initialize        
//
//let p = System.IO.Directory.GetCurrentDirectory().ToString()
//let path = p.Replace(@"\bin\Debug",@"\Data\RavenDB").Replace(@"\bin\Release",@"\Data\RavenDB")//"path/to/database/directory"
//let docStore = (new EmbeddableDocumentStore( DataDirectory = path ))
//docStore.Initialize() |> ignore
//                
//let queryPrice = 
//    raven { 
//       return! query (where <@ fun x -> x.Exchange = "Nyse" @>)
//    }
//
//let QueryPrice exchange = 
//    use session = docStore.OpenSession()
//    queryPrice |> run session
//
//let SavePrice price : Price = 
//    use session = docStore.OpenSession()
//    store price |> run session
//
//let SaveSymbol symbol : Symbol = 
//    use session = docStore.OpenSession()
//    store symbol |> run session