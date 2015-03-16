

type x =
    | A of string
    | B of string * int
    | C of z

and z =
    | A of string

type abc = 
    {name : string;
    desc: string}

//inline allows multiple types to get passed in
let inline add a b = a + b

add 1.0 2.0
add 1 2

//fhsarp extensions using with
type System.Double with
    static member Pi = 3.141

System.Double.Pi


//comnputational builder--http://fsharpforfunandprofit.com/posts/computation-expressions-intro/
type LoggingBuilder() =
    let log p = printfn "expression is %A" p

    member this.Bind(x, f) = 
        log x
        f x

    member this.Return(x) = 
        x

let loggedWorkflow = 
    let logger = new LoggingBuilder()
    logger
        {
        let! x = 42
        let! y = 43
        let! z = x + y
        return z
        }

//maybe monad
let divideBy bottom top =
    if bottom = 0
    then None
    else Some(top/bottom)

type MaybeBuilder() =

    member this.Bind(x, f) = 
        match x with
        | None -> None
        | Some a -> f a

    member this.Return(x) = 
        Some x

let divideByWorkflow init x y z = 
    let maybe = new MaybeBuilder()
    maybe 
        {
        let! a = init |> divideBy x
        let! b = a |> divideBy y
        let! c = b |> divideBy z
        return c
        }  


