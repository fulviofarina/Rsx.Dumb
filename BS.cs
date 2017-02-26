using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

///FULVIO
namespace Rsx
{
  public static partial class Dumb
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
    /// Returns a HashTable with "Text" Bindings, where the key is the DataTableColumn.ColumnName of the bound member
    /// </summary>
    /// <param name="bs"></param>
    /// <returns></returns>
    public static Hashtable ArrayOfBindings(ref BindingSource bs, string format)
    {
      Hashtable bslist = new Hashtable();
      DataSourceUpdateMode mo = DataSourceUpdateMode.OnPropertyChanged;
      bool t = true;
      string text = "Text";
      string column;

      DataTable dt = (bs.DataSource as DataSet).Tables[bs.DataMember];
      //     Binding diam = new Binding(text, bs, column, t, mo);

      foreach (DataColumn item in dt.Columns)
      {
        column = item.ColumnName;
        // column = Unit.DiameterColumn.ColumnName;
        Binding b = new Binding(text, bs, column, t, mo,DBNull.Value,format);
        bslist.Add(column, b);
      }

      return bslist;
    }

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