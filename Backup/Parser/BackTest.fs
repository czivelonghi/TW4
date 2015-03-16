module public FSharpAnalysis.BackTest

open System
open Microsoft.FSharp.Text.Lexing
open System.Collections
open System.Threading.Tasks
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections

open Ast
open Lexer
open Parser
open Reducer
open Evaluator
open System.Numerics
open System.Collections.Generic 
open Utility
open Tw.Model

exception private MissingDataError of string
exception private TokenizeError of string

let private dict = Dictionary<_, _>()

let private Memoize f =
    fun n ->
        match dict.TryGetValue(n) with
        | (true, v) -> v
        | _ ->
            let temp = f(n)
            dict.Add(n, temp)
            temp

let private Reduce(expression : string, list : list<Entity.Expression>) =
    let expr = ReduceFromList(expression.ToLower(), list)
    expr

let private Tokenize(expression : string) =
    try
        let lexbuff = LexBuffer<char>.FromString(expression.ToUpper())
        let equation = Parser.start Lexer.tokenize lexbuff
        equation
    with
    | ex -> raise (TokenizeError("expression error: " + expression))

let private MemoizeToken expression = 
    Memoize Tokenize expression

type Search(price : list<Entity.Price>, debug : int) =  
    inherit BaseEvaluate(price,debug)
    //extend here
    member this.Test = 1 + 1

type private Exit(parentid : string, exit : Entity.Exit, expression: list<Entity.Expression>) =
    let mutable m_id= ""
    let mutable m_active= true
    member this.ParentID=parentid
    member this.Name=exit.Name
    member this.Expression=Reduce(exit.Expression,expression)
    member this.Equation =Tokenize(Reduce(exit.Expression,expression))
    member this.RiskPercent= if exit.RiskType="p" then exit.Risk else 0.0M
    member this.RiskAmount = if exit.RiskType <> "p" then int exit.Risk else 0
    member this.ID with get() = m_id and set(x) = m_id <- x
    member this.Active with get() = m_active and set(x) = m_active <- x

type private Entry(entry : Entity.Entry, expression: list<Entity.Expression>)=
    let mutable m_id= ""
    let mutable m_parentid=""
    let mutable m_exit : list<Exit> = [for x in entry.Exit do yield new Exit(m_id,x,expression)]
    let mutable m_opendate : int = 0
    let mutable m_closedate : int = 0
    let mutable m_openprice = 0.0M
    let mutable m_closeprice = 0.0M
    let mutable m_shares = 0
    let mutable m_commission = 0.0M
    let mutable m_timelimit = entry.TimeLimit
    
    member this.Name=entry.Name
    member this.ID with get() = m_id and set(x) = m_id <- x
    member this.ParentID with get() = m_parentid 
    member this.OpenDate with get() = m_opendate and set(x) = m_opendate <- x
    member this.CloseDate with get() = m_closedate and set(x) = m_closedate <- x
    member this.OpenPrice with get() = m_openprice and set(x) = m_openprice <- x
    member this.ClosePrice with get() = m_closeprice and set(x) = m_closeprice <- x
    member this.Expression=Reduce(entry.Expression,expression)
    member this.Equation =Tokenize(Reduce(entry.Expression,expression))
    member this.Type=entry.EntryType //short / long
    member this.RiskPercent= if entry.RiskType="p" then entry.Risk else 0.0M
    member this.RiskAmount = if entry.RiskType <> "p" then int entry.Risk else 0
    member this.StopLoss=entry.StopLoss //% loss
    member this.Exit with get() = m_exit and set(x) = m_exit <- x
    member this.Total = (decimal m_shares * m_openprice) + m_commission 
    member this.Shares  with get() = m_shares and set(x) = m_shares <- x
    member this.Commission  with get() = m_commission and set(x) = m_commission <- x
    member this.TimeLimit  with get() = m_timelimit and set(x) = m_timelimit <- x

type private Test(symbol, backTest : Entity.BackTest, t : Search, expression : list<Entity.Expression>) = 
    let mutable m_trade : list<Entity.Trade> = []
    let mutable m_balance = backTest.Capital
    let mutable m_commission = backTest.Commission
    let mutable m_search = t
    let mutable m_entry : list<Entry> = [for x in backTest.Entry do yield new Entry(x, expression)]
    let Round(p:decimal) = Math.Round(p,2)

    let OpenShares(e : Entry) = 
        if e.RiskPercent > 0.0M then
            int(Round((m_balance * e.RiskPercent)/e.OpenPrice))
        else
            e.RiskAmount

    let ExitShares(e : Entry, x : Exit) = 
        if x.RiskPercent > 0.0M then
            int(Round(decimal e.Shares * x.RiskPercent))
        else
            x.RiskAmount

    member this.Symbol = symbol
    member this.MaxPosition=backTest.MaxPosition
    member this.Search with get() = m_search
    member this.Commission with get() = m_commission
    member this.Balance with get() = m_balance and set(x) = m_balance <- x
    member this.Entry with get() = m_entry and set(x) = m_entry <- x
    member this.Trade with get() = m_trade and set(x) = m_trade <- x
    member this.ExitStatus(e : Entry, active: Boolean) = e.Exit |> List.iter(fun x-> x.Active<-active)
    member this.CalcExitTotal(e : Entry, t : Entity.Trade) =
        if e.Type="long" then
            (t.Price * decimal t.Shares) - m_commission
        else
            (((e.OpenPrice-t.Price) + e.OpenPrice) * decimal t.Shares) - m_commission

    member this.EnterTrade(e : Entry) = 
        let t = new Entity.Trade()
        e.ID <- System.Guid.NewGuid().ToString()
        t.ID<-e.ID
        t.Commission <- m_commission
        t.Date<- e.OpenDate
        t.Description <- e.Name
        t.Symbol <- this.Symbol
        t.ParentID<- e.ParentID
        t.Price<-e.OpenPrice
        t.Shares<-OpenShares(e)
        t.TradeType<-if e.Type="long" then "buy" else "sell"
        t.PositionType <- e.Type
        this.Trade <- t :: this.Trade
        t.Expression<-this.Search.State
        t.Total<- (t.Price * decimal t.Shares) + t.Commission
        t
    
    member this.StopTrade(e : Entry, d : string) = 
        let t = new Entity.Trade()
        t.ParentID  <- e.ID
        t.Symbol    <- this.Symbol
        t.Price     <- Round(this.Search.CurrentPrice.Close)
        t.Date      <- this.Search.CurrentPrice.Date
        t.Commission<- m_commission
        t.Description<- d
        t.Shares    <- e.Shares
        t.TradeType <- if e.Type="long" then "sell" else "buy"
        t.PositionType<- e.Type
        t.Total<- this.CalcExitTotal(e,t)
        this.Trade  <- t :: this.Trade
        t

    member this.ExitTrade(e : Entry, x : Exit) = 
        let t = new Entity.Trade()
        t.ID        <- System.Guid.NewGuid().ToString()
        t.ParentID  <- e.ID
        t.Symbol    <- this.Symbol
        t.Price     <- Round(this.Search.CurrentPrice.Close)
        t.Date      <- this.Search.CurrentPrice.Date
        t.Commission<- m_commission
        t.Description<- x.Name
        t.Shares    <-ExitShares(e,x)
        t.TradeType <-if e.Type="long" then "sell" else "buy"
        t.PositionType<- e.Type
        t.Total<- this.CalcExitTotal(e,t)
        this.Trade  <- t :: this.Trade
        t

    member this.Active(e : Entry) = (e.OpenDate > 0 && e.CloseDate=0)

    member this.StopPosition(e : Entry, d : string) =
        let t = this.StopTrade(e,d)
        e.Shares <- 0
        e.CloseDate  <- this.Search.CurrentPrice.Date
        e.ClosePrice <- Round(this.Search.CurrentPrice.Close)
        this.Balance <-this.Balance + t.Total
        t.Balance<-this.Balance //running balance of each trade
            
    member this.ExitPosition(e : Entry,x : Exit) =
        let t = this.ExitTrade(e,x)
        e.Shares <- e.Shares - t.Shares
        x.Active <- false
        t.Expression<-this.Search.State
        if e.Shares=0 then
            e.CloseDate  <- t.Date
            e.ClosePrice <- t.Price
        this.Balance<-this.Balance + t.Total
        t.Balance<-this.Balance //running balance of each trade

    member this.OpenPosition(e : Entry) =
        e.OpenDate  <- this.Search.CurrentPrice.Date
        e.OpenPrice <- Round(this.Search.CurrentPrice.Close)
        e.CloseDate <- 0
        e.ClosePrice<- 0.0M
        let t = this.EnterTrade(e)
        e.Shares    <- t.Shares
        this.ExitStatus(e,true)
        this.Balance<-this.Balance - t.Total
        t.Balance<-this.Balance

    member this.CloseOutTrade(e : Entry) = this.StopPosition(e,"closeout")
    member this.ExpireTrade(e : Entry) = this.StopPosition(e,"time limit")
    member this.StopLossTrade(e : Entry) = this.StopPosition(e,"stoploss")
    member this.Evaluate(expr : string) = this.Search.Pass(MemoizeToken(expr))

    member this.Exit(e : Entry) = 
        let mutable ret = false
        for x in e.Exit do
            if x.Active=true && this.Active(e)=true then
                if this.Evaluate(x.Expression) then
                    this.ExitPosition(e,x)
                    ret<-true
        ret

    member this.Enter(e : Entry) = 
        if this.Active(e)=false then
            if this.Evaluate(e.Expression) then
                this.OpenPosition(e)

    member this.Stop(e : Entry) = 
        let mutable ret = false
        if this.Active(e)=true && e.StopLoss>0.0M then
            let sl = e.StopLoss * e.OpenPrice
            if e.Type="long" then
                ret <- (this.Search.CurrentPrice.Low <= (e.OpenPrice-sl))
            else
                ret <- (this.Search.CurrentPrice.High >= (sl + e.OpenPrice))
        ret

    member this.TimeLimit(e : Entry) = 
        let mutable ret = false
        if this.Active(e)=true && e.TimeLimit > 0 then
            if this.Search.CurrentPrice.Interval="day" then
                let diff = Utility.Convert.DateDiff(this.Search.CurrentPrice.Date,e.OpenDate)
                ret <- (e.TimeLimit < diff)
        ret

    member this.Close =
        this.Entry
            |> List.filter(fun e-> this.Active(e)=true)
            |> List.iter(fun e-> this.CloseOutTrade(e))
    
    member this.SetEntryVars(e : Entry) = 
        if this.Active(e)=true then
            this.Search.SetVar("entrydate", float e.OpenDate)//underscores do not work
            this.Search.SetVar("entryprice",float e.OpenPrice)
            this.Search.SetVar2("entrytype",e.Type)
    //additional vars here

    member this.Evaluate() =
        while this.Search.Read do
            for e in this.Entry do
                this.SetEntryVars(e)
                if this.Stop(e) then
                    this.StopLossTrade(e)
                else if this.TimeLimit(e) then
                    this.ExpireTrade(e)    
                else if this.Exit(e)=false then
                    this.Enter(e)
        this.Close

let private EvaluateNonParallel(companies : list<Entity.Company> , prices : list<Entity.Price>,expression : list<Entity.Expression>, view : View.BackTestView) =
    companies 
        |> List.iter(fun company -> 
                        try
                            let price = prices |> List.filter(fun (p) -> company.Symbol = p.Symbol)   
                            if price.Length=0 then raise (MissingDataError(company.Symbol + ": missing price data"))  
                            let t = new Test(company.Symbol,view.BackTest,new Search(price,view.BackTest.Debug),expression)
                            t.Evaluate()
                            let r = new Result.BackTestResult(Company=company,Balance = t.Balance, Capital = view.BackTest.Capital,Trade= (Utility.Convert.ToGenericList <| List.rev t.Trade))
                            view.Result.Add(r)
                        with
                            | :? MissingDataError as ex -> view.Error.Add(new Entity.Error("data","'" + ex.Data0))
                            | :? TokenizeError as ex -> view.Error.Add(new Entity.Error("tokenize","'" + ex.Data0))
                            | ex -> view.Error.Add(new Entity.Error("error","'" + ex.Message)))

let private AsynchEval(company : Entity.Company, t : Test, prices : list<Entity.Price>, view : View.BackTestView) =
    try
        let price = prices |> List.filter(fun (p) -> company.Symbol.Equals(p.Symbol))
        if price.Length = 0 then raise (MissingDataError(company.Symbol + ": missing price data"))  
        t.Evaluate()
        let r = new Result.BackTestResult(Company= company, Balance = t.Balance, Capital = view.BackTest.Capital,Trade= (Utility.Convert.ToGenericList <| List.rev t.Trade))
        lock(view.Result)(fun x-> view.Result.Add(r))
    with
        | :? MissingDataError as ex -> lock(view.Error)(fun x-> view.Error.Add(new Entity.Error("data","'" + ex.Data0)))
        | :? TokenizeError as ex -> lock(view.Error)(fun x -> view.Error.Add(new Entity.Error("tokenize","'" + ex.Data0)))
        | ex -> lock(view.Error)(fun x-> view.Error.Add(new Entity.Error("evalEquation","'" + company.Symbol + "' failed with: " + ex.InnerException.ToString())))

let private EvaluateParallel(companies : list<Entity.Company> , prices : list<Entity.Price>,expression : list<Entity.Expression>, testView : View.BackTestView) =
         [for i in 0..companies.Length - 1 -> let t = new Test( companies.Item(i).Symbol,testView.BackTest,new Search(prices,testView.BackTest.Debug),expression)
                                              async {Utility.Thread.Lock(t) (fun x -> AsynchEval( companies.Item(i),t,prices,testView)) } ]
                                              |> Async.Parallel
                                              |> Async.RunSynchronously
                                              |> ignore

let public Run(view : View.BackTestView) =
    let p = Seq.toList view.Price
    let c = Seq.toList view.Company
    let e = Seq.toList view.Expression
    if view.Parallel then
        EvaluateParallel(c,p,e,view)
    else
        EvaluateNonParallel(c,p,e,view)

