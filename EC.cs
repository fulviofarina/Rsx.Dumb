using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

///FULVIO
namespace Rsx
{
    /// <summary> STATIC CLASS FOR CHECKING ROWS & CONTROLS FOR ERRORS </summary>
    public static partial class EC
    {
        public static IEnumerable<DataRow> NotNulls(IEnumerable<DataRow> array, String Field)
        {
            return array.Where(o => !IsNuDelDetch(o) && o.Field<object>(Field) != null);
        }

        public static bool HasErrors<T>(IEnumerable<T> rows)
        {
            if (GetRowsInError<T>(rows).Count() != 0) return true;
            else return false;
        }

        public static IEnumerable<T> Deleted<T>(IEnumerable<T> rows)
        {
            Func<T, bool> find = o =>
            {
                DataRow row = (object)o as DataRow;
                if (row.RowState == DataRowState.Deleted) return true;
                else return false;
            };
            return rows.Where<T>(find).ToList<T>();
        }

        /// <summary>
        /// Returns true if the row is either NULL, was Deleted or is Detached, else false
        /// </summary>
        /// <param name="row">DataRow to check</param>
        /// <returns></returns>
        public static bool IsNuDelDetch(DataRow row)
        {
            if (row == null || row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached) return true;
            else return false;
        }

        public static IEnumerable<T> NotDeleted<T>(IEnumerable<T> rows)
        {
            Func<T, bool> find = o =>
            {
                DataRow row = (object)o as DataRow;
                if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached) return true;
                else return false;
            };
            return rows.Where<T>(find).ToList<T>();
        }

        public static IEnumerable<T> GetRowsInError<T>(IEnumerable<T> rows)
        {
            Func<T, bool> find = s =>
            {
                bool errors = false;

                // if (s != null) {
                DataRow r = s as DataRow;

                if (!IsNuDelDetch(r))
                {
                    // if (r.RowState != DataRowState.Detached && r.RowState != DataRowState.Deleted) {
                    errors = r.HasErrors;
                    // }
                }

                // }
                return errors;
            };

            return rows.Where<T>(find).ToList<T>();
        }

        public static void SetError(Control control, ErrorProvider error, string message)
        {
            error.SetError(control, error.GetError(control) + message);
        }

        /// <summary>
        /// Puts Null Error in the given column value of the given row (row,column)
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row">   </param>
        public static bool CheckNull(DataColumn column, DataRow row)
        {
            row.SetColumnError(column, null);

            Type t = column.DataType;

            if (t.Equals(typeof(double)))
            {
                if (row.IsNull(column) || Convert.ToDouble(row[column]) == 0)
                {
                    row.SetColumnError(column, "NULL!");
                }
            }
            else if (t.Equals(typeof(Int32)))
            {
                if (row.IsNull(column) || Convert.ToInt32(row[column]) == 0)
                {
                    row.SetColumnError(column, "NULL!");
                }
            }
            else if (t.Equals(typeof(String)))
            {
                if (row.IsNull(column) || Convert.ToString(row[column]).CompareTo(String.Empty) == 0)
                {
                    row.SetColumnError(column, "NULL!");
                }
            }
            else if (t.Equals(typeof(DateTime)))
            {
                if (row.IsNull(column) && !column.ReadOnly)
                {
                    row[column] = DateTime.Now;
                }
            }
            else if (t.Equals(typeof(bool)))
            {
                if (row.IsNull(column) && !column.ReadOnly) row[column] = false;
            }
            else if (t.Equals(typeof(byte[])))
            {
                if (row.IsNull(column)) row.SetColumnError(column, "NULL!");
            }
            string error = row.GetColumnError(column);

            return !String.IsNullOrEmpty(error);
        }

        public static void CheckRowColumn(DataRowChangeEventArgs e, DataColumnChangeEventHandler columnChecker)
        {
            if (e.Action == System.Data.DataRowAction.Add || e.Action == System.Data.DataRowAction.Change)
            {
                if (e.Row.RowState != System.Data.DataRowState.Deleted)
                {
                    DataColumn currentcol = null;
                    DataColumnChangeEventArgs args = null;

                    foreach (System.Data.DataColumn col in e.Row.Table.Columns)
                    {
                        currentcol = col;
                        args = new System.Data.DataColumnChangeEventArgs(e.Row, currentcol, e.Row[currentcol]);
                        columnChecker(null, args);
                    }
                }
            }
        }

        /// <summary>
        /// Forces the check-up of a Data Row according to the DataColumnChangeEventHandler given
        /// </summary>
        /// <typeparam name="T">DatTable, DataRow[] or DataRow</typeparam>
        /// <param name="table2">       object to check</param>
        /// <param name="columnChecker">DataColumnChangeEventHandler to use for checking</param>
        public static void CheckRows<T>(T table2, DataColumnChangeEventHandler columnChecker)
        {
            Type tipo = typeof(T);

            if (tipo.Equals(typeof(DataTable)))
            {
                DataTable dt = table2 as DataTable;

                DataRow[] rows = dt.Rows.OfType<DataRow>().ToArray();

                CheckRows(rows, columnChecker);
            }
            else if (tipo.Equals(typeof(DataRow[])))
            {
                IEnumerable<DataRow> rows = table2 as IEnumerable<DataRow>;

                foreach (DataRow r in rows)
                {
                    CheckRows(r, columnChecker);
                }
            }
            else if (tipo.Equals(typeof(DataRow)))
            {
                DataRow r = table2 as DataRow;
                DataRow currentRow = null;
                currentRow = r;
                DataRowChangeEventArgs args = new DataRowChangeEventArgs(currentRow, DataRowAction.Change);

                if (currentRow.RowState == DataRowState.Unchanged) currentRow.SetModified();
                EC.CheckRowColumn(args, columnChecker);
                currentRow.AcceptChanges();
            }
            else
            {
                NotImplemented();
            }
        }

        public static void SetRowError(DataRow row, DataColumn column, Exception ex)
        {
            // if (IsNuDelDetch(row)) return; if (column == null) return;

            row.SetColumnError(column, ExceptionMsg(ex));
        }

        public static void SetRowError(DataRow row, Exception ex)
        {
            // if (IsNuDelDetch(row)) return;

            row.RowError = ExceptionMsg(ex);
        }

        /// <summary>
        /// Throws a SystemException with the "Not implemented" message
        /// </summary>
        public static void NotImplemented()
        {
            throw new Exception("Not implemented");
        }

        /// <summary>
        /// Returns the message I would like to get from an Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ExceptionMsg(Exception ex)

        {
            if (ex == null) return "Exception sent was Empty";
            else return ex.Message + "\n\n" + ex.StackTrace;
        }
    }
}