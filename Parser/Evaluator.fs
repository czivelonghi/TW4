//www.jfsowa.com/logic/math.htm#Propositional
// define propistional/predicate type evaluators
//  {1,2,3}=set , U=union ({1,2,3}U{4,5,6}={1,2,3,4,5,6}, 
/// Evaluate an equation

module Evaluator

open System
open Microsoft.FSharp.Text.Lexing
open System.Collections
open System.Threading.Tasks
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra.Complex32
open MathNet.Numerics.Algorithms.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double

open Ast
open Lexer
open Parser
open Reducer
open System.Numerics
open Tw.Model

type BaseEvaluate(price : list<Entity.Price>, verbose : int) = 
    let mutable m_expression = Equation
    let mutable m_price : list<Entity.Price> = price
    let mutable m_state : string = ""
    let mutable m_iter : Generic.Dictionary<string,int> = new Generic.Dictionary<string,int>()
    let mutable m_keyvalue : Generic.Dictionary<string,double> = new Generic.Dictionary<string,double>()
    let mutable m_keyvalue2 : Generic.Dictionary<string,string> = new Generic.Dictionary<string,string>()
    let mutable m_index : int = -1
    let mutable m_verbose : int = verbose

    member this.Expression with get() = m_expression and set(x) = m_expression <- x
    member this.Price with get() = m_price and set(x) = m_price <- x
    member this.State with get() = m_state and set(x) = m_state <- x
    member this.Verbose with get() = m_verbose and set(x) = m_verbose <- x
    member this.Index with get() = m_index and set(x) = m_index <- x
    member private this.round(value : float) = System.Math.Round(value,2)
    member this.CurrentPrice = m_price.[m_index]

    member this.SetVar(key : string, value : float) = 
        let k = key.Replace("'","").ToLower()
        if m_keyvalue.ContainsKey(k) then
            m_keyvalue.[k]<-value
        else
            m_keyvalue.Add(k,value)

    member this.SetVar2(key : string, value : string) = 
        let k = key.Replace("'","").ToLower()
        let v = value.Replace("'","").ToLower()
        if m_keyvalue2.ContainsKey(k) then
            m_keyvalue2.[k]<-v
        else
            m_keyvalue2.Add(k,v)

    member this.Read =
        if m_index = -1 then
            m_index <- m_price.Length - 1
            true
        else if m_index <= m_price.Length && m_index > 0 then
            m_index <- m_index - 1
            true
        else
            false

    member this.GetVar(key : string) = 
        let k = key.Replace("'","").ToLower()
        if m_keyvalue.ContainsKey(k) then
            m_keyvalue.[k]
        else
            0.0

    member this.Equals(key : string,value : string) = 
        let k = key.Replace("'","").ToLower()
        let v = value.Replace("'","").ToLower()
        if m_keyvalue2.ContainsKey(k) && m_keyvalue2.[k]=v then
            1.0
        else
            0.0

    member this.NextIndex(index : int) =                
        if m_index = -1 then
            index
        else
            index + m_index

    member this.std(input : float list) =
        let sampleSize = float input.Length
        let mean = (input |> List.fold ( + ) 0.0) / sampleSize
        let differenceOfSquares =
            input |> List.fold( fun sum item -> sum + Math.Pow(item - mean, 2.0) ) 0.0
        let variance = differenceOfSquares / sampleSize
        Math.Sqrt(variance)

    member this.find(index : int) =
        let rec loop(idx : int, inc : int) =
            if idx <= -1 || idx >= m_price.Length then
                null
            else if m_price.[idx] <> null then
                m_price.[idx]
            else
                loop(idx + inc,inc)

        let mutable x = loop(index,-1)//work back
        if x = null then x <- loop(index,1) //work forward
        if x = null then x <- new Entity.Price(Symbol="",Open=0.0M,High=0.0M,Low=0.0M,Close=0.0M,Date=19000101,Volume=0)//otherwise
        x                

    member this.OHLC(index : int) =
       let pos = this.NextIndex(index)
       if (m_price.IsEmpty = false) && (m_price.Length > pos) then
            m_price.Item(pos)
       else
            this.find(pos)
    
    member this.PriceElement(prop : string, p : Entity.Price) = 
        match prop with
            | "open" -> float p.Open
            | "high" -> float p.High
            | "low" -> float p.Low
            | "close" -> float p.Close
            | "volume" -> float p.Volume
            | "date" -> float p.Date
            | _ -> float 0 

    member this.PriceValue(prop : string, index : int) =
       let pos = this.NextIndex(index)
       if m_price.IsEmpty then
            float 0
       else if m_price.Length > pos then
            this.PriceElement(prop, m_price.Item(pos))
       else
            this.PriceElement(prop,this.find(pos))
    
    member this.Log(op_state : string )=
        if m_verbose=1 then
            m_state <- if m_state<>"" then m_state + " -> " + op_state else op_state

    member this.MathOp op x y =
        this.Log (x.ToString() + op + y.ToString())
        match op with
        | "+" -> x + y
        | "-" -> this.round(x - y)
        | "/" -> this.round(x / y)
        | "*" -> x * y
        | "**" -> x ** y
        | "%" -> x % y

    member this.MathFunc1 func x =
        this.Log (func + "(" + x.ToString() + ")")
        match func with
        | "sqrt" -> sqrt(x)
        | "log" -> log(x)
        | "abs" -> abs(x)
        
    member this.MathFunc2 func x y =
        this.Log (func + "(" + x.ToString() + "," + y.ToString() + ")")
        match func with
        | "min" -> Math.Min(float x, float y)
        | "max" -> Math.Max(float x, float y)
    
    member this.Bool bool x y =
        this.Log (x.ToString() + bool + y.ToString())
        match bool with
        | "&" -> if y <> 1.0 || x <> 1.0 then 0.0 else 1.0
        | "|" -> if y <> 1.0 && x <> 1.0 then 0.0 else 1.0

    member this.Compare comp x y =
        this.Log (x.ToString() + comp + y.ToString())
        match comp with
        | ">" -> if x > y then 1.0 else 0.0
        | "<" -> if x < y then 1.0 else 0.0
        | ">=" -> if x >= y then 1.0 else 0.0
        | "<=" -> if x <= y then 1.0 else 0.0
        | "=" -> if x = y then 1.0 else 0.0
        | "!=" -> if x <> y then 1.0 else 0.0
    
    //exception!!!
    member this.Sma (period : int, column : string, pos : int) =
        this.Log ("sma(" + period.ToString() + ","  + column + "," + pos.ToString() + ")")
        let p = List.init period (fun x -> this.OHLC(x+pos))
        //let p = q |> List.filter(fun x -> x.Equals(null)=false)
        if p.Length>=period then
            match column.Replace("'","") with
            | "O" ->  p |> List.averageBy(fun x-> float x.Open)
            | "H" ->  p |> List.averageBy(fun x-> float x.High)
            | "L" ->  p |> List.averageBy(fun x-> float x.Low)
            | "C" ->  p |> List.averageBy(fun x-> float x.Close)
            | "V" ->  p |> List.averageBy(fun x-> float x.Volume)
            | _   ->  0.0
        else
            0.0
            
    //general patterns
    member this.upday(pos : int)=
        let p = this.OHLC(pos)
        this.Log ("upday(" +  p.Close.ToString() + ">"  + p.Open.ToString() + ")")
        p.Close>p.Open//Close(n1)>Open(n1)

    member this.downday(pos : int)=
        let p = this.OHLC(pos)
        this.Log ("downday(" +  p.Close.ToString() + "<"  + p.Open.ToString() + ")")
        p.Close<p.Open

    member this.higherclose(pos : int) =
        let p = this.OHLC(pos)
        let p1 = this.OHLC(pos+1)
        p1.Close<p.Close// Close(n1+1)<Close(n1)

    member this.lowerclose(pos : int) =
        let p = this.OHLC(pos)
        let p1 = this.OHLC(pos+1)
        (p1.Close > p.Close)

    member this.gapup(pos : int) =
        let p = this.OHLC(pos)
        let p1 = this.OHLC(pos+1)
        this.Log ("gapup(" + p1.High.ToString() + "<"  + p.Open.ToString() + ")")
        p1.High<p.Open

    member this.gapdown(pos : int) =
        let p = this.OHLC(pos)
        let p1 = this.OHLC(pos+1)
        this.Log ("gapdown(" + p1.Low.ToString() + ">"  + p.Open.ToString() + ")")
        p1.Low>p.Open
    
    //exhusastion gap(gap w/vol) followed by a breakaway gap
    member this.islandreversal(pos : int) =
        this.gapup(pos+1)&&this.gapdown(pos)//this.gapup(pos+1)&&this.upday(pos+1)&&this.gapdown(pos)&&this.downday(pos)
    
    //pat2('fb',0,3)=0=pos, 3=# of test periods
    member this.flatbase(pos : int,sma : int, testperiods : int) = 
        let rec loop(pos : int, period : int, i:int, acc:bool) =
            if i<period && acc then
                 let p = this.OHLC(pos + i)
                 let a = (this.Sma(sma,"H",pos+i)>=float p.High)&&(this.Sma(sma,"L",pos+i)<=float p.Low)
                 loop(pos, period, i+1,a)
            else
                acc
        loop(pos,testperiods,0,true)

    member this.macrossover(pos : int, ohlc : string,maperiod1 : int, maperiod2: int ) =
        let x=1//AvgClose(n1,n3+1)<AvgClose(n2,n3+1)  
        //AvgClose(n1,n3)>AvgClose(n2,n3))
        //(Close(1+n2) < AvgClose(n1,1+n2)
        //(Close(n2) > AvgClose(n1,n2))
        x

    //macover(0,'C',20,40)
    member this.macrossunder(pos : int, ohlc : string ) =
        let x=1//and(AvgClose(n1,n3+1)>AvgClose(n2,n3+1),AvgClose(n1,n3)<AvgClose(n2,n3))
        x //and((Close(n2+1) > AvgClose(n1,n2+1)),(Close(n2) < AvgClose(n1,n2)))
    
    member this.Pat1(pat : string, pos : int) =
        this.Log ("pat1(" + pat.ToLower() + ","  +  pos.ToString() + ")")
        match pat.Replace("'","").ToLower() with
        | "gu"     -> if this.gapup(pos) then 1.0 else 0.0
        | "gd"     -> if this.gapdown(pos) then 1.0 else 0.0
        | "islrev" -> if this.islandreversal(pos) then 1.0 else 0.0
        | _        ->  0.0
    
    member this.Pat2(pat : string, pos : int, n1 : int) =
        this.Log ("pat2(" + pat.ToLower() + ","  +  pos.ToString() + ")")
        match pat.Replace("'","").ToLower() with
        | "macover"     -> if this.flatbase(pos,30,n1) then 1.0 else 0.0
        | "macunder"    -> if this.flatbase(pos,30,n1) then 1.0 else 0.0
        | _        ->  0.0

    member this.Pat3(pat : string, pos : int, n1 : int, n2 : int) =
        this.Log ("pat3(" + pat.ToLower() + ","  +  pos.ToString() + ")")
        match pat.Replace("'","").ToLower() with
        | "fb"          -> if this.flatbase(pos,n1,n2) then 1.0 else 0.0
        | _        ->  0.0

    //+++candle patterns+++
    //small body: Spinning body where close > open by 1.01%. 
    member this.smallbody(pos : int,t : string) =
        let p = this.OHLC(pos)
        let perc = 1.005M
        if t="white" then
            (p.Close>=p.Open)&&(p.Close<=p.Open*perc)// AND(Close(n1)>=Open(n1),Close(n1)<=(Open(n1)*1.01))
        else
            (p.Open>=p.Close)&&(p.Open<=p.Close*perc)// AND(Open(n1)>=Close(n1),Open(n1)<=(Close(n1)*1.01))

    //doji: open=close
    member this.doji(pos : int) =
        let p = this.OHLC(pos)
        this.Log ("doji(" + p.Open.ToString() + "="  +  p.Close.ToString() + ")")
        (p.Close = p.Open)

    //wc: close above open
    member this.whitecandle(pos : int, diff : decimal) =
        let diff = 1.02M
        let p = this.OHLC(pos)
        this.Log ("whitecandle(" + p.Close.ToString() + ">"  +  p.Open.ToString() + " * " + diff.ToString() + ")")
        (p.Close > p.Open*diff)

    //bc: close below open
    member this.blackcandle(pos : int, diff : decimal) =
        let p = this.OHLC(pos)
        this.Log ("blackcandle(" + p.Open.ToString() + ">"  +  p.Close.ToString() + " * " + diff.ToString() + ")")
        (p.Close*diff < p.Open)

    //blhrm: current day closes up and body is within the previous down day.
    member this.bullishharami(pos : int) = 
        let p =  this.OHLC(pos)
        let p1 =  this.OHLC(pos+1)
        ((p1.Close<p.Open)&&(p1.Open>p.Close)&&this.downday(pos+1)&&this.upday(pos)) //and(close(n1+1)<open(n1),open(n1+1)>close(n1),downday(n1+1),upday(n1))

    //brhrm: current day closes down and within the previous up day.
    member this.bearishharami(pos : int) = 
        let p =  this.OHLC(pos)
        let p1 =  this.OHLC(pos+1)
        ((p1.Close<p.Open)&&(p1.Open>p.Close)&&this.downday(pos+1)&&this.upday(pos)) //and(close(n1+1)>open(n1),open(n1+1)<close(n1),upday(n1+1),downday(n1))
                
    //blkr: 3 white candle with a preceeding black candled.
    member this.bearishkicker(pos : int) = 
        let incr = 1.02M//candle size
        (this.whitecandle(pos+1,incr) && this.whitecandle(pos+2,incr) && this.whitecandle(pos+3,incr) && this.blackcandle(pos,incr))

    //brkr: 3 black candle with a preceeding a white candle.
    member this.bullishkicker(pos : int) = 
        let incr = 1.02M//candle size
        (this.blackcandle(pos+1,incr) && this.blackcandle(pos+2,incr) && this.blackcandle(pos+3,incr) && this.whitecandle(pos,incr))

    //tt: long tail above close/open.
    //n1=day,n2=percent (increase to increase tail length)
    member this.toptail(pos : int,perc : decimal) =
        let p = this.OHLC(pos)
        ((p.Open*perc)<p.High)&&((p.Close*perc)<p.High) //AND((Open(n1)*n2)<High(n1),(Close(n1)*n2)<High(n1))
    
    //blms: 1st day black candle, 2nd day small body, 3rd day a white candle.
    member this.bullishmorningstar(pos : int) =
        (this.downday(pos+2)&&this.gapdown(pos+1)&&(this.smallbody(pos+1,"white")||this.smallbody(pos+1,"black"))&&this.upday(pos)&&this.higherclose(pos))//and(downday(n1+2), gapdown(n1+1), smallbody(n1+1),upday(n1) ,higherclose(n1) )
    
    //bres: 1st day white candle, 2nd day small body, 3rd day a black candle.
    member this.bearisheveningstar(pos : int) =
        (this.upday(pos+2)&&this.gapup(pos+1)&&this.upday(pos)&&(this.smallbody(pos+1,"white")||this.smallbody(pos+1,"black"))&&this.downday(pos))//and(upday(n1+2),gapup(n1+1),smallbody(n1+1),downday(n1))

    //dfd: long body with doji at top
    //n1= body size % (.5 = 50% body size of candle), n2=day. I.e. Hanging man=.5 , doji=.2
    member this.dragonflydoji(pos : int, bodysize : decimal) =
        let p = this.OHLC(pos)
        if (p<>null) then
            let x = Math.Abs(p.Open - p.Close)/(0.001M + p.High - p.Low)<=bodysize //ABS(open(n2)-Close(n2))/(.001+High(n2)-Low(n2))<=n1
            let y = (p.Close - p.Low)/(0.001M + p.High - p.Low)>=(1.0M - bodysize) //(Close(n2)-Low(n2))/(.001+High(n2)-Low(n2))>=1-n1
            let z = (p.Open - p.Low)/(0.001M + p.High-p.Low)>=(1.0M - bodysize) //(open(n2)-Low(n2))/(.001+High(n2)-low(n2))>=1-n1)
            (x&&y&&z)
        else
            false
    
    //Current body engulfs previous day high and low
    //n1=day"
    member this.bullishengulfing(pos : int) =
        let p = this.OHLC(pos)
        let p2 = this.OHLC(pos+1)
        let w = (p2.High<p.Close) //high(n1+1)<close(n1)
        let x = (p2.Low>p.Open) //low(n1+1)>open(n1)
        let y = this.downday(pos+1) //downday(n1+1)
        let z = this.upday(pos) //upday(n1)
        (w&&x&&y&&z)

    member this.bearishengulfing(pos : int) =
        let p = this.OHLC(pos)
        let p2 = this.OHLC(pos+1)
        let w = (p2.High<p.Open) //(high(n1+1)<open(n1)
        let x = (p2.Low>p.Close) //low(n1+1)>close(n1)
        let y = this.upday(pos+1) //upday(n1+1)
        let z = this.downday(pos) //downday(n1))
        (w&&x&&y&&z)

    member this.threeblacksoldiers(pos : int, diff : decimal) =
        let p = this.OHLC(pos)
        let x = this.blackcandle(pos,diff) && this.blackcandle(pos+1,diff) && this.blackcandle(pos+2,diff)
        let y = this.lowerclose(pos) && this.lowerclose(pos+1) && this.lowerclose(pos+2)
        (x&&y)

    member this.threewhitesoldiers(pos : int, diff : decimal) =
        let p = this.OHLC(pos)
        let x = this.whitecandle(pos,diff) && this.whitecandle(pos+1,diff) && this.whitecandle(pos+2,diff)
        let y = this.higherclose(pos) && this.higherclose(pos+1) && this.higherclose(pos+2)
        (x&&y)

    //gsd: Doji with long topping tail
    //n1= body size % (.5 = 50% body size of candle),n2=day
    //I.e. Hammer =.5, Doji=.2
    //!!!!!NOT WORKING!!!!
    
    member this.gravestonedoji(pos : int, bodysize : decimal) =
        let p = this.OHLC(pos)
        let x = Math.Abs(p.Open - p.Close)/(0.001M + p.High-p.Low)<=bodysize //ABS(open(n2)-Close(n2))/(.001+High(n2)-Low(n2))<=n1
        let y = (p.Close - p.Low)/(0.001M + p.High - p.Low)<=bodysize//(Close(n2)-Low(n2))/(.001+High(n2)-Low(n2))<=n1
        let z = (p.Open - p.Low)/(0.001M + p.High-p.Low)<=bodysize//(open(n2)-Low(n2))/(.001+High(n2)-low(n2))<=n1)
        (x&&y&&z)

    member this.Candle(candletype : string, pos : int) =
        this.Log ("candle(" + candletype.ToLower() + ","  +  pos.ToString() + ")")
        match candletype.Replace("'","").ToLower() with
        | "doji" -> if this.doji(pos) then 1.0 else 0.0
        | "3ws" -> if this.threewhitesoldiers(pos,1.02M) then 1.0 else 0.0
        | "3bs" -> if this.threeblacksoldiers(pos,1.02M) then 1.0 else 0.0
        | "wc" -> if this.whitecandle(pos,1.0M) then 1.0 else 0.0
        | "bc" -> if this.blackcandle(pos,1.0M) then 1.0 else 0.0
        | "tt" -> if this.toptail(pos,1.02M) then 1.0 else 0.0
        | "dfd" -> if this.dragonflydoji(pos,0.02M) then 1.0 else 0.0
        | "hm" -> if this.dragonflydoji(pos,0.05M) then 1.0 else 0.0
        | "ihm" -> if this.gravestonedoji(pos,0.05M) then 1.0 else 0.0
        | "gsd" -> if this.gravestonedoji(pos,0.02M) then 1.0 else 0.0
        | "bleng" -> if this.bullishengulfing(pos) then 1.0 else 0.0
        | "breng" -> if this.bearishengulfing(pos) then 1.0 else 0.0
        | "blkr" -> if this.bullishkicker(pos) then 1.0 else 0.0
        | "brkr" -> if this.bearishkicker(pos) then 1.0 else 0.0
        | "wsb" -> if this.smallbody(pos,"white") then 1.0 else 0.0
        | "bsb" -> if this.smallbody(pos,"black") then 1.0 else 0.0
        | "blms" -> if this.bullishmorningstar(pos) then 1.0 else 0.0
        | "bres" -> if this.bearisheveningstar(pos) then 1.0 else 0.0
        | _   ->  0.0

    //simple linear regression. usage: slope('C',0,50)>.05
    member this.Slope(column : string, pos : int, periods : int) = 
        let x = [| 1.0 .. float periods |]//do we initialize at 1.0 or 0.0
        let c =  match column.Replace("'","") with
                    | "O" -> "open"
                    | "C" -> "close"
                    | "H" -> "high"
                    | _ -> "low"

        let y = Array.init periods (fun a -> (float(this.PriceValue(c,pos+a))))
        try
            let offset, slope = Fit.line x y//offset closer to 1.0 equals perfectly fit to line. pos slope=uptrending, negative=downtrending
            if slope>=0.8 then
                printfn "%s: %f - %f" (this.OHLC(pos).Symbol) offset slope
            slope
        with ex-> 0.0
    
    //add correlation here
    member this.Corr() =
        let c = 1.0
        c
    
    //standard deviation
    member this.Std(column : string, pos : int) =
        let period = 10//adjust this
        let p = List.init period (fun x -> this.OHLC(x+pos))
        List.init period (fun x -> float(this.OHLC(x+pos).Close)) |> this.std

    //historical vol     
    member this.HVol (period : int, pos : int) =
        let tradingperiods=252
        this.Log ("hvol(" + period.ToString() + ")")
        let p = List.init 200 (fun x -> this.OHLC(x+pos))//filter prices base on idx
        let np = [for i in 0 .. p.Length-1 do
                    let mutable lastprice = 0M
                    if i = 0 then
                        lastprice <- p.[i].Close
                    else
                        let diff = float(p.[i].Close / lastprice)
                        yield  Math.Log(diff)]//calc list of price changes
        let idx = pos - (period-1)
        this.std(np) *  Math.Sqrt(float tradingperiods)

    member this.Incr(var: string) =
        if m_iter.ContainsKey(var) then
            m_iter.[var]<-m_iter.[var]+1
        else
            m_iter.Add(var,0)

    member this.Evaluate expr =
        this.State <- ""
        let store x y = [x;y]

        let rec evalFactor factor =
            match factor with
            | Float x   -> x
            | Integer x -> float x
            | ParenEx x -> evalExpr x

        and evalTerm term =
            match term with
            | Sqrt  (fact)              -> this.MathFunc1 "sqrt" (evalFactor fact)
            | Log  (fact)               -> this.MathFunc1 "log" (evalFactor fact)
            | Min (exp1, exp2)          -> this.MathFunc2 "min" (evalExpr exp1) (evalExpr exp2)
            | Max (exp1, exp2)          -> this.MathFunc2 "max" (evalExpr exp1) (evalExpr exp2)
            | Sma (exp1, string, exp2)  -> this.Sma(int(evalExpr exp1), string, int(evalExpr exp2))
            | Std (string, exp2)        -> this.Std(string, int(evalExpr exp2))
            | Slope (string, exp2, exp3)-> this.Slope(string, int(evalExpr exp2), int(evalExpr exp3))
            | Candle (string, exp2)     -> this.Candle(string, int(evalExpr exp2))
            | Pat1 (string, exp2)       -> this.Pat1(string, int(evalExpr exp2))
            | Pat2 (string, exp2, exp3) -> this.Pat2(string, int(evalExpr exp2),int(evalExpr exp3))
            | Pat3 (string, exp2, exp3,exp4) -> this.Pat3(string, int(evalExpr exp2),int(evalExpr exp3),int(evalExpr exp4))
            | O (fact)                  -> this.PriceValue("open", int (evalFactor fact))
            | H (fact)                  -> this.PriceValue("high", int (evalFactor fact))
            | L (fact)                  -> this.PriceValue("low", int (evalFactor fact))
            | C (fact)                  -> this.PriceValue("close", int (evalFactor fact))
            | V (fact)                  -> this.PriceValue("volume", int (evalFactor fact))
            | D (fact)                  -> this.PriceValue("date", int (evalFactor fact))
            | If (exp1, exp2, exp3)     -> if evalExpr exp1 <> 1.0 then evalExpr exp3 else evalExpr exp2
            | For (total, exp)          -> [1..int(evalFactor total)] |> (List.fold (fun acc elem -> this.Bool "&" (evalExpr exp) (acc) ) 1.0)//change this
            | And (total, exp)          -> [1..int(evalFactor total)] |> (List.fold (fun acc elem -> this.Bool "&" (evalExpr exp) (acc) ) 1.0)
            | Or (total, exp)           -> [1..int(evalFactor total)] |> (List.fold (fun acc elem -> this.Bool "|" (evalExpr exp) (acc) ) 0.0)
            | Count(incr, total, exp)   -> [1..int(evalFactor total)] |> (List.fold (fun acc elem -> this.Incr(incr)
                                                                                                     this.MathOp "+" (evalExpr exp) (acc)) 0.0)
            | Abs (expr)                -> this.MathFunc1 "abs" (evalExpr expr)
            | Incr  (expr)              -> float m_iter.[expr]
            | Var  (string)             -> this.GetVar(string)
            | Equals(str1,str2)         -> this.Equals(str1,str2)
            | Factor fact               -> evalFactor fact

        and evalExpr expr =
            match expr with
            | Plus  (expr, term) -> this.MathOp "+" (evalExpr expr) (evalTerm term)
            | Minus (expr, term) -> this.MathOp "-" (evalExpr expr) (evalTerm term)
            | Pow   (expr, term) -> this.MathOp "**" (evalExpr expr) (evalTerm term)
            | Times (expr, term) -> this.MathOp "*" (evalExpr expr) (evalTerm term)
            | Divide (expr, term)-> this.MathOp "/" (evalExpr expr) (evalTerm term)
            | Modulus(expr, term)-> this.MathOp "*" (evalExpr expr) (evalTerm term)
            | Eq    (e1, e2)     -> this.Compare "=" (evalExpr e1) (evalExpr e2)
            | Gt    (e1, e2)     -> this.Compare ">" (evalExpr e1) (evalExpr e2)
            | Ge    (e1, e2)     -> this.Compare ">=" (evalExpr e1) (evalExpr e2)
            | Lt    (e1, e2)     -> this.Compare "<" (evalExpr e1) (evalExpr e2)
            | Le    (e1, e2)     -> this.Compare "<=" (evalExpr e1) (evalExpr e2)
            | Ne    (e1, e2)     -> this.Compare "!=" (evalExpr e1) (evalExpr e2)
            | Amp   (e1, e2)     -> this.Bool "&" (evalExpr e1) (evalExpr e2)
            | Pipe  (e1, e2)     -> this.Bool "|" (evalExpr e1) (evalExpr e2)
            | Term term          -> evalTerm term

        and evalEquation eq =
            match eq with
            | Equation expr -> evalExpr expr

        try
            evalEquation expr
        with
        | e -> float -1

    member this.Pass expr =
        let p = this.Evaluate expr
        if p = 1.0 then
            true
        else
            false