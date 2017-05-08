﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;

///FULVIO
namespace Rsx.Dumb
{
 
        public static partial class IO
        {
        private static string MSMQ_INSTALL_TITLE = "MSMQ Activation...";



        private static string MSMQ_INSTALL = "This program will activate Microsoft Message Queue (MSMQ)\n\n"
        // msg += "You'll need to hold the Window's Logo Key and press R\n\n"; msg +=
        // "Write 'optionalfeatures' in the box and press Enter\n\nSelect the MSMQ
        // package and click OK\n\n";
       + "Please wait for the installation to finish.\n" + 
            "Two pop-ups will appear to activate the feature.\n\n"+
            "Changes will take effect after the next system restart.\n\n"+ 
            "\t\tThank you\n";


        private static string RESTART_PC_TITLE = "Restart the computer";

            private static string RESTART_PC = "The computer will restart in 10 minutes to validate the changes.\n\n" +
                "PLEASE SAVE ANY PENDING WORK\n\n" + CLICK_OK_TO_RESTART;

            private static string CLICK_OK_TO_RESTART = "Click OK to Restart the computer with no further delay or\n\nClick Cancel to abort the scheduled shutdown";


        public static void WatchFolder (string path, string extension, ref Action<object,FileSystemEventArgs> callBack)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
         
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*." + extension;
            watcher.Changed += new FileSystemEventHandler( callBack);
            watcher.EnableRaisingEvents = true;


        }

      

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
            string  workDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            workDir += "\\Temp";
            string path = workDir + msmq + architecture + ".bat";
            System.IO.File.WriteAllText(path, content);
            //run bat files that create the VB SCRIPTS
            IO.Process(path, string.Empty, workDir);

            //run vb.vbs script!!
            string scriptFile = "vb.vbs";
            //now execute the VB scripts 1 and 2 for Container and Server MSMQ installation
       
            path = "/c " + workDir + "\\" + scriptFile;
            string cmd = "cmd.exe";
            IO.Process(cmd, path, workDir);

            //scheduled 10 minutes restart
            if (setRestart ) IO.RestartPC();

        }
    }
        public static partial class IO
        {

        public static void Process(string path, string argument, string workDir, string[] writeConsoleContent=null, bool hide = true)
        {


            try
            {

           
            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = workDir;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
          Process pro = new Process();
            info.Arguments = argument;
            info.FileName = path;
            //   info.Verb = "runas";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            if (!hide) info.WindowStyle = ProcessWindowStyle.Normal;
            pro.StartInfo = info;
                info.RedirectStandardInput = true;
              
            pro.OutputDataReceived += Pro_OutputDataReceived;
            pro.ErrorDataReceived += Pro_ErrorDataReceived;
            pro.Start();
            pro.BeginErrorReadLine();
            pro.BeginOutputReadLine();
          //  pro.WaitForExit();

            if (writeConsoleContent!=null)
            {
                foreach (string item in writeConsoleContent)
                {
                    pro.StandardInput.WriteLine(item);
                }

            }

                pro.WaitForExit();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static void Pro_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string data = e.Data;
            }

            // throw new NotImplementedException();
        }

        private static void Pro_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string data = e.Data;
            }
            //  throw new NotImplementedException();
        }

        /// <summary>
        /// unpack a Resource
        /// </summary>
        public static void UnpackCABFile(string resourcePath, string destFile, string startExecutePath, bool unpack)
            {
                if (File.Exists(resourcePath))
                {
                 if (resourcePath.CompareTo(destFile)!=0)   File.Copy(resourcePath, destFile);
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    //conservar esto para unzippear
                    if (unpack)
                    {
                        IO.Process(process, startExecutePath, "expand.exe", destFile + " -F:* " + startExecutePath, false, true, 100000);
                        File.Delete(destFile);
                    }
                }
            }

            public static void WriteFileBytes(ref byte[] r, string destFile)
            {
                FileStream f = new FileStream(destFile, FileMode.Create, FileAccess.Write);
                f.Write(r, 0, Convert.ToInt32(r.Length));
                f.Close();
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

            public static void RestartPC()
            {
                string cmd = "shutdown.exe";
                string argument = string.Empty;
                argument = "-c \"" + RESTART_PC + "\"" +
                " -r -t 600 -d P:4:1";

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

            public static double Process(Process process, string WorkingDir, string EXE, string Arguments, bool hide, bool wait, int wait_time)
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

        public static string ReadFile(string File)
        {
            int counter = 1;
            Exception ex = new Exception();
            string lecture = string.Empty;
            System.IO.FileStream fraw = null;
            System.IO.StreamReader raw = null;
            while (ex != null)
            {
                try
                {
                    fraw = new System.IO.FileStream(File, System.IO.FileMode.Open, FileAccess.Read);
                    raw = new System.IO.StreamReader(fraw);

                    ex = null;
                }
                catch (Exception ex2)
                {

                    ex = ex2;
                }
                counter++;

                if (counter == 200) return lecture;
            }
            lecture = raw.ReadToEnd();
            fraw.Close();
            fraw.Dispose();
            fraw = null;
            raw.Close();
            raw.Dispose();
            raw = null;
            return lecture;
        }
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

            public static void MakeADirectory(string path, bool overrider = false)
            {
                DirectorySecurity secutiry = new DirectorySecurity(path, AccessControlSections.Owner);

                if (!Directory.Exists(path) || overrider)
                {
                    Directory.CreateDirectory(path, secutiry);
                }
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