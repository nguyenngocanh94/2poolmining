using System.Timers;

namespace Chia2Pool.Periodical
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
        
        protected virtual void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }
        
        protected virtual void OnStop()
        {
            timer.Stop();
        }
    }
}