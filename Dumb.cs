using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

///FULVIO
namespace Rsx
{
   

    public static  partial class Dumb
    {
        public static string GetNextName(string prefix, IList<string> items, bool putCaps)
        {
            int actualMeasCount = items.Count;

            string nextMeas = prefix + Number2String(actualMeasCount, putCaps);
            while (items.Contains(nextMeas))
            {
                actualMeasCount++;
                nextMeas = prefix + Number2String(actualMeasCount, putCaps);
            }

            return nextMeas;
        }

        public static double Parse(string Mdens, double val)
        {
            double ro = val;
            double.TryParse(Mdens, out ro);
            if (double.IsNaN(ro)) ro = val;
            return ro;
        }

        public static String Number2String(int number, bool isCaps)
        {
            int newchar = (isCaps ? 65 : 97) + (number);

            Char c = (Char)(newchar);
            return c.ToString();
        }



        /// <summary>
        /// Forces Dispose = Function to delete IDisposable objects!
        /// </summary>
        /// <typeparam name="T">type of IDisposable object</typeparam>
        /// <param name="objeto">object to disposable that implements IDisposable</param>
        public static void FD<T>(ref T objeto)
        {
            object o = objeto;
            if (o == null) return;
            Type t = typeof(T);
            if (t.Equals(typeof(DataTable)))
            {
                DataTable table = (DataTable)o;
                table.Clear();
            }
            else if (t.Equals(typeof(DataSet)))
            {
                DataSet set = (DataSet)o;
                set.Clear();

            }
            
            IDisposable disposable = (IDisposable)o;
            disposable.Dispose();
            disposable = null;
        }

        public static bool IsUpper(string value)
        {
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\d", "");
            if (String.IsNullOrEmpty(value)) return false;
            IEnumerable<char> chars = value.AsEnumerable<char>();
            chars = chars.Where(c => char.IsLower(c));
            if (chars.Count() == 0) return true;
            else return false;
        }

        public static bool IsLower(string value)
        {
            // Consider string to be lowercase if it has no uppercase letters.
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\d", "");
            if (String.IsNullOrEmpty(value)) return false;
            IEnumerable<char> chars = value.AsEnumerable<char>();
            chars = chars.Where(c => char.IsUpper(c));
            if (chars.Count() == 0) return true;
            else return false;
        }

        public static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Days > 0 ? string.Format("{0:0}d", span.Days) : string.Empty,
                span.Hours > 0 ? string.Format("{0:0}h", span.Hours) : string.Empty,
                span.Minutes > 0 ? string.Format("{0:0}m", span.Minutes) : string.Empty,
                span.Seconds > 0 ? string.Format("{0:0}s", span.Seconds) : string.Empty);

            return formatted;
        }

        public static TimeSpan ToReadableTimeSpan(string formattedTS)
        {
            TimeSpan span = new TimeSpan(0, 0, 0, 0);
            int days = 0;
            int h = 0;
            int m = 0;
            int s = 0;
            string[] arr = null;
            string auxiliar = formattedTS.ToUpper();

            if (auxiliar.Contains('D'))
            {
                arr = auxiliar.Split('D');
                days = Convert.ToInt32(arr[0]);
                auxiliar = arr[1];
            }
            if (auxiliar.Contains('H'))
            {
                arr = auxiliar.Split('H');
                h = Convert.ToInt32(arr[0]);
                auxiliar = arr[1];
            }
            if (auxiliar.Contains('M'))
            {
                arr = auxiliar.Split('M');
                m = Convert.ToInt32(arr[0]);
                auxiliar = arr[1];
            }

            if (auxiliar.Contains('S'))
            {
                auxiliar = auxiliar.Replace('S', ' ').Trim();
                s = Convert.ToInt32(auxiliar);
            }

            return new TimeSpan(days, h, m, s);
        }

        /// <returns>a HashSet</returns>
    }
}