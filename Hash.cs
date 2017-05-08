using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

///FULVIO
namespace Rsx.Dumb
{
    public static partial class Hash
    {
        public static String[] ArrayFromColum(DataColumn Column)
        {
            System.Converter<DataRow, String> convi = delegate (DataRow row) { return Convert.ToString(row[Column.ColumnName]); };
            String[] rowValuesForColumn = Array.ConvertAll<DataRow, String>(Column.Table.Select(), convi);
            return rowValuesForColumn;
        }

        public static string GetRowAsString(ref DataRow r)
        {
            string content = string.Empty;
            IEnumerable<DataColumn> cols = r.Table.Columns.OfType<DataColumn>().ToList();
            foreach (System.Data.DataColumn c in cols)
            {
                try
                {
                    content += c.ColumnName + ":\t\t" + r[c] + "\n";
                }
                catch (SystemException ex)
                {
                    r.SetColumnError(c, ex.Message);
                }
            }
            return content;
        }

        /// <summary>
        /// Creates a non-repeated list of items (HashSet) from values in a given DataTable's Column
        /// </summary>
        /// <typeparam name="T">the type of desired elements in the output list</typeparam>
        /// <param name="column">input Column with items to generate the list from</param>
        /// <returns>a HashSet with elements of generic type</returns>
        public static IList<T> HashFrom<T>(IEnumerable<DataRow> array, String Field)
        {
            if (array == null) throw new ArgumentException("arra is null", "array");

            IEnumerable<object> array2 = EC.NotNulls(array, Field).Select(o => o.Field<object>(Field));

            Func<object, T> converter = o => (T)Convert.ChangeType(o, typeof(T));

            IEnumerable<T> enumcol = array2.Distinct().Select<object, T>(converter);

            return enumcol.ToList<T>();
        }

        /// <summary>
        /// Creates a non-repeated list of items (HashSet) from values in a given DataTable's Column
        /// </summary>
        /// <typeparam name="T">the type of desired elements in the output list</typeparam>
        /// <param name="column">input Column with items to generate the list from</param>
        /// <returns>a HashSet with elements of generic type</returns>
        public static IList<T> HashFrom<T>(IEnumerable<DataRowView> array, String Field)
        {
            if (array == null) throw new ArgumentException("array is null", "array");
            IEnumerable<DataRow> array2 = array.Select(o => o.Row as DataRow);
            return HashFrom<T>(array2, Field);
        }

        public static IList<T> HashFrom<T>(IEnumerable<DataGridViewRow> array, String Field)
        {
            if (array == null) throw new ArgumentException("array is null", "array");
            IEnumerable<DataRowView> array2 = array.Select(o => o.DataBoundItem as DataRowView);
            return HashFrom<T>(array2, Field);
        }

        /// <summary>
        /// Creates a non-repeated list of items (HashSet) from values in a given DataTable's Column
        /// </summary>
        /// <typeparam name="T">the type of desired elements in the output list</typeparam>
        /// <param name="column">input Column with items to generate the list from</param>
        /// <returns>a HashSet with elements of generic type</returns>
        public static IList<T> HashFrom<T>(DataColumn column)
        {
            if (column.Table == null) throw new ArgumentException("column.Table is null", "column.Table");
            IEnumerable<DataRow> rows = column.Table.AsEnumerable();
            return HashFrom<T>(rows, column.ColumnName);
        }

        /// <summary>
        /// Creates a non-repeated list of items of the given Row array under the given ColumnName
        /// </summary>
        /// <typeparam name="T">type of HashSet elements</typeparam>
        /// <param name="Rows">      Row array to make list from</param>
        /// <param name="ColumnName">
        /// ColumnName of the Row array with items to generate the hash set from
        /// </param>
        /// <returns>a HashSet</returns>
        public static IList<T> HashFrom<T>(IEnumerable<DataRow> array, String Field, String FieldFilter, object FilterValue)
        {
            if (array == null) throw new ArgumentException("array is null", "array");

            IEnumerable<object> array2 = EC.NotNulls(array, Field).Where(i => i.Field<object>(FieldFilter).Equals(FilterValue)).Select(o => o.Field<object>(Field));

            Func<object, T> converter = o => (T)Convert.ChangeType(o, typeof(T));

            IEnumerable<T> enumcol = array2.Distinct().Select<object, T>(converter);

            return enumcol.ToList<T>();
        }

        /// <summary>
        /// Creates a non-repeated list of items from the given DataColumn where any occurrence of
        /// string "replace" has been replaced by string "by"
        /// </summary>
        /// <typeparam name="T">type of HashSet elements</typeparam>
        /// <param name="column"> Table DataColumn to make list from</param>
        /// <param name="replace">String to replace on each item in the list</param>
        /// <param name="by">     
        /// String to use as replacement on each item in the list when occurrence is found
        /// </param>
        /// <returns>a HashSet</returns>
        public static IList<T> HashFrom<T>(DataColumn column, T replace, T by)
        {
            if (column.Table == null) throw new ArgumentException("column.Table is null", "column.Table");
            string replace_ = replace.ToString();
            string by_ = by.ToString();
            var list = from i in column.Table.AsEnumerable()
                       where (i.RowState != DataRowState.Deleted && i.RowState != DataRowState.Detached && i.Field<object>(column) != null)
                       select i.Field<object>(column).ToString().Replace(replace_, by_);

            Func<object, T> converter = o => (T)Convert.ChangeType(o, typeof(T));

            IEnumerable<T> enumcol = list.Distinct().Select<object, T>(converter);
            return enumcol.ToList<T>();
        }

        /// <summary>
        /// Creates a non-repeated list of items from the given DataColumn
        /// </summary>
        /// <typeparam name="T">type of HashSet elements</typeparam>
        /// <param name="column">Table DataColumn to make list from</param>

        /// <returns>a HashSet</returns>
    }
}