#light 

//etype
type vector = {x: float;y: float}
let v1 = {x=2.2;y=3.3}
let v2 = {v1 with x=1.1; y=3.3} // type define from another defined type

// fun applied to 2
(fun x -> x + x ) 2

//non-recurse pow funct
let pw4 x =
    let sr x = x * x
    sr (sr x) 

//recursive pow func
let rec pow n m =
    if m=0 then 1 else n * pow n (m-1)

//Op evaluator
let Eval(a:'a, op:string) =
    let andACC acc x = acc && x
    let orACC acc x = acc || x
    match op with
    | "AND" -> List.fold andACC true a
    | "OR" -> List.fold orACC true a

//using tre,false list
Eval( [true;true], "AND");;

//Op eval using functions
let x = (1>1)
let y = (1<2)
Eval( [x; y], "OR");;

//candle evaluation
let ct( day:array<float>) = 
    if day.[0] = day.[3] then "doji" else
         if day.[0] < day.[3] then "white"  else
            "black"

//stock ohlc 
type stock = {mutable symbol: string;
             openPrice: double;
             highPrice: double;
             lowPrice: double;
             closePrice: double;
             volume: int}
     
let day = [|12.1;11.5;11.1;11.2|]
let days = [| [| 1.0;2.01;3.2; 0.0 |]; [|0.0; 1.0; 2.2; 3.2 |] |]
let scan = [| {symbol="spy"; openPrice=10.1; highPrice=0.1; lowPrice=2.2;closePrice=3.0; volume=1000} |]
let scans = Array.append([|{symbol="spy"; openPrice=10.1; highPrice=0.1; lowPrice=2.2;closePrice=3.0; volume=1000}|] )
let values = days |> Array.map(fun x -> ct(x))

//named subpatterns
let rec pairs = function
    | h1::(h2::_ as t) -> [|h1,h2|] :: pairs t
    | _ -> [];
//pairs [1;2;3] = [(1, 2); (2, 3)]

//gaurded subpatterns
let rec pos = function
    | [] ->[]
    | h::t when h < 0 -> pos t
    | h::t -> h::pos t

//pos [-3;2;1;-4];; = [2;1]

//or patterns
let is_sign =  function
    | -1 | 0 |1 -> true
    | _ -> false

//normalize 2d vector -http://www.fundza.com/vectors/normalize/index.html
let norm (x,y) = 
    match sqrt(x*x + y*y) with
        | 0.0 -> 0.0,0.0
        | s -> x/s,y/s

//patern matching
let (|Even|Odd|) n =
    match n % 2 with
        | 0 -> Even
        | _ -> Odd 

let print_match n =
    match n with
    | Even -> printfn "even"
    | Odd -> printfn "odd"
    
    
//seq
seq{for i in 1..10 -> if i < 5 then i else 0}   

//curried functions
let rec ipow n x = 
    match n with    
    | 0 -> 1.0
    | n -> x * ipow (n-1) x
    
let square = ipow 2;
let cube = ipow 3;
//cube 3;; = 27

//high order function - numerical approx
let epsilon_float = System.Double.Epsilon

let d (f: float -> float) x =
    let dx = sqrt epsilon_float
    (f (x + dx) - f (x - dx)) / (2.0 * dx)

let f x = x ** 3.0 - x - 1.0
//d f 2.0;;

//Haskell Iterate--yield returns 1 item, while yield! returns a concatenated list of elements
let rec iterate f value = seq { 
  yield value
  yield! iterate f (f value) }

// Returns: seq [1; 2; 4; 8; ...]
Seq.take 10 (iterate ((*)2) 1)

// compose a list of functions fs into a single function
let compose fs = List.reduce (>>) fs

// apply list of functions to an initial arg
let fs = [(*) 2; (+) 7; (*) 3; (+) 3]
compose fs 3
// = 3 |> ((*)2 >> (+)7 >> (*)3 >> (+) 3)
// = 3 |> (*)2 |> (+)7 |> (*)3 |> (+) 3
// = (((3 * 2) + 7) * 3) + 3
// = 42

//apply weight valued to array values and sum the value
let ApplyWeight (weights:float[], arrs:float[][]) = 
    Array.map2 (fun w -> Array.map ((*) w)) weights arrs 
    //|> Array.reduce (Array.map2 (+))

let w = [|0.6;0.3;0.1|]

let a = [| [|0.0453;0.065345;0.07566;1.562;356.6|] ; 
           [|0.0873;0.075565;0.07666;1.562222;3.66|] ; 
           [|0.06753;0.075675;0.04566;1.452;3.4556|] |]

ApplyWeight(w,a);;

//
//// Create a new variable 'x'
//let arg = Var.Global("x", typeof<Foo>)
//// Use Reflection to get information about the 'Prop' member
//let propInfo = typeof<Foo>.GetProperty("Prop")
//// Create a lambda 'fun x -> x.Prop'
//let e = Expr.Lambda(arg, Expr.PropertyGet(Expr.Var(arg), propInfo))
//
//let IsWhiteCandle(openPrice:double, closePrice:double) = openPrice < closePrice
//
////let reducefunc (k,(vs:seq)) = k, vs |> Seq.sum |> Seq.singleton
////let newValues(values) = values |> Seq.cast<Mongo.Stock> //|> Seq.find(fun(i) -> i.Ticker = ticker)
//let desc = (fun() -> "1+1")()
    