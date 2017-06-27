using System;
using System.Timers;


namespace Rsx.Dumb
{

    public class EventData : EventArgs
    {
        public EventData(object[] arguments = null) : base()
        {
            args = arguments;
        }

        private object[] args = null;

        public object[] Args
        {
            get
            {
                return args;
            }

            set
            {
                args = value;
            }
        }
    }


    public class Timer : System.Timers.Timer
  {


     

        public Timer(int seg, Action toDo)
    {
            this.Elapsed += Tim_Tick;
      this.Interval = seg * 1000;
    }

        private void Tim_Tick(object sender, ElapsedEventArgs e)
        {
            if (afterTick == null) return;
            afterTick.Invoke();
            afterTick = null;
            this.Dispose();
        }


        //  private void Tim_Tick(object sender, EventArgs e)

        private Action afterTick;

    public Action AfterTick
    {
      get { return afterTick; }
      set { afterTick = value; }
    }
  }

  /// <summary>
  /// A Worker made for multiple methods
  /// </summary>
  ///
}