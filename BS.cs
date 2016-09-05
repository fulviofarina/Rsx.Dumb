using System;
using System.Data;
using System.Windows.Forms;

///FULVIO
namespace Rsx
{
    public partial class Dumb
    {
        public static void LinkBS(ref BindingSource BS, DataTable table)
        {
            if (table == null) throw new ArgumentException("table is null", "table");
            if (BS == null) throw new ArgumentException("BindingSource is null", "BS");

            BS.EndEdit();
            BS.SuspendBinding();
            if (table.DataSet != null)
            {
                BS.DataSource = table.DataSet;
                BS.DataMember = table.TableName;
            }
            else BS.DataSource = table;
            BS.ResumeBinding();
        }

        public static void LinkBS(ref BindingSource BS, DataTable table, String Filter, String Sort)
        {
            LinkBS(ref BS, table);
            BS.EndEdit();
            BS.SuspendBinding();
            try
            {
                BS.Filter = Filter;
                BS.Sort = Sort;
            }
            catch (SystemException ex)
            {
            }
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
            BS.DataMember = string.Empty;
            BS.DataSource = null;
            BS.ResumeBinding();
            return sortFilter;
        }
    }
}