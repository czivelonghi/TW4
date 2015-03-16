module Eval

open Microsoft.FSharp.Quotations
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Signals
open MathNet.Numerics.Statistics
open MongoData
open MongoData.Types

module Candles = 
    let Doji(data: Stock) = 
        data.Close = data.Open

    let White(data: Stock) = 
        data.Close > data.Open

    let Black(data: Stock) = 
        data.Close < data.Open

module Bool = 
    //Op evaluator
    let Eval(a:'a, op:string) =
        let andACC acc x = acc && x
        let orACC acc x = acc || x
        match op with
        | "AND" -> List.fold andACC true a
        | "OR" -> List.fold orACC true a

module Math = 
    let private pctChange (val1:float, val2:float) =
       System.Math.Round(( (val2-val1) / Array.max [|val1; val2|] ) * 100.00,2)

    let PctChange (data:Stock) =
        pctChange(data.Open,data.Close)

    let MovAvg(periods: int, lst: seq<float>) =
       Seq.windowed periods lst
       |> Seq.map Array.average


//    let private PercentChangeList(n:double[]) =
//        let x = ( (n.[1]-n.[0]) / Array.max n ) * 100.00
//        Math.Round(x, 2)

module Statistics = 
    let public Correlation(val1: float, val2: float) = 
        let x = seq[1.0;2.0]    //let v1 = Seq.singleton val1
        let y = seq[1.0;1.0]  
        let data = Correlation.Pearson(x, y)//Correlation.Pearson(seq[val1],seq[val2])
        data

    let public Correlated(val1: float, val2: float, threshold: float) = 
             Correlation(val1,val1) >=threshold

    let public StdDev(values: seq<float>) =
            let value = Statistics.StandardDeviation(values)
            System.Math.Round(value)
            
//   let Sum(lst:#seq<'a>) =
//        let accumulate acc x = acc + x         
//        Array.fold accumulate 0 lst
//

//
//    let Correlation3(lst1: #seq<'a>, lst2: #seq<'a>) =
//        [for x in 1..lst1 |> Seq.length do
//            let a = [1.0]
//            let b = [1.0]
//            yield Correlation.Pearson(a,b)]
//    
//    let Test x y = 
//        let z = x + y
//        z

//    let Correlation(l1:#seq<double>, l2:#seq<'double>) =
//        Seq.map2(fun x y ->  Correlation.Pearson((x),(y)) ) l1 l2 

//    let Correlation2(l1, l2) =
//        Seq.map2(fun x y ->  Correlation.Pearson((x),(y)) ) l1 l2 
   
//    let d = seq[1.0]    
//    let d2 = seq {2.0..3.0}
//    let data = Correlation.Pearson(d, d2)
        //Seq.iter2(fun x y - Correlate [1.0] [2]) lst1 lst2
        //(lst1, lst2) ||> Seq.map2 Correlation.Pearson 
        
//        let Correlate n1 n2 = Correlation.Pearson(n1, n2)
//        let x =  Array.fold |> Correlation.Pearson(lst1, lst2) 


//                    
//    //calc price change correalations between two lists
//    let PercentChange(lst: #seq<double>) =
//        Seq.windowed 2 lst |> Seq.map PercentChangeList
        
//let avgs = List.MovAvg 5000 (Seq.map float [|1..500|])
//
//for avg in avgs do
//    printfn "%f" avg
//    System.Console.ReadKey() |> ignore

//let MovAvgVol(symbol: string, periods: int, element: string, lst: #seq<Stock>) =
//    let accum vol = 
//    let avg = [for s in lst do yield s.Volume * ]
//    avg
    //


//var samples = new ChiSquare(5).Samples().Take(1000);
//var statistics = new DescriptiveStatistics(samples);
            
