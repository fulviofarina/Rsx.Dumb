using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.Dumb
{
    public static partial class IO
    {
        private static string CLICK_OK_TO_RESTART = "Click OK to Restart the computer with no further delay or\n\nClick Cancel to abort the scheduled shutdown";
        private static string MSMQ_INSTALL = "This program will activate Microsoft Message Queue (MSMQ)\n\n"
        // msg += "You'll need to hold the Window's Logo Key and press R\n\n"; msg += "Write
        // 'optionalfeatures' in the box and press Enter\n\nSelect the MSMQ package and click OK\n\n";
       + "Please wait for the installation to finish.\n" +
            "Two pop-ups will appear to activate the feature.\n\n" +
            "Changes will take effect after the next system restart.\n\n" +
            "\t\tThank you\n";

        private static string MSMQ_INSTALL_TITLE = "MSMQ Activation...";
        private static string RESTART_PC = "The computer will restart in 10 minutes to validate the changes.\n\n" +
            "PLEASE SAVE ANY PENDING WORK\n\n" + CLICK_OK_TO_RESTART;

        private static string RESTART_PC_TITLE = "Restart the computer";


        public static Uri GenerateURI(string file, int guidLenght, string folderpath, bool asHTML)
        {
            Uri uri = new Uri("about:blank");
            if (System.IO.File.Exists(file))
            {
                string newFile = Rsx.Dumb.Strings.CachePath;
                string fileName = file.Replace(folderpath, null);
                int indexofPoint = fileName.LastIndexOf('.');
                string extension = fileName.Substring(indexofPoint, fileName.Length - indexofPoint);
                newFile += fileName.Substring(0,indexofPoint);
                newFile += "." + Guid.NewGuid().ToString().Substring(0, guidLenght);
                newFile += extension;
                if (asHTML) newFile += ".html";
                File.Copy(file, newFile, true);
                uri = new Uri(newFile);
            }

            return uri;
        }

        /// <summary>
        /// Installs MSMQ, The Microsoft Queuing Messaging System
        /// </summary>
        /// <param name="setRestart">True to restart the computer afterwards</param>
        public static void InstallMSMQ(bool setRestart = true)
        {
            DialogResult result = MessageBox.Show(MSMQ_INSTALL, MSMQ_INSTALL_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.Cancel) return;

            bool is64 = Environment.Is64BitOperatingSystem;
            string msmq = "msmq";
            string architecture = "x86";
            string content = Resource.msmqx86;

            if (is64)
            {
                architecture = "x64";
                content = Resource.msmqx64;
            }

            //make bat file
            string workDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            workDir += "\\Temp";
            string path = workDir + msmq + architecture + ".bat";
            System.IO.File.WriteAllText(path, content);
            //run bat files that create the VB SCRIPTS
            IO.Process(path, workDir, string.Empty, true);

            //run vb.vbs script!!
            string scriptFile = "vb.vbs";
            //now execute the VB scripts 1 and 2 for Container and Server MSMQ installation

            path = "/c " + workDir + "\\" + scriptFile;
            string cmd = "cmd.exe";
            IO.Process(cmd, workDir, path, true);

            //scheduled 10 minutes restart
            if (setRestart) IO.RestartPC();
        }

        public static RichTextBox RichTextBox(string filepath, string title, float size)
        {
            RichTextBox box;
            box = new RichTextBox();
            box.Text = filepath;
           
            Form   from = new Form();
            from.Text = title;
            box.Font = new System.Drawing.Font(box.Font.FontFamily, size);
            box.Dock = DockStyle.Fill;
            from.StartPosition = FormStartPosition.CenterScreen;
            from.TopMost = true;
            from.MaximizeBox = false;
            from.Controls.Add(box);
            from.BringToFront();
            return box;
         }
        /// <summary>
        ///  Reads a file and shows it in a message box
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="title"></param>
        public static void ReadFileToMessageBox(string filePath, string title)
        {
            if (!System.IO.File.Exists(filePath)) return;
            string error = System.IO.File.ReadAllText(filePath);
        
            MessageBox.Show(error, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>
        /// Writes the exception content to a file
        /// </summary>
        /// <param name="justFileName"></param>
        /// <param name="ex"></param>
        public static void WriteException(string justFileName, Exception ex)
        {
            string error = "Severe program error: " + ex?.Message + "\n\nat code:\n\n" + ex?.StackTrace;
            justFileName= Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\" + justFileName;
            //  if (System.IO.File.Exists(crashFile)) System.IO.File.Delete(crashFile);
            File.AppendAllText(justFileName, error);
        }


        /// <summary>
        /// Watches a folder according to the filter and extensions
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extension"></param>
        /// <param name="callBack"></param>
        /// <param name="filter"></param>
        public static void WatchFolder(string path, string extension, ref Action<object, FileSystemEventArgs> callBack, string filter = "*")
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;

            // watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite |
            // NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.NotifyFilter = NotifyFilters.FileName;
            // | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Filter = filter + extension;
            // watcher.Changed += new FileSystemEventHandler( callBack);
            watcher.Created += new FileSystemEventHandler(callBack);
            watcher.EnableRaisingEvents = true;
        }

        public static void WriteFileText(string tempFile, string Response, bool append = false)
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            if (append) File.AppendAllText(tempFile, Response);
            else File.WriteAllText(tempFile, Response);
        }

        public static void DeleteIfExists(string tempFile)
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    public static partial class IO
    {

        /// <summary>
        /// File to use to expand compressed files
        /// </summary>
        public static string EXPAND_EXE = "expand.exe";



        /// <summary>
        /// Gets directories and subdirectories
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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



        /// <summary>
        /// Gets the file names without the extension
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Ext"></param>
        /// <returns></returns>
        public static IList<string> GetFileNames(string path, string Ext)
        {
            if (!System.IO.Directory.Exists(path)) return new List<string>();
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(path);
            IEnumerable<string> list = info.GetFiles().Where(o => o.Extension.ToUpper().CompareTo(Ext) == 0).Select(o => o.Name.ToUpper().Replace(Ext, null));
            return new HashSet<string>(list).ToList();
        }



        /// <summary>
        /// Gets a file list from a folder
        /// </summary>
        /// <param name="rootpath"></param>
        /// <param name="subFolders"></param>
        /// <returns></returns>
        public static IList<System.IO.FileInfo> GetFiles(string rootpath, bool subFolders = true)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(rootpath);
            IEnumerable<System.IO.FileInfo> files = dir.GetFiles();

            if (subFolders)
            {
                IEnumerable<System.IO.DirectoryInfo> dirs = dir.GetDirectories();
                foreach (System.IO.DirectoryInfo d in dirs)
                {
                    IEnumerable<System.IO.FileInfo> fs = d.GetFiles();
                    files = files.Union(fs);
                }
            }
            return files.ToList();
        }


        /// <summary>
        /// Loads a file content into a RTB box and reports
        /// </summary>
        /// <param name="showProgress"></param>
        /// <param name="input"></param>
        /// <param name="file"></param>
        public static void LoadFilesIntoBoxes(Action showProgress, ref RichTextBox input, string file)
        {
            //load files
            //Clear InputFile RTF Control
            input.Clear();
            //load file if exists
            bool exist = System.IO.File.Exists(file);
            if (exist) input.LoadFile(file, RichTextBoxStreamType.PlainText);

            showProgress?.Invoke();
        }

        /// <summary>
        /// Makes a directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="overrider"></param>
        public static void MakeADirectory(string path, bool overrider = false)
        {
            // DirectorySecurity secutiry = new DirectorySecurity(path, AccessControlSections.Owner);

            if (!Directory.Exists(path) || overrider)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static Process Process(string path, string workDir, string argument="", bool start=false, bool hide = true, DataReceivedEventHandler receivedHandler = null, EventHandler exited = null)
        {
            // int id = 0;
            Process pro = Process(path, workDir, argument, hide, ref receivedHandler, ref exited);
            // int id = pro.Id;
            if (start)
            {
                pro.Start();

                pro.BeginErrorReadLine();
                pro.BeginOutputReadLine();

                pro.WaitForExit();
            }

            return pro;
        }

        /// <summary>
        /// Starts a process
        /// </summary>
        /// <param name="path"></param>
        /// <param name="workDir"></param>
        /// <param name="argument"></param>
        /// <param name="hide"></param>
        /// <param name="receivedHandler"></param>
        /// <param name="exited"></param>
        /// <returns></returns>
        public static Process Process(string path, string workDir, string argument, bool hide, ref DataReceivedEventHandler receivedHandler, ref EventHandler exited)
        {
            Process pro = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = workDir;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.Arguments = argument;
            info.FileName = path;
            // info.Verb = "runas";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            if (!hide) info.WindowStyle = ProcessWindowStyle.Normal;

            pro.StartInfo = info;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            if (receivedHandler == null)
            {
                receivedHandler = defaultOutputMode;
            }

            pro.OutputDataReceived += defaultOutputMode;
            pro.ErrorDataReceived += defaultOutputMode;

            if (exited != null)
            {
                pro.Exited += exited;
                pro.EnableRaisingEvents = true;
            }

            return pro;
        }

        /// <summary>
        /// Starts a process and returns the time elapsed
        /// </summary>
        /// <param name="process"></param>
        /// <param name="WorkingDir"></param>
        /// <param name="EXE"></param>
        /// <param name="Arguments"></param>
        /// <param name="hide"></param>
        /// <param name="wait"></param>
        /// <param name="wait_time"></param>
        /// <returns></returns>
        public static double Process(Process process, string WorkingDir, string EXE, string Arguments="", bool hide=true, bool wait= true, int wait_time = 100000)
        {
            double span = 0;
            ProcessStartInfo info = new ProcessStartInfo(EXE);
            info.WorkingDirectory = WorkingDir;
            info.Arguments = Arguments;
            info.ErrorDialog = true;
            process.StartInfo = info;

            // process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
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

        /// <summary>
        /// Start process with output
        /// </summary>
        /// <param name="exeName"></param>
        /// <param name="arguments"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="exitCode"></param>
        /// <param name="output"></param>
        public static void ProcessWithOutPut(string exeName, string arguments, int timeoutMilliseconds, out int exitCode, out string output)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = exeName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                bool exited = process.WaitForExit(timeoutMilliseconds);
                if (exited) { exitCode = process.ExitCode; }
                else
                {
                    exitCode = -1;
                }
            }
        }



        /// <summary>
        /// ESTO ES UN ASCO, ARREGLAR, ESTO NO PUEDE SER ASI
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static string ReadFile(string File)
        {
            // int counter = 1;
            Exception ex = new Exception();
            string lecture = string.Empty;
            System.IO.FileStream fraw = null;
            System.IO.StreamReader raw = null;
            while (ex != null)
            {
                try
                {

                    ex = null;
                    fraw = new System.IO.FileStream(File, System.IO.FileMode.Open, FileAccess.Read);
                    raw = new System.IO.StreamReader(fraw);
                    lecture = raw.ReadToEnd();
                  
                    fraw.Close();
                    fraw.Dispose();
                    fraw = null;
                    raw.Close();
                    raw.Dispose();
                    raw = null;
                }
                catch (Exception ex2)
                {
                  
                    ex = ex2;
                    return lecture;
                   
                }
           
            }

       
            return lecture;
        }


        /// <summary>
        /// Reads the bytes from a file
        /// </summary>
        /// <param name="file">Full filepath</param>
        /// <returns></returns>
        public static byte[] ReadFileBytes(string file)
        {
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            int size = Convert.ToInt32(stream.Length);
            Byte[] rtf = new Byte[size];
            stream.Read(rtf, 0, size);
            stream.Close();
            stream.Dispose();
            stream = null;
            return rtf;
        }


        /// <summary>
        /// Restarts PC in 600 seconds
        /// </summary>
        public static void RestartPC(string seconds = "600")
        {
            string cmd = "shutdown.exe";
            string argument = string.Empty;
            argument = "-c \"" + RESTART_PC + "\"" +
            " -r -t "+ seconds + " -d P:4:1";

            System.Diagnostics.Process.Start(cmd, argument);

            DialogResult result = MessageBox.Show(RESTART_PC + CLICK_OK_TO_RESTART, RESTART_PC_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.Cancel)
            {
                argument = "/a";
            }
            else
            {
                argument = "-r -t 0";
            }
            System.Diagnostics.Process.Start(cmd, argument);
        }

       /// <summary>
       /// Unpacks a file resource from a folder to another folder
       /// </summary>
       /// <param name="resourcePath">filepath to the resource</param>
       /// <param name="destFile">destiny filepath</param>
       /// <param name="startExecutePath">execution folder</param>
       /// <param name="unpack">true to unpack, false to just copy the resource</param>
        public static void UnpackCABFile(string resourcePath, string destFile, string startExecutePath, bool unpack, int wait = 100000)
        {
            if (File.Exists(resourcePath))
            {
                if (resourcePath.CompareTo(destFile) != 0) File.Copy(resourcePath, destFile);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                //conservar esto para unzippear
                if (unpack)
                {
                //    int wait = 100000;
                    IO.Process(process, startExecutePath, EXPAND_EXE, destFile + " -F:* " + startExecutePath, false, true, wait);
                    File.Delete(destFile);
                }
            }
        }

     /// <summary>
     /// Writes bytes to a file
     /// </summary>
     /// <param name="content"></param>
     /// <param name="destFile"></param>
        public static void WriteFileBytes(ref byte[] content, string destFile)
        {
            FileStream f = new FileStream(destFile, FileMode.Create, FileAccess.Write);
            f.Write(content, 0, Convert.ToInt32(content.Length));
            f.Close();
        }


        /// <summary>
        /// a handler to the default output mode in a process. Does nothing so far
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void defaultOutputMode(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}