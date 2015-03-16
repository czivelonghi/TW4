module public Reducer

    open Tw.Model

    module String =
        open System.Globalization
        let rev s =
            seq {
                let rator = StringInfo.GetTextElementEnumerator(s)
                while rator.MoveNext() do
                yield rator.GetTextElement()
            }
            |> Array.ofSeq
            |> Array.rev
            |> String.concat ""

    //String.rev "cba"

    exception UnmatchedParens of string
    exception InvalidExpression of string
 
    let clean(exp:string) = exp.Replace(" ","")
    let valid_func_char(c:char) = System.Char.IsLetterOrDigit(c) || c = '_'
    let valid_expression(exp: string) = true

    let remove_parens(func: string) =
        let p = func.IndexOf('(')
        if p>0 then
            func.Substring(0,p)
        else
            func

    let r_paren_pos(exp:string, pos:int) = 
        let mutable count = (1,0)
        let mutable p = 0
        for i=pos+1 to exp.Length-1 do
            let c = exp.Chars(i)
            if fst count <> snd count then
                if c.ToString().EndsWith("(") then count <- fst count+1,snd count
                else if c.ToString().EndsWith(")") then
                    count <- fst count,snd count+1
                    p<-i
        p
    
    let func_name(exp: string, pos: int) =
        let rec loop(i:int, exp:string, acc:string) =
            if i = 0 then
                String.rev acc
            else
                let c = exp.Chars(i-1)
                if valid_func_char(c) then
                    loop (i - 1, exp,acc+c.ToString())
                else loop ( 0, exp,acc)
        loop(pos, exp, "")

    //seq<func * parms * lp_pos * rp_pos * c>
    let explode(exp: string) = 
        let c=ref 0
        seq { for i in 0 .. exp.Length-1 do 
                if exp.Chars(i).ToString().EndsWith("(") then
                    c := !c+1
                    let rp = r_paren_pos(exp, i)
                    yield (func_name(exp,i),exp.Substring(i, rp-i+1), i, rp, c.Value) }

    let replace_parms(parm: string, new_parm: string) =
        let p = parm.Substring(1, parm.Length-2).Split(',')
        let rec loop i acc:string =
            if i >= p.Length then acc
            else loop (i + 1) (acc.Replace( "n" + (i+1).ToString(), p.[i]))
        loop 0 new_parm 

    let replace_func((a,b): (string * string), funcs: List<(string*string)>) =
        let x = ref (a+b)
        let mutable replace = false
        for f in funcs do
            let newf= fst f |> remove_parens
            if  newf = a then 
                x := "(" + replace_parms(b,snd f) + ")"
                replace <- true
        (x.Value,replace)

    let replace(exp:string, old_f:string,new_f:string) = exp.Replace(old_f,new_f)

    let parse(exp:string) = exp |> clean |> explode |> Seq.sortBy(fun (_,_,_,_,a) -> -a-1) // sort desc to get innermost func
    
    let private reduce(exp: string, funcs:List<(string*string)>) = 
        let mutable new_exp = exp
        let mutable replace = true
        while replace do
            replace <- false
            for (a,b,_,_,_) in parse(new_exp) do
                let (x,r) = replace_func((a,b), funcs)
                if r then
                    new_exp <- new_exp.Replace(a+b,x)
                    replace <- true
        new_exp

    let public ReduceFromList(exp: string, func: list<Entity.Expression>) = 
        let f = Seq.toList <| seq{for x in func do yield (x.Name,x.Value)}
        reduce(exp,f)
    
    let public ReduceFromSeq(exp: string, func: seq<Entity.Expression>) = 
        let f = Seq.toList <| seq{for x in func do yield (x.Name,x.Value)}
        reduce(exp,f)
////pass
    //Reducer.reduce("d(1,2)",[("x(n1)","n1");("d(n1,n2)","n1 + n2");("doji(n1)","o(n1)=c(n1)")]) //basic
//Reducer.reduce("do(1)+ds(1)",[("d(n1)","n1");("doji(n1)","o(n1)=c(n1)")]) //no replace
//Reducer.reduce("d(1,x(2))",[("x(n1)","n1");("d(n1,n2)","n1 + n2");("doji(n1)","o(n1)=c(n1)")]) //f in f
//Reducer.reduce("x(1,2)",[("z(n1)","n1+n1");("y(n1)","z(n1)");("x(n1,n2)","y(n1) + y(n2)");("doji(n1)","o(n1)=c(n1)")]) //uf in uf in uf
//
////fail
//Reducer.reduce("d(x(d(1,2)),3)",[("x(n1)","n1");("d(n1,n2)","n1 + n2");("doji(n1)","o(n1)=c(n1)")]) //self ref func
//
//
////other
//Reducer.remove_parens("doji(1)")
//Reducer.func_name("a+abc(1)",5)
//Reducer.explode "doji(1,x(2),3)+d(2)" |> Seq.sortBy(fun (_,_,_,_,a) -> -a-1) //uniary minus -- sort desc
//Reducer.r_paren_pos("x(1)",2)
//Reducer.replace_parms("(1)","o(n1)=c(n1)")
//Reducer.replace_parms("(1,2,x(3))","n1+n2-n3")
//Reducer.replace_parms("(y(1),2,x(3))","n1+n2-n3")
//Reducer.replace_parms("(a(1,2))","n1+n2")
//


