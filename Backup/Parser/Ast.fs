namespace Ast
open System

type Factor =
    | Float   of Double
    | Integer of Int32
    | ParenEx of Expr
        
and Term =
    | Sqrt   of Factor
    | Log    of Factor 
    | O      of Factor
    | H      of Factor
    | L      of Factor
    | C      of Factor
    | V      of Factor
    | S      of Factor
    | D      of Factor
    | Min    of Expr * Expr
    | Max    of Expr * Expr
    | Sma    of Expr * string * Expr
    | Std    of string * Expr
    | Slope  of string * Expr * Expr
    | Pat1   of string * Expr
    | Pat2   of string * Expr * Expr
    | Pat3   of string * Expr * Expr * Expr
    | Candle of string * Expr
    | If     of Expr * Expr * Expr
    | And    of Factor * Expr
    | Or     of Factor * Expr
    | Count  of string * Factor * Expr
    | For    of Factor * Expr
    | Abs    of Expr
    | Incr   of string
    | Var    of string
    | Equals of string * string
    | Factor of Factor

and Expr =
    | Text  of String
    | Plus  of Expr * Term
    | Minus of Expr * Term
    | Pow   of Expr * Term
    | Times of Expr * Term
    | Divide of Expr * Term
    | Modulus of Expr * Term
    | Eq    of Expr * Expr
    | Gt    of Expr * Expr
    | Ge    of Expr * Expr
    | Lt    of Expr * Expr
    | Le    of Expr * Expr
    | Ne    of Expr * Expr
    | Amp   of Expr * Expr
    | Pipe  of Expr * Expr
    | Term  of Term
    
and Equation =
    | Equation of Expr