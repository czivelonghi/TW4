using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Model.Comon.Utility;
using Tw.Model.Entity;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Tw.Model.View
{
    public class BackTestView : IView
    {
        public Entity.BackTest BackTest { get; set; }

        public List<Entity.Company> Company { get; set; }
        public List<Entity.Expression> Expression { get; set; }
        public List<Entity.Price> Price { get; set; }
        public List<Result.BackTestResult> Result { get; set; }
        public List<Entity.Error> Error { get; set; }
        public List<string> Debug { get; set; }
        public int DebugMode { get; set; }
        public List<string> Timing { get; set; }
        public bool Parallel { get; set; }
        public string ExportType { get; set; }

        public BackTestView()
        {
            Company = new List<Company>();
            Expression = new List<Expression>();
            Price = new List<Price>();
            Result = new List<Result.BackTestResult>();
            Error = new List<Entity.Error>();
            Debug = new List<string>();
            Timing = new List<string>();
        }

        public decimal AverageROI
        {
            get
            {
                return Result.Where(x=>x.ROI>0).Average(x=>x.ROI);
            }
        }

        public decimal HighROI
        {
            get {
                var results =  (Result.Count>0)? Result.Max(x => x.ROI) : 0;
                return results*100;
                }
        }

        public string LowROISymbol
        {
            get
            {
                var results = Result
                                .OrderBy(r => r.ROI)
                                .Take(1)
                                .Select(r => r.Company.Symbol);

                return results.First();

            }
        }

        public string HighROISymbol
        {
            get
            {
                var results = Result
                                .OrderByDescending(r=>r.ROI)
                                .Take(1)
                                .Select(r=> r.Company.Symbol);

                return results.First();
                
            }
        }

        public decimal LowROI
        {
            get{var results =  (Result.Count>0)?Result.Min(x => x.ROI):0;
                return results*100;
                }
        }

        //exporttype: 3 options
        //xls(xls format),csv(comma seperated),txt(text file),none(screen)
        void FreezeTopRow( Microsoft.Office.Interop.Excel._Worksheet ws)
        {
            ws.Activate();
            ws.Application.ActiveWindow.SplitRow = 1;
            ws.Application.ActiveWindow.FreezePanes = true;
        }

        public void Xls()
        {
            if ((this.ExportType != "xls") || (Result.Count==0)) return;

            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbooks = excelApp.Workbooks;

            //workbook template
            var book = workbooks.Add();//@"C:\MyTemplate.xltx");

            //Sheets objects for workbook
            var sheets = book.Sheets;
            var sheet = (Worksheet)sheets.get_Item("Sheet1");
            int row = 1;

            //alternate row
            //FormatCondition format = sheet.Rows.FormatConditions.Add(XlFormatConditionType.xlExpression, XlFormatConditionOperator.xlEqual, "=MOD(ROW(),2) = 0");
            //format.Interior.Color = XlRgbColor.rgbBisque;

            WriteCell(excelApp, row, 1, "company");
            WriteCell(excelApp, row, 2, "trades");
            WriteCell(excelApp, row, 3, "long w/l");
            WriteCell(excelApp, row, 4, "short w/l");
            WriteCell(excelApp, row, 5, "total w/l");

            WriteCell(excelApp, row, 6, "long w/l(%)");
            WriteCell(excelApp, row, 7, "short w/l(%)");
            //WriteCell(excelApp, row, 8, "total% w/l");

            WriteCell(excelApp, row, 8, "long/short exp($)");
            WriteCell(excelApp, row, 9, "commission");
            WriteCell(excelApp, row, 10, "balance");
            WriteCell(excelApp, row, 11, "roi");

            Result.OrderByDescending(x=>x.ROI);

            for (int i=0;i<Result.Count;i++)
            {
                var r = Result[i];
                row += 1;
                WriteCell(excelApp, row, 1, r.Company.Symbol);
                WriteCell(excelApp, row, 2, r.Trade.Count);
                WriteCell(excelApp, row, 3, r.LongWins.ToString() + "/" + r.LongLosses.ToString());
                WriteCell(excelApp, row, 4, r.ShortWins.ToString() + "/" + r.ShortLosses.ToString());
                WriteCell(excelApp, row, 5, r.TotalWins.ToString() + "/" + r.TotalLosses.ToString());

                WriteCell(excelApp, row, 6, r.LongWinPercent.ToString() + "/" + r.LongLossPercent.ToString());
                WriteCell(excelApp, row, 7, r.ShortWinPercent.ToString() + "/" + r.ShortLossPercent.ToString());

                WriteCell(excelApp, row, 8, r.LongExpectancy.ToString() + "/" + r.ShortExpectancy.ToString());
                WriteCell(excelApp, row, 9, r.TotalCommission);
                WriteCell(excelApp, row, 10, r.Balance);
                WriteCell(excelApp, row, 11, (r.ROI * 100) + "%");

                var trades = r.Trade.OrderBy(x => x.Date);
                int startrow = row;
                foreach (Entity.Trade t in trades)
                {
                    row += 1;
                    WriteCell(excelApp, row, 1, Comon.Utility.Date.ToDate(t.Date).ToShortDateString());
                    WriteCell(excelApp, row, 2, t.Description);
                    WriteCell(excelApp, row, 3, t.TradeType + " " + t.PositionType);
                    WriteCell(excelApp, row, 4, t.Price);
                    WriteCell(excelApp, row, 5, t.Shares);
                    WriteCell(excelApp, row, 6, t.Total);
                    WriteCell(excelApp, row, 7, t.Balance);
                    WriteCell(excelApp, row, 8, t.ID == null ? t.Balance.ToString() : "");
                    //WriteCell(excelApp, row, 8, BackTest.Debug == 1 ? t.Expression : "");
                }

                if(trades.Count()>0)
                    GroupRow(sheet, startrow+1, row);

            }

            FreezeTopRow(sheet);

            excelApp.Visible = true;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //cleanup
            Marshal.ReleaseComObject(sheet);
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(workbooks);
            Marshal.ReleaseComObject(excelApp);
        }

        void WriteCell(Microsoft.Office.Interop.Excel.Application app, int row, int col, object value)
        {
            app.Cells[row, col] = "\r" + value;
        }

        void GroupRow(Worksheet sheet, int startrow, int endrow)
        {
            var range = sheet.Rows[string.Format("{0}:{1}", startrow, endrow), Type.Missing] as Range;
            range.Name = "test" + startrow;
            range.Group(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            range.Rows.Hidden = true;
           
        }

        public string Export()
        {
            string output = "";



            return output;
        }

        public string Report()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Text.Pad("name: ", 10) + BackTest.Name);
            sb.AppendLine(Text.Pad("range: ", 10) + BackTest.PeriodStart + "-" + BackTest.PeriodEnd);
            sb.AppendLine(Text.Pad("capital: ", 10) + BackTest.Capital);
            sb.AppendLine(Text.Pad("company: ", 10) + Result.Count());
            sb.AppendLine(Text.Pad("error: ", 10) + Error.Count());
            sb.AppendLine();

            if (Error.Count > 0)
            {
                sb.AppendLine("error(s):");
                foreach (var e in Error)
                {
                    sb.AppendLine(e.Description);
                }
            }

            if (Result.Count > 0)
            {
                //trades
                sb.AppendLine();
                sb.AppendLine(Text.Pad("date", 10) +
                            Text.Pad("company", 10) +
                            Text.Pad("position", 15) +
                            Text.Pad("description", 15) +
                            Text.Pad("price", 10) +
                            Text.Pad("shares", 10) +
                            Text.Pad("total", 15) +
                            Text.Pad("balance", 15) +
                            Text.Pad("total value", 15) +
                            ((BackTest.Debug == 1) ? "expression" : ""));


                foreach (var r in Result)
                {
                    var trades = r.Trade.OrderBy(x => x.Date);
                    foreach (Entity.Trade t in trades)
                    {
                        sb.AppendLine(
                        Text.Pad(t.Date, 10) +
                        Text.Pad(t.Symbol, 10) +
                        Text.Pad(t.TradeType + " " + t.PositionType, 15) +
                        Text.Pad(t.Description, 15) +
                        Text.Pad(t.Price, 10) +
                        Text.Pad(t.Shares, 10) +
                        Text.Pad(t.Total, 15) +
                        Text.Pad(t.Balance, 15) +
                        Text.Pad(t.ID == null ? t.Balance.ToString() : "", 15) +
                        ((BackTest.Debug == 1) ? t.Expression : ""));
                    }
                }

                //summation
                sb.AppendLine();
                sb.AppendLine(Text.Pad("company", 10) +
                            Text.Pad("trades", 10) +
                            Text.Pad("long w/l", 12) +
                            Text.Pad("short w/l", 12) +
                            Text.Pad("total w/l", 12) +
                            Text.Pad("long% w/l", 12) +
                            Text.Pad("short% w/l", 12) +
                            Text.Pad("total% w/l", 12) +
                            Text.Pad("long/short$ exp.", 17) +
                            Text.Pad("commission", 12) +
                            Text.Pad("balance", 12) +
                            "roi"
                            );

                for (int i = 0; i < Result.Count(); i++)
                {
                    var r = Result[i];

                    sb.AppendLine(Text.Pad(r.Company.Symbol, 10) +
                                Text.Pad(r.Trade.Count, 10) +
                                Text.Pad(r.LongWins.ToString() + "/" + r.LongLosses.ToString(), 12) +
                                Text.Pad(r.ShortWins.ToString() + "/" + r.ShortLosses.ToString(), 12) +
                                Text.Pad(r.TotalWins.ToString() + "/" + r.TotalLosses.ToString(), 12) +
                                Text.Pad(r.LongWinPercent.ToString() + "/" + r.LongLossPercent.ToString(), 12) +
                                Text.Pad(r.ShortWinPercent.ToString() + "/" + r.ShortLossPercent.ToString(), 12) +
                                Text.Pad(r.TotalWins.ToString() + "/" + r.TotalLosses.ToString(), 12) +
                                Text.Pad(r.LongExpectancy.ToString() + "/" + r.ShortExpectancy.ToString(), 17) +
                                Text.Pad(r.TotalCommission, 12) +
                                Text.Pad(r.Balance, 12) +
                                (r.ROI * 100) + "%"
                                );
                }

                //totals goes here
                sb.AppendLine();
                sb.AppendLine("avg roi: " + AverageROI);
                sb.AppendLine("high/low roi: " + HighROI + "(" + HighROISymbol + ")/" + LowROI + "(" + LowROISymbol + ")");
            }

            return sb.ToString();
        }
    }
    
}
