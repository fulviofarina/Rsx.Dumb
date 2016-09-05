using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

///FULVIO
namespace Rsx
{
    public partial class Dumb
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
            if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
            {
                return false;
            }
            else if (row == null || row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached) return true;
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
                if (s != null)
                {
                    DataRow r = s as DataRow;
                    if (r.RowState != DataRowState.Detached && r.RowState != DataRowState.Deleted)
                    {
                        errors = r.HasErrors;
                    }
                }
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
        /// <param name="row"></param>
        public static bool CheckNull(DataColumn column, DataRow row)
        {
            row.SetColumnError(column, null);

            Type t = column.DataType;

            if (t.Equals(typeof(double)))
            {
                if (row.IsNull(column) || Convert.ToDouble(row[column]) == 0)
                {
                    row.SetColumnError(column, "NULL. Not good!");
                }
            }
            else if (t.Equals(typeof(Int32)))
            {
                if (row.IsNull(column) || Convert.ToInt32(row[column]) == 0)
                {
                    row.SetColumnError(column, "NULL. Not good!");
                }
            }
            else if (t.Equals(typeof(String)))
            {
                if (row.IsNull(column) || Convert.ToString(row[column]).CompareTo(String.Empty) == 0)
                {
                    row.SetColumnError(column, "NULL. Not good!");
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

        public static void CheckRows(DataTable table2, DataColumnChangeEventHandler columnChecker)
        {
            DataRow currentRow = null;
            DataRowChangeEventArgs args = null;

            foreach (DataRow r in table2.Rows)
            {
                currentRow = r;
                if (currentRow.RowState == DataRowState.Unchanged) currentRow.SetModified();
                args = new DataRowChangeEventArgs(currentRow, DataRowAction.Change);

                Dumb.CheckRowColumn(args, columnChecker);
                currentRow.AcceptChanges();
            }
        }

        public static void CheckRows(DataRow[] rows, DataColumnChangeEventHandler columnChecker)
        {
            DataRow currentRow = null;
            DataRowChangeEventArgs args = null;
            foreach (DataRow r in rows)
            {
                currentRow = r;
                args = new DataRowChangeEventArgs(currentRow, DataRowAction.Change);

                if (currentRow.RowState == DataRowState.Unchanged) currentRow.SetModified();
                Dumb.CheckRowColumn(args, columnChecker);
                currentRow.AcceptChanges();
            }
        }

        public static void SetRowError(DataRow row, DataColumn column, SystemException ex)
        {
            try
            {
                row.SetColumnError(column, ex.Message + "\n\n" + ex.StackTrace);
            }
            catch (SystemException nada)
            {
            }
        }

        public static void SetRowError(DataRow row, SystemException ex)
        {
            try
            {
                row.RowError = ex.Message + "\n\n" + ex.StackTrace;
            }
            catch (SystemException nada)
            {
            }
        }
    }
}