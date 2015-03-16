// Signature file for parser generated by fsyacc
module Parser
type token = 
  | EOF
  | COMMA
  | APOS
  | LPAREN
  | RPAREN
  | IF
  | AMP
  | PIPE
  | AND
  | OR
  | COUNT
  | FOR
  | INCR
  | VAR
  | OPEN
  | HIGH
  | LOW
  | CLOSE
  | VOLUME
  | SYMBOL
  | DATE
  | POW
  | ASTER
  | SLASH
  | SQRT
  | LOG
  | MOD
  | MAX
  | MIN
  | SMA
  | ABS
  | STD
  | PLUS
  | MINUS
  | GT
  | GE
  | EQ
  | LT
  | LE
  | NE
  | PARSEREXCEPTION of (string)
  | LEXEREXCEPTION of (string)
  | TEXT of (string)
  | FLOAT of (System.Double)
  | INT32 of (System.Int32)
type tokenId = 
    | TOKEN_EOF
    | TOKEN_COMMA
    | TOKEN_APOS
    | TOKEN_LPAREN
    | TOKEN_RPAREN
    | TOKEN_IF
    | TOKEN_AMP
    | TOKEN_PIPE
    | TOKEN_AND
    | TOKEN_OR
    | TOKEN_COUNT
    | TOKEN_FOR
    | TOKEN_INCR
    | TOKEN_VAR
    | TOKEN_OPEN
    | TOKEN_HIGH
    | TOKEN_LOW
    | TOKEN_CLOSE
    | TOKEN_VOLUME
    | TOKEN_SYMBOL
    | TOKEN_DATE
    | TOKEN_POW
    | TOKEN_ASTER
    | TOKEN_SLASH
    | TOKEN_SQRT
    | TOKEN_LOG
    | TOKEN_MOD
    | TOKEN_MAX
    | TOKEN_MIN
    | TOKEN_SMA
    | TOKEN_ABS
    | TOKEN_STD
    | TOKEN_PLUS
    | TOKEN_MINUS
    | TOKEN_GT
    | TOKEN_GE
    | TOKEN_EQ
    | TOKEN_LT
    | TOKEN_LE
    | TOKEN_NE
    | TOKEN_PARSEREXCEPTION
    | TOKEN_LEXEREXCEPTION
    | TOKEN_TEXT
    | TOKEN_FLOAT
    | TOKEN_INT32
    | TOKEN_end_of_input
    | TOKEN_error
type nonTerminalId = 
    | NONTERM__startstart
    | NONTERM_start
    | NONTERM_Prog
    | NONTERM_CommaList
    | NONTERM_Expr
    | NONTERM_Term
    | NONTERM_Factor
/// This function maps integers indexes to symbolic token ids
val tagOfToken: token -> int

/// This function maps integers indexes to symbolic token ids
val tokenTagToTokenId: int -> tokenId

/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production
val prodIdxToNonTerminal: int -> nonTerminalId

/// This function gets the name of a token as a string
val token_to_string: token -> string
val start : (Microsoft.FSharp.Text.Lexing.LexBuffer<'cty> -> token) -> Microsoft.FSharp.Text.Lexing.LexBuffer<'cty> -> ( Ast.Equation ) 