module FSharpAnalysis.Metric

open System
open Microsoft.FSharp.Text.Lexing
open System.Numerics
open System.Collections
open System.Threading.Tasks
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections

open Evaluator
open Ast
open Lexer
open Parser
open Reducer
open Tw.Model

type Search(price : list<Entity.Price>, debug : int) =  inherit BaseEvaluate(price,debug)

let Tokenize(expression : string) =
    let lexbuff = LexBuffer<char>.FromString(expression.ToUpper())
    let equation = Parser.start Lexer.tokenize lexbuff
    equation

type private Metric(symbol, view : View.MetricView, search : Search, study : list<(string * Equation)> ) = 
    let mutable m_search = search
    let mutable m_study = study
    let mutable m_type = view.Type
    member this.Search with get() = m_search

    member this.Count((s,e) : (string * Equation)) = 
        let p = search.CurrentPrice
        if this.Search.Pass(e) then view.AddCount(s,p.Symbol,p.Date)
    //other study types go here


    member this.Evaluate() =
        while this.Search.Read do
            study |> List.iter(fun (x,y) -> if m_type ="count" then this.Count(x,y))///other study types

let private EvaluateNonParallel(company : list<Entity.Company>, price : list<Entity.Price>, study : list<(string*Equation)>, view : View.MetricView) =
     company 
        |> List.iter(fun c-> let p = price |> List.filter(fun p -> p.Symbol = c.Symbol)
                             let m = new Metric(c.Symbol,
                                                view,
                                                new Search(p,view.DebugMode),
                                                study)
                             m.Evaluate())

let TokenizeStudy(view : View.MetricView)  =
    let study = Seq.toList view.Study
    let expression = Seq.toList view.Expression
    let eq = seq{for s in study do
                    let (ok,eq) = (try
                                    let e = ReduceFromList(s.Expression,expression)
                                    (true,Tokenize(e))
                                   with ex ->
                                    view.Error.Add(new Entity.Error("tokenizestudy","'" + s.Name + "' failed with: " + ex.Message))
                                    (false,Tokenize("0=1")))
                    if ok then yield(s.Name,eq)}
                        
    Seq.toList eq

let public Run(view : View.MetricView) =
    let e = TokenizeStudy(view)
    let p = Seq.toList view.Price
    let c = Seq.toList view.Company
    EvaluateNonParallel(c,p,e,view)


//
//    if view.Parallel then
//        EvaluateParallel(c,p,e,view)
//    else
//        EvaluateNonParallel(c,p,e,view)
