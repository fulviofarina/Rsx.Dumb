﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

///FULVIO
namespace Rsx.Dumb
{
  
        public static partial class Tables
        {
            public static bool ReadTable(string path, ref DataTable dt)
            {
                //keep this this way, works fine
                if (File.Exists(path)) //user preferences found...
                {
                    dt.BeginLoadData();
             
                    FileInfo info = new FileInfo(path);
                    if (info.Length < 204800)
                    {
                        dt.ReadXml(path);
                    }
                    else File.Delete(path);

                    return true;
                    //cleaning
                }
                else return false;
            }

            public static void SetField(ref string MCL, ref IEnumerable<string> arrayTxtFile, string fieldTitle, string units)
            {
                string x = arrayTxtFile.FirstOrDefault(o => o.Contains(fieldTitle));

                if (string.IsNullOrEmpty(x)) return;

                x = x.Replace(fieldTitle, null);
                if (!string.IsNullOrEmpty(units)) x = x.Replace(units, null);
                MCL = x.Trim(null);
            }

            public static void CleanColumnExpressions(DataTable table)
            {
                IEnumerable<DataColumn> columns = table.Columns.OfType<DataColumn>().Where(c => c.Expression.CompareTo(string.Empty) != 0).ToArray();
                foreach (DataColumn column in columns)
                {
                    column.Expression = null;
                    column.ReadOnly = false;
                }
            }

            /// <summary>
            /// Returns a cloned "input" table (with values) after the filter "expression" has been applied
            /// </summary>
            /// <param name="input">     DataTable to filter and clone</param>
            /// <param name="expression">Expression to use as a filter</param>
            /// <returns>A new DataTable with same schema and values from "input"</returns>
            public static DataTable Clone(DataTable input, string expression)
            {
                DataTable filtered = input.Clone();
                DataView v = input.AsDataView();
                v.RowFilter = expression;
                if (v.Count != 0)
                {
                    filtered.Load(v.ToTable().CreateDataReader(), LoadOption.OverwriteChanges);
                }
                return filtered;
            }

            /// <summary>
            /// Perform a deep Copy of the object.
            /// </summary>
            /// <typeparam name="T">The type of object being copied.</typeparam>
            /// <param name="source">The object instance to copy.</param>
            /// <returns>The copied object.</returns>
            public static T Clone<T>(T source)
            {
                if (!typeof(T).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (Object.ReferenceEquals(source, null))
                {
                    return default(T);
                }

                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.Stream stream = new System.IO.MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }

            /// <summary>
            /// Reduces the DataSet "set" removing the strongly-typed DataTables from the input Type array
            /// </summary>
            /// <param name="set">           Data set to reduce</param>
            /// <param name="DataTableTypes">
            /// input Type array of strongly-typed DataTables to remove from set
            /// </param>
            public static void Preserve(System.Data.DataSet set, Type[] DataTableTypes)
            {
                System.Data.DataSet Set = set.Clone();

                bool orgConst = set.EnforceConstraints;
                Set.EnforceConstraints = false;

                foreach (System.Data.DataTable table in Set.Tables)
                {
                    System.Data.DataTable toremove = null;

                    HashSet<Type> hs = new HashSet<Type>(DataTableTypes.ToArray());

                    Type t = table.GetType();

                    if (!hs.Contains(t))
                    {
                        toremove = set.Tables[table.TableName, table.Namespace];
                    }

                    if (toremove != null)
                    {
                        toremove.ChildRelations.Clear();
                        toremove.ParentRelations.Clear();
                        foreach (System.Data.Constraint constraint in table.Constraints)
                        {
                            if (constraint.GetType().Equals(typeof(System.Data.ForeignKeyConstraint)))
                            {
                                toremove.Constraints.Remove(constraint.ConstraintName);
                            }
                        }
                        toremove.TableName += "...";
                    }
                }

                foreach (System.Data.DataTable table in Set.Tables)
                {
                    System.Data.DataTable toremove = set.Tables[table.TableName + "...", table.Namespace];
                    if (toremove != null)
                    {
                        toremove.Dispose();
                        set.Tables.Remove(toremove);
                    }
                }

                Set.EnforceConstraints = orgConst;
            }

            /// <summary>
            /// Reads a data table from an array of bytes and loads the dataTable Destiny with the contents
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="file">            the name of the file to temporaryly create</param>
            /// <param name="auxiliar">        array of bytes with datatable content</param>
            /// <param name="DestinyDataTable">table where data should be loaded</param>
            public static void ReadDTBytes<T>(string startupPath, ref byte[] auxiliar, ref T DestinyDataTable)
            {
                string file = startupPath + Guid.NewGuid().ToString() + ".xml";

               IO.WriteFileBytes(ref auxiliar, file);
                DataTable toLoad = DestinyDataTable as DataTable;

                // toLoad.BeginLoadData();

                //DataTable table = new DataTable();
                //XmlReadMode m = XmlReadMode.IgnoreSchema;
                 toLoad.ReadXml(file); // XmlReadMode.IgnoreSchema ;
              //  m = XmlReadMode.IgnoreSchema;
                // FillErrorEventHandler hanlder = fillhandler;

               // toLoad.Merge(table, true, MissingSchemaAction.AddWithKey);
                // toLoad.EndLoadData();
                toLoad.AcceptChanges();

                System.IO.File.Delete(file);

        


            // auxiliar = null;
        }
        public static void ReadDTBytes<T>( ref byte[] auxiliar, ref T DestinyDataTable)
        {
            //string file = startupPath + Guid.NewGuid().ToString() + ".xml";

          //  IO.WriteFileBytes(ref auxiliar, file);
            DataTable toLoad = DestinyDataTable as DataTable;

            string xml = Encoding.UTF8.GetString(auxiliar);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeReader reader = new XmlNodeReader(doc);
            try
            {
                toLoad.ReadXml(reader);
            }
            catch (Exception ex)
            {

              
            }
          

            reader.Close();
            reader = null;
            doc.RemoveAll();
            doc = null;
                       toLoad.AcceptChanges();
            // auxiliar = null;
        }
        public static byte[] MakeDTBytes<T2>( ref T2 dataTable, string startupPAth)
        {
            string afile = startupPAth + Guid.NewGuid().ToString() + ".xml";

            File.Delete(afile);

            DataTable dt = dataTable as DataTable;
            dt.WriteXml(afile, XmlWriteMode.WriteSchema, true);
          //  dt.Clear();
         //   dt.Dispose();
            byte[] arr = IO.ReadFileBytes(afile);
            System.IO.File.Delete(afile);
            return arr;
        }
        public static byte[] MakeDTBytes<T2>(ref T2 dataTable)
        {
           
        //   XmlDocument doc = new XmlDocument();
         //  XmlNodeReader reader = new XmlNodeReader(doc);
     

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);

            DataTable dt = dataTable as DataTable;
            dt.WriteXml(writer, XmlWriteMode.WriteSchema, true);

           
         //   writer.WriteNode(reader, true);
            //  dt.Clear();
            //   dt.Dispose();
            byte[] arr =Encoding.UTF8.GetBytes(sb.ToString());
      
            return arr;
        }

        // USE THISSSSSSS
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="rowsInput"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private static DataTable LoadTableWith<T, T2>(IEnumerable<T> rowsInput, T2 dataTable)
        {
            IEnumerable<DataRow> rows = rowsInput as IEnumerable<DataRow>;
            DataTable dt = dataTable as DataTable;

            foreach (DataRow a in rows) dt.LoadDataRow(a.ItemArray, LoadOption.OverwriteChanges);
            return dt;
        }

        public static DataTable DGVToTable(DataGridView dgv)
            {
                System.Data.DataTable table = new System.Data.DataTable();

            IEnumerable<DataGridViewColumn> cols = dgv.Columns.OfType<DataGridViewColumn>();
            cols = cols.Where(o => o.Visible);

                foreach (DataGridViewColumn col in cols)
                {
                    table.Columns.Add(col.HeaderText, typeof(object), String.Empty);
                }

                System.Collections.ArrayList list = new System.Collections.ArrayList();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cols.Contains(cell.OwningColumn))
                    {
                        list.Add(cell.Value);
                    }
                }
                table.LoadDataRow(list.ToArray(), true);
                list.Clear();
            }
                table.TableName = dgv.Name;
                return table;
            }

            public static void CloneRows(ref DataGridView Origin, ref DataTable Destiny)
            {
                HashSet<int> hint = new HashSet<int>();
                if (Origin != null && Destiny != null)
                {
                    foreach (DataGridViewCell cell in Origin.SelectedCells)
                    {
                        if (hint.Add(cell.OwningRow.Index))
                        {
                            DataRowView rv = (DataRowView)cell.OwningRow.DataBoundItem;
                            DataRow ToClone = (DataRow)rv.Row;
                            CloneARow(Destiny, ToClone);
                        }
                    }
                }
                hint.Clear();
            }

            public static DataRow CloneARow(DataTable Destiny, DataRow ToClone)
            {
                DataRow newr = Destiny.NewRow();

                for (int i = 0; i < Destiny.Columns.Count; i++)
                {
                    if (!Destiny.PrimaryKey[0].Equals(Destiny.Columns[i]))
                    {
                        newr[i] = ToClone[i];
                    }
                }
                Destiny.Rows.Add(newr);
                return newr;
            }

            public static Comparison<string> stringsorter = delegate (string a, string b)
            {
                return a.Remove(0, a.Length - 1).CompareTo(b.Remove(0, b.Length - 1));
            };

            public static T First<T>(string fieldToCompare, object valueToCompare, IEnumerable<T> rows)
            {
                Func<T, bool> compare = (i) =>
                {
                    DataRow r = i as DataRow;
                    if (EC.IsNuDelDetch(r)) return false;
                    else
                    {
                        if (!r.IsNull(fieldToCompare) && r.Field<object>(fieldToCompare).Equals(valueToCompare)) return true;
                        else return false;
                    }
                };

                T result = rows.AsQueryable().FirstOrDefault<T>(compare);

                return result;
            }

            public static IEnumerable<T> SelectMany<T>(string fieldToCompare, object valueToCompare, IEnumerable<T> rows)
            {
                Func<T, IEnumerable<T>> compare = i =>
                {
                    List<T> list = new List<T>();
                    if (i.GetType().BaseType.Equals(typeof(DataRow)))
                    {
                        DataRow r = i as DataRow;
                        if (r.RowState != DataRowState.Detached && r.RowState != DataRowState.Deleted)
                        {
                            if (!r.IsNull(fieldToCompare) && r.Field<object>(fieldToCompare).Equals(valueToCompare)) list.Add(i);
                        }
                    }
                    return list;
                };

                IEnumerable<T> results = rows.AsQueryable().SelectMany<T, T>(compare).ToList<T>();
                return results;
            }

            public static IEnumerable<T> Where<T>(string fieldToCompare, object valueToCompare, IEnumerable<T> rows)
            {
                Func<T, bool> compare = (i) =>
                {
                    if (i.GetType().BaseType.Equals(typeof(DataRow)))
                    {
                        DataRow r = i as DataRow;
                        if (r.RowState == DataRowState.Detached || r.RowState == DataRowState.Deleted) return false;
                        else
                        {
                            if (!r.IsNull(fieldToCompare) && r.Field<object>(fieldToCompare).Equals(valueToCompare)) return true;
                            else return false;
                        }
                    }
                    else
                    {
                        object o = Convert.ChangeType(i, typeof(DataRow));
                        DataRow r = o as DataRow;
                        if (r.RowState == DataRowState.Detached || r.RowState == DataRowState.Deleted) return false;
                        else
                        {
                            if (!r.IsNull(fieldToCompare) && r.Field<object>(fieldToCompare).Equals(valueToCompare)) return true;
                            else return false;
                        }
                    }
                };

                IEnumerable<T> results = rows.AsQueryable().Where<T>(compare).ToList<T>();

                return results;
            }

            public static bool MergeTable<T, T2>(ref T dt, ref T2 dataset)
            {
                object o = dt;
                DataTable table = (DataTable)o;
                object s = dataset;
                DataSet set = (DataSet)s;
                bool success = false;
                if (set == null) return success;
                if (table == null || table.Rows.Count == 0) return success;

                DataTable destination = set.Tables[table.TableName];
                destination.BeginLoadData();
                destination.Merge(table, false, MissingSchemaAction.AddWithKey);
                destination.EndLoadData();
                // destination.AcceptChanges();
                success = true;

                return success;
            }

            public static T GetTable<T>(System.Data.DataSet set)
            {
                return set.Tables.OfType<T>().FirstOrDefault();
            }
        }
    }
