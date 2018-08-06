using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rsx.Dumb
{
   
    public interface ILoader
    {
        bool IsBusy { get; }
        void RunWorkerAsync(object argument);
        void RunWorkerAsync();
        void Set(IList<Action> LoadMethods, Action CallBackMethod = null, Action<int> ReportMethod = null, Action<Exception> ExceptionMethod = null);
        void Set(Action LoadMethod, Action CallBackMethod = null, Action<int> ReportMethod = null, Action<Exception> ExceptionMethod = null);
        void CancelLoader();
    }

    /// <summary>
    /// This is a System.ComponentModel.BackgroundWorker For be used by another class?
    /// </summary>
    public partial class Loader : BackgroundWorker, ILoader
    {

        public void CancelLoader()
        {
             if (this.IsBusy)   this.CancelAsync = true;
        
        }
        public Loader() : base()
        {
        
        }

      //  public bool IsDisposed = false;
   
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // if (report == null) return;
            if (this.CancellationPending) return;
            if (e.UserState != null)
            {
                SystemException ex = e.UserState as SystemException;
                exceptionReport?.Invoke(ex);
            }
            int percentage = e.ProgressPercentage;
            report?.Invoke(percentage);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int length = mainMethods.Count;
            double step = 100;
            if (length != 1) step = (100.0 / (length - 1));

            for (int i = 0; i < mainMethods.Count; i++)
            {
                if (this.CancellationPending)
                {
                    continue;
                }
                int perc = Convert.ToInt32(Math.Ceiling((step * i)));
                SystemException x = null;
                try
                {
                    Action async = mainMethods[i];
                    // if (async == null) continue;
                    async?.Invoke();
                }
                catch (SystemException ex)
                {
                    x = ex;
                }
                ReportProgress(perc, x);
            }
        }
        public void Set(Action LoadMethod, Action CallBackMethod = null, Action<int> ReportMethod = null, Action<Exception> ExceptionMethod = null)
        {
            List<Action> ls = new List<Action>();
            ls.Add(LoadMethod);
            Set(ls, CallBackMethod, ReportMethod, ExceptionMethod);
        }
        public void Set(IList<Action> LoadMethods, Action CallBackMethod=null, Action<int> ReportMethod=null, Action<Exception> ExceptionMethod=null)
        {
            mainMethods = LoadMethods;
            callback = CallBackMethod;
            report = ReportMethod;

            exceptionReport = defaultExceptionReport;
           
            if (ExceptionMethod!=null)
            {
                exceptionReport = ExceptionMethod;
            }

     
                this.WorkerReportsProgress = true;
                this.ProgressChanged += worker_ProgressChanged;
      
            this.WorkerSupportsCancellation = true;
            this.DoWork += worker_DoWork;
       
            this.RunWorkerCompleted += worker_RunWorkerCompleted;

           
        }
     
        private void worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            callback?.Invoke();


            this.Dispose();

         

        


          //  isDisposed = true;

         

          
        }

        private void defaultExceptionReport(Exception ex)
        {
            throw ex;
           // MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.TargetSite, "Problems loading a data table content");
        }
     //   private bool isDisposed = false;

        protected internal IList<Action> mainMethods;
        protected internal Action callback;
        protected internal Action<int> report;
        protected internal Action<Exception> exceptionReport;

        public  new bool IsBusy
        {
            get
            {
                return base.IsBusy;
            }

          
        }
    }
}