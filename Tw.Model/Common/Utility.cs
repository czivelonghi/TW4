using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tw.Model.Comon.Utility
{
    public class Text
    {
        public static string Pad(object value, int width, bool right = true)
        {
            if (right)
                return ((value != null) ? value.ToString() : "").PadRight(width);
            else
                return ((value != null) ? value.ToString() : "").PadLeft(width);
        }
    }

    public class Date
    {

        public static string ToString(DateTime date)
        {
            return date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00");
        }

        public static int ToInt(DateTime date)
        {
            return Int32.Parse((date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00")));
        }

        public static DateTime ToDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        public static DateTime ToDate(int date)
        {
            return DateTime.ParseExact(date.ToString(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        public static int DateAdd(int date, int days)
        {
            var d = ToDate(date).AddDays(days);
            return ToInt(d);
        }

        private static int CalcWeekDays(int period)
        {
            decimal weeks = 1;
            int totalperiods = period;

            if (period > 5)
            {
                weeks = period / 5;
                totalperiods = period + ((int)Math.Ceiling(weeks) * 2);
            }

            return totalperiods;
        }

        private static int CalcStartDate(int period)
        {
            DateTime date = DateTime.Today;

            var startdate = date.AddDays(-CalcWeekDays(period));

            return int.Parse(startdate.ToString("yyyyMMdd"));
        }

        public static int CalcStartDate(int period, int enddate)
        {

            DateTime date = ToDate(enddate.ToString());
            var newdate = date.AddDays(-CalcWeekDays(period));
            return int.Parse(newdate.ToString("yyyyMMdd"));
        }


        public static int PeriodsBetween(int startdate, int enddate)
        {
            DateTime edate = ToDate(enddate.ToString());
            DateTime sdate = ToDate(startdate.ToString());
            return Int32.Parse((edate - sdate).TotalDays.ToString());
        }

    }
}
