module EODData

open System.IO
open System

module HistoricalFiles =
    let private dateToNum (date:string) =
        try
            let olddate = (DateTime.Parse date)
            olddate.Year.ToString() + olddate.Month.ToString("00") + olddate.Day.ToString("00")
        with 
            | e -> printfn "date error: %s" date |> reraise() 

    let private createImportRow (row:string) = 
        let values = row.Split([|','|])
        values.[0] + "," + dateToNum values.[1] + "," + values.[2] + "," + values.[3] + "," + values.[4] + "," + values.[5] + "," + values.[6]

    let private appendRow row destFile =
        if row <> "Symbol,Date,Open,High,Low,Close,Volume" && row <> "" then
            let newrow = createImportRow(row) + System.Environment.NewLine
            File.AppendAllText(destFile,newrow)
    
    let private mergeFile srcfile destFile =
        printfn "%s" srcfile
        File.ReadAllLines(srcfile) |> Seq.toList |> List.iter(fun x -> appendRow x destFile)

    let MergeDirectoryFiles(directory:string, destFile:string, includeHeader:bool) =
        if (includeHeader) then File.AppendAllText(destFile,"Symbol,Date,Open,High,Low,Close,Volume" + System.Environment.NewLine)
        Directory.GetFiles(directory) |> Seq.toList |> List.iter(fun f -> mergeFile f destFile)

HistoricalFiles.MergeDirectoryFiles(@"C:\Users\clark\Desktop\StockData\nyse_work", @"C:\Users\clark\Desktop\TradeWiki\TradeWiki\Data\Sqlite\nyse_19920101_20111013_no_head.csv", false);