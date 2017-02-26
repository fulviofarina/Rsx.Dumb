using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

///FULVIO
namespace Rsx
{
  public static partial class Dumb
  {
    public static void WriteBytesFile(ref byte[] r, string destFile)
    {
      try
      {
        FileStream f = new FileStream(destFile, FileMode.Create, FileAccess.Write);
        f.Write(r, 0, Convert.ToInt32(r.Length));
        f.Close();
      }
      catch (SystemException ex)
      {
        System.Windows.Forms.MessageBox.Show("El archivo está abierto");
      }
    }

    public static byte[] ReadFileBytes(string file)
    {
      FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
      int size = Convert.ToInt32(stream.Length);
      Byte[] rtf = new Byte[size];
      stream.Read(rtf, 0, size);
      stream.Close();
      stream.Dispose();
      return rtf;
    }

    public static double Process(Process process, string WorkingDir, string EXE, string Arguments, bool hide, bool wait, int wait_time)
    {
      double span = 0;
      ProcessStartInfo info = new ProcessStartInfo(EXE);
      info.WorkingDirectory = WorkingDir;
      info.Arguments = Arguments;
      info.ErrorDialog = true;
      process.StartInfo = info;

      //   process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
      if (hide)
      {
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      }
      process.Start();
      if (wait)
      {
        process.WaitForExit();
      }

      if (process.HasExited) span = ((TimeSpan)(process.ExitTime - process.StartTime)).TotalSeconds;

      return span;
    }

    public static string ReadFile(string File)
    {
      System.IO.FileStream fraw = new System.IO.FileStream(File, System.IO.FileMode.Open);
      System.IO.StreamReader raw = new System.IO.StreamReader(fraw);

      string lecture = raw.ReadToEnd();
      fraw.Close();
      fraw.Dispose();
      fraw = null;
      raw.Close();
      raw.Dispose();
      raw = null;
      return lecture;
    }

    public static IList<string> GetDirectories(string path)
    {
      if (!System.IO.Directory.Exists(path)) return new List<string>();
      System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(path);
      IEnumerable<string> list = info.GetDirectories().Select(o => o.Name.ToUpper());
      HashSet<string> hs = new HashSet<string>(list);
      foreach (string l in list)
      {
        System.IO.DirectoryInfo info3 = new System.IO.DirectoryInfo(path + "\\" + l);
        IEnumerable<string> list3 = info3.GetDirectories().Select(o => o.Parent + "\\" + o.Name.ToUpper());
        hs.UnionWith(list3);
      }

      return hs.ToList();
    }

    public static IList<System.IO.FileInfo> GetFiles(string rootpath)
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(rootpath);
      IEnumerable<System.IO.FileInfo> files = dir.GetFiles();

      IEnumerable<System.IO.DirectoryInfo> dirs = dir.GetDirectories();
      foreach (System.IO.DirectoryInfo d in dirs)
      {
        IEnumerable<System.IO.FileInfo> fs = d.GetFiles();
        files = files.Union(fs);
      }
      return files.ToList();
    }

    public static IList<string> GetFileNames(string path, string Ext)
    {
      if (!System.IO.Directory.Exists(path)) return new List<string>();
      System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(path);
      IEnumerable<string> list = info.GetFiles().Where(o => o.Extension.ToUpper().CompareTo(Ext) == 0).Select(o => o.Name.ToUpper().Replace(Ext, null));
      return new HashSet<string>(list).ToList();
    }
  }
}