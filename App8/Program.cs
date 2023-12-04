using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App8
{
    class CustomTimer
    {
        private readonly object lockObject = new object();
        private Timer timer;
        private bool isRunning;

        public event Action TimeElapsed;
        public event Action<Exception> OnError;
        public int Interval { get; set; }
        public CustomTimer(int interval)
        {
            Interval = interval;
            isRunning = false;
        }
        public void Start(int time)
        {
            lock (lockObject)
            {
                if (isRunning)
                    return;
                timer = new Timer(TimerCallback, null, 0, Interval);
                isRunning = true;
            }
            Thread.Sleep(time);
        }
        public void Stop()
        {
            lock (lockObject)
            {
                if (!isRunning)
                    return;

                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                isRunning = false;
            }
        }

        private void TimerCallback(object state)
        {
            try
            {
                Task.Run(() => TimeElapsed?.Invoke());
            }
            catch (Exception ex)
            {
                Task.Run(() => OnError?.Invoke(ex));
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            CustomTimer customTimer = new CustomTimer(500);
            customTimer.TimeElapsed += () => Console.WriteLine("Time elapsed!");
            customTimer.OnError += ex => Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Timer started");
            customTimer.Start(1000);
            customTimer.Stop();
            Console.WriteLine("Timer stopped");
        }
    }
}
