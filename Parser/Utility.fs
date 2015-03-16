module public Utility

open System.Collections
open Tw.Model
open System.Threading.Tasks
open Microsoft.FSharp.Collections

module Convert = 
    let ToDate(yyymmdd : int) = 
        System.DateTime.ParseExact(string yyymmdd, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    
    let DateDiff(date1 : int ,date2 : int) = 
        let d1 = ToDate(date1)
        let d2 = ToDate(date2)
        d1.Subtract(d2).Days

    let ToGenericList(l : list<'a>) = 
        let g =  new Generic.List<'a>()
        for obj in l do
            g.Add(obj)
        g

    let ToList(l : Generic.List<'T>) = 
        let s = seq{for obj in l do yield obj}
        Seq.toList s

    let TypeList<'T>(l : Generic.List<'T>) = 
        let s = seq{for obj in l do yield obj}
        Seq.toList s

//random data
module TestData = 
    let private randomDec(r: System.Random, number:float) =
        let incr = 1.5
        let x = r.Next(int number,int (number * incr))
        let p = System.Math.Round(float x + r.NextDouble(),2)
        System.Convert.ToDecimal(p)

    let private randomInt(r: System.Random, number:int)=
        let incr = 9
        int number+r.Next(number,number*incr)

    let private nextDate i =
        if(i<=30) then 20121200 + i else 20130100+(i-30)

    let private staticPrice symbol price iteration =
         new Entity.Price(Symbol=symbol, Open=price,High=price+1.25M, Low=price-0.15M, Close=price+1.10M,Volume=100000, Date= nextDate(iteration))

    let private randomPrice symbol price iteration  randomP randomV = 
        new Entity.Price(Symbol=symbol, Open=randomDec(randomP,price),High=randomDec(randomP,price), Low=randomDec(randomP,price), Close=randomDec(randomP,price),Volume=randomInt(randomV,100000), Date= nextDate(iteration))

    let StaticPrices size =
        let p = 10.00
        [for i in 1 .. size ->  staticPrice "A" 10.00M i]
    
    let RandomPrices size =
        let rVol = new System.Random()
        let rPrice = new System.Random()
        let p = 10.00
        [for i in 1 .. size ->  randomPrice "A" 10.00 i rPrice rVol]

module Thread =
    let pfor(nfrom : int32, nto : int32, f) =
        System.Threading.Tasks.Parallel.For(nfrom, nto + 1, System.Action<_>(f)) |> ignore     

    let Lock (lockObj : obj) f =
        System.Threading.Monitor.Enter lockObj
        try
              f()
        finally
             System.Threading.Monitor.Exit lockObj

    let writeFiles bufferData =
        Seq.init 1000 (fun num -> bufferData num)
        |> Seq.mapi (fun num value ->
            async {
                let fileName = "file" + num.ToString() + ".dat" 
                use outputFile = System.IO.File.Create(fileName)
                do! outputFile.AsyncWrite(value)
            })
        |> Async.Parallel
        |> Async.Ignore

//let testPFor = 
//    let y = Array.zeroCreate(11)
//    Thread.pfor(1,10,(fun x -> Thread.Lock y (fun z -> y.[x] <- y.[x] + 1))) 
    
    