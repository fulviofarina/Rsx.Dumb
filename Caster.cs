using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

///FULVIO
namespace Rsx.Dumb
{
   
        public static  class Caster
        {

        public static bool IsTimeDividerOk(string timeDivider)
        {
            bool ok = timeDivider.Contains("m");
            ok = ok || timeDivider.Contains("d");
            ok = ok || timeDivider.Contains("y");
            ok = ok || timeDivider.Contains("h");
            ok = ok || timeDivider.Contains("s");
            return ok;
        }
        public static string GetTimeDividerSuffix(string timeDivider)
        {
            string suffix = "seconds";
            if (timeDivider.Contains("s")) suffix = "seconds";
            else if (timeDivider.Contains("y")) suffix = "years";
            else if (timeDivider.Contains("d")) suffix = "days";
            else if (timeDivider.Contains("h")) suffix = "hours";
            else if (timeDivider.Contains("m")) suffix = "minutes";
            return suffix;
        }
        public static double GetTimeFactor(string timeDivider = "m")
        {
            double factor = 1;
            if (timeDivider.CompareTo("s") == 0)
            {
                factor = 1;

            }
            else if (timeDivider.CompareTo("m") == 0)
            {
                factor = 60;

            }
            else if (timeDivider.CompareTo("h") == 0)
            {
                factor = 60 * 60;
            }
            else if (timeDivider.CompareTo("d") == 0)
            {
                factor = 60 * 60 * 24.0000006;
            }
            else if (timeDivider.CompareTo("y") == 0)
            {
                factor = 60 * 60 * 24.0000006 * 365.2422;
            }

            return factor;
        }
        public static IEnumerable<T> Cast<T>(DataView view)
            {
                if (view == null) throw new Exception("DataView is null, Cannot Cast<T>");

                IEnumerable<DataRowView> vs = view.OfType<DataRowView>();
                return Cast<T>(vs);
            }

            public static IEnumerable<T> Cast<T>(IEnumerable<DataRowView> views)
            {
                if (views == null) throw new Exception("DataRowViews are null, Cannot IEnum<T> Cast<T>");
                return views.Select(o => o.Row).Cast<T>();
            }

            /// <summary>
            /// Cast to a T generic type (normally a DataRow)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="view"></param>
            /// <returns></returns>
            public static T Cast<T>(DataRowView view)
            {
                if (view == null) throw new Exception("DataRowView is null, Cannot T Cast<T>");
                return (T)(object)view.Row;
            }

            public static IEnumerable<T> Cast<T>(DataGridView dgv)
            {
                IEnumerable<DataGridViewRow> vs = dgv.Rows.OfType<DataGridViewRow>();
                return Cast<T>(vs);
            }

            /// <summary>
            /// Cast to a IEnumerable of generic type (normally a IEnumerable of DataRow)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="grids"></param>
            /// <returns></returns>
            public static IEnumerable<T> Cast<T>(IEnumerable<DataGridViewRow> grids)
            {
                Type tipo = typeof(T);
                IEnumerable<DataRowView> views = grids.Select(o => o.DataBoundItem as DataRowView);
                if (tipo.Equals(typeof(DataRowView))) return (IEnumerable<T>)views;
                else return Cast<T>(views);
            }

            /// <summary>
            /// Cast to a T generic type (normally a DataRow)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="view"></param>
            /// <returns></returns>
            public static T Cast<T>(DataGridViewRow view)
            {
                Type tipo = typeof(T);
                object aux = null;
                if (view == null) return (T)aux;
                DataRowView v = view.DataBoundItem as DataRowView;
                if (v == null) return (T)aux;
                if (tipo.Equals(typeof(DataRowView)))
                {
                    aux = v;
                    return (T)aux;
                }
                else if (tipo.BaseType.Equals(typeof(DataRow)))
                {
                    return Cast<T>(v);
                }
                else if (tipo.Equals(typeof(DataRow)))
                {
                    return Cast<T>(v);
                }
                else throw new ArgumentException("not implemented");
            }

            /// <summary>
            /// Cast any object of type DataGridViewRow or DataRowView to T
            /// </summary>
            /// <typeparam name="T">Generic output, normally a DataRow</typeparam>
            /// <param name="view">A DataGridViewRow or a DataRowView</param>
            /// <returns></returns>
            public static T Cast<T>(object view)
            {
                if (view == null) throw new ArgumentException("nothing to cast?");
                Type tipo = view.GetType();
                if (tipo.Equals(typeof(DataGridViewRow))) return Cast<T>(view as DataGridViewRow);
                else if (tipo.Equals(typeof(DataRowView))) return Cast<T>(view as DataRowView);
                else throw new ArgumentException("not implemented");
            }

            public static IEnumerable<T> Cast<T>(IEnumerable<object> view)
            {
                if (view == null) throw new ArgumentException("nothing to cast?");
                Type tipo = view.GetType();

                if (tipo.Equals(typeof(DataView))) return Cast<T>(view as DataView);
                else if (tipo.Equals(typeof(DataGridView))) return Cast<T>(view as DataGridView);
                else throw new ArgumentException("not implemented");
            }
        }
    
}