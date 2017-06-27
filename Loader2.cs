using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rsx.Dumb
{
    /// <summary>
    /// A class for any worker (in principle)
    /// </summary>
    public interface ILoader2
    {
        object[] Arguments { set; }

        Action CallBackMethod { set; }

        bool CancelAsync { set; }

        Action ReportMethod { set; }

        Loader.Work WorkMethod { set; }

        void Async();
    }

    public partial class Loader : BackgroundWorker, ILoader2
    {
        protected internal object[] args;

        // private Action callbackMethod2;

        protected internal Action reportMethod2;

        // private BackgroundWorker worker;

        protected internal Work workMethod2;

        public delegate void Work(ref object arrayElement, ref object toUseOnElement);

        public object[] Arguments
        {
            // get { return args; }
            set
            {
                args = value;
            }
        }

        public Action CallBackMethod
        {
            // get { return callback; }
            set { callback = value; }
        }

        public new bool CancelAsync
        {
            set
            {
                base.CancelAsync();
            }
        }

        public Action ReportMethod
        {
            // get { return reportMethod2; }
            set { reportMethod2 = value; }
        }

        public Work WorkMethod
        {
            // get { return workMethod2; }
            set { workMethod2 = value; }
        }

        public void Async()
        {
            if (workMethod2 == null) throw new SystemException("Please specify a WorkMethod");

            if (args[2] == null) args[2] = false;
            else if (reportMethod2 == null) args[2] = false; //the final word is mine. If there is no report method, inform is false...

            // if (worker != null) worker.CancelAsync();

            // worker = new BackgroundWorker();
            DoWork += new DoWorkEventHandler(worker_DoWork2);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged2);
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            RunWorkerAsync(args);
        }

        private void worker_DoWork2(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            object[] args = e.Argument as object[];
            bool inform = (bool)args[2];
            object reader = args[1]; //might be null...

            object array = args[0];

            //if the user sent a null array, just execute the method, the user should know what does the method with these parms..
            //also the user should know hoy to try-catch errors in the method...
            if (array == null)
            {
                if (!worker.CancellationPending)
                {
                    workMethod2(ref array, ref reader);
                    if (inform) worker.ReportProgress(1);
                }
            }
            else
            {
                IEnumerable<object> ls = array as IEnumerable<object>;
                foreach (object o in ls)
                {
                    if (worker.CancellationPending) break;
                    object fileinfo = o;
                    workMethod2(ref fileinfo, ref reader);
                    if (inform) worker.ReportProgress(1);
                }
            }
        }

        private void worker_ProgressChanged2(object sender, ProgressChangedEventArgs e)
        {
            reportMethod2?.Invoke();
        }
    }
}