using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

///FULVIO
namespace Rsx.Dumb
{
   
        public static partial class BS
        {
            public static void ChangeBindingsFormat(string newFormat, ref Hashtable binds)
            {
                ICollection cools = binds.Values;
                foreach (var item in cools)
                {
                    Binding b = item as Binding;
                    b.FormatString = newFormat;
                }
            }

            /// <summary>
            /// Returns a HashTable with "Text" Bindings, where the key is the DataTableColumn.ColumnName
            /// of the bound member
            /// </summary>
            /// <param name="bs"></param>
            /// <returns></returns>
            public static Hashtable ArrayOfBindings(ref BindingSource bs, string format = "", string Property = "Text")
            {
                DataTable dt = (bs.DataSource as DataSet).Tables[bs.DataMember];
                Hashtable bslist = new Hashtable();

                foreach (DataColumn item in (dt as DataTable).Columns)
                {
                    string column = item.ColumnName;
                    // column = Unit.DiameterColumn.ColumnName;
                    Binding b = ABinding(ref bs, column, format, Property);
                    bslist.Add(column, b);
                }

                return bslist;
            }

            public static Binding ABinding(ref BindingSource bs, string column, string format = "", string Property = "Text")
            {
                DataSourceUpdateMode mo = DataSourceUpdateMode.OnPropertyChanged;
                bool t = true;
                // format = string.Empty; string text = "Text";
                Binding b = new Binding(Property, bs, column, t, mo, DBNull.Value, format);
                b.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
                return b;
            }

            public static void LinkBS(ref BindingSource BS, DataTable table)
            {
                if (table == null) throw new ArgumentException("table is null", "table");
                if (BS == null) throw new ArgumentException("BindingSource is null", "BS");

                BS.EndEdit();
                BS.SuspendBinding();
                if (table.DataSet != null)
                {
                    //first the datamemember otherwise it is scanned twice!!
                    //important wisdom
                    BS.DataMember = table.TableName;
                    BS.DataSource = table.DataSet;
                }
                else BS.DataSource = table;
                BS.ResumeBinding();
            }

            public static void LinkBS(ref BindingSource BS, DataTable table, String Filter, String Sort)
            {
                LinkBS(ref BS, table);
                BS.EndEdit();
                BS.SuspendBinding();

                BS.Filter = Filter;
                BS.Sort = Sort;

                BS.ResumeBinding();
            }

            public static string[] DeLinkBS(ref BindingSource BS)
            {
                if (BS == null) throw new ArgumentException("BindingSource is null", "BS");

                BS.EndEdit();
                BS.SuspendBinding();
                string[] sortFilter = new string[] { BS.Sort, BS.Filter };

                BS.Sort = string.Empty;
                BS.Filter = string.Empty;
                //first datamember
                BS.DataMember = string.Empty;
                BS.DataSource = null;
                BS.ResumeBinding();
                return sortFilter;
            }
        }
    
}