// This project type requires the F# PowerPack at http://fsharppowerpack.codeplex.com/releases
// Learn more about F# at http://fsharp.net
// Original project template by Jomo Fisher based on work of Brian McNamara, Don Syme and Matt Valerio
// This posting is provided "AS IS" with no warranties, and confers no rights.
module public FSharpAnalysis.Filter

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

let private Evaluate(eq : Equation, company : Entity.Company, search : Search, prices : list<Entity.Price>, view : View.FilterView) =
    try
        search.Price <- List.filter(fun (p) -> company.Symbol.Equals(p.Symbol)) prices
        if search.Price.Length >= view.Period then
            if search.Pass(eq) then
                if view.Parallel then
                    lock(view.Company) (fun x -> view.Result.Add(company)
                                                 view.Debug.Add(search.State))
                else
                    view.Result.Add(company)
                    view.Debug.Add(search.State)
        else
            view.Error.Add(new Entity.Error("eval","'" + company.Symbol.PadRight(6) + "' failed with: missing data"))
    with ex ->
        view.Error.Add(new Entity.Error("eval","'" + company.Symbol.PadRight(6) + "' failed with: " + ex.InnerException.ToString()))

let private EvaluateNonParallel(companies : list<Entity.Company> , price : list<Entity.Price>, equation : Equation, view : View.FilterView) =
    companies
        |> List.iter(fun company -> Evaluate(equation, company,new Search([], view.DebugMode), price ,view) )

//http://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/
let private EvaluateParallel(companies : list<Entity.Company> , price : list<Entity.Price>, equation : Equation, view : View.FilterView) =
    [for i in 0..companies.Length - 1 -> let s = new Search([], view.DebugMode)
                                         async { Utility.Thread.Lock(s) (fun x -> Evaluate(equation, companies.Item(i),s, price, view))} ]
                                            |> Async.Parallel
                                            |> Async.RunSynchronously
                                            |> ignore

let public Run(view : View.FilterView) =
    let p = Seq.toList view.Price
    let c = Seq.toList view.Company
    let e = ReduceFromList(view.Filter, Seq.toList view.Expression) |> Tokenize
    if view.Parallel then
        EvaluateParallel(c,p,e,view)
    else
        EvaluateNonParallel(c,p,e,view)

let public Test(expression : string, debug : int) =
    try
        let data = Utility.TestData.StaticPrices 50
        let expr = Tokenize(expression)
        let s = new Search(data,debug)
        let e = s.Evaluate(expr)
        "ok" + (if debug = 1 then " (" + s.State.ToString() + ")" else "")
    with ex ->
        ex.Message