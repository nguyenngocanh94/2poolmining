using System;
using System.Timers;

namespace Timer
{
    public class BaseTimer
    {
        private System.Timers.Timer timer;
        
        public BaseTimer(int time)
        {
            int wait = time * 1000;
            timer = new System.Timers.Timer(wait);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;
        }
        
        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }
        
        protected void OnStop()
        {
            timer.Stop();
        }
    }
}