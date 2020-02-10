using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ScoreSoccer8.Classes
{
    public static class Clock
    {
        private static bool _initialized = false;
        private static void Initialize()
        {
            _timer.Interval = TimeSpan.FromSeconds(.8);
            _timer.Tick += _timer_Tick;
        }

        static void _timer_Tick(object sender, EventArgs e)
        {
            if (IsClockRunning())
            {
                if (ClockHasChanged != null)
                {
                    ClockHasChanged(new Object(), new ClockEventArgs(GetCurrentTimeSpan()));
                }
            }

            if (!CountUp)
            {
                TimeSpan ts = GetCurrentTimeSpan();
                if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                {
                    StopClock();
                }
            }
        }

        public static string GetCurrentClockValue()
        {
            string returnValue = string.Empty;

            if (CountUp)
            {
                returnValue = _stopWatch.Elapsed.ToString("mm\\:ss");
            }
            else
            {
                if ((StartTime - _stopWatch.Elapsed) <= TimeSpan.FromSeconds(0))
                {
                    _timer.Stop();
                    _stopWatch.Stop();

                    returnValue = TimeSpan.FromSeconds(0).ToString("mm\\:ss");
                }
                else
                {
                    returnValue = (StartTime - _stopWatch.Elapsed).ToString("mm\\:ss");
                }
            }

            return returnValue;
        }

        public static TimeSpan GetCurrentTimeSpan()
        {
            TimeSpan returnValue;

            if (CountUp)
            {
                returnValue = _stopWatch.Elapsed;
            }
            else
            {
                if ((StartTime - _stopWatch.Elapsed) <= TimeSpan.FromSeconds(0))
                {
                    _timer.Stop();
                    _stopWatch.Stop();

                    returnValue = TimeSpan.FromSeconds(0);
                }
                else
                {
                    returnValue = (StartTime - _stopWatch.Elapsed);
                }
            }

            return returnValue;
        }

        public static event ClockChanged ClockHasChanged;

        public delegate void ClockChanged(object sender, ClockEventArgs e);
        public class ClockEventArgs
        {
            public ClockEventArgs(TimeSpan formattedTime)
            {
                FormattedTime = formattedTime;
            }

            public TimeSpan FormattedTime { get; set; }
        }

        private static DispatcherTimer _timer = new DispatcherTimer();
        private static StopWatchWithOffset _stopWatch = new StopWatchWithOffset();

        public static void StopClock()
        {
            if (_stopWatch.IsRunning())
            {
                _stopWatch.Stop();
                _timer.Stop();
            }
        }

        public static bool IsClockRunning()
        {
            bool returnValue = false;

            if (_stopWatch != null)
            {
                returnValue = _stopWatch.IsRunning();
            }

            return returnValue;
        }

        public static void ResetClock()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
        }

        public static void StartClock()
        {
            if (!_initialized)
            {
                Initialize();
            }

            if (!_stopWatch.IsRunning())
            {
                _stopWatch.Start();
                _timer.Start();
            }
        }

        public static TimeSpan GetElapsed()
        {
            return _stopWatch.Elapsed;
        }

        public static TimeSpan GetCountDownTimeLeft()
        {
            return StartTime - _stopWatch.Elapsed;
        }

        public static void SetClock(TimeSpan ts)
        {
            bool restartAfterFinishedSetting = false;
            StartTime = ts;
            if (_stopWatch.IsRunning())
            {
                restartAfterFinishedSetting = true;
            }

            _stopWatch.Stop();
            _stopWatch.Reset();

            if (CountUp)
            {
                _stopWatch.SetElapsed(ts);
            }

            //This will set the clock if it is not running
            //if (ClockHasChanged != null)
            //{
            //    ClockHasChanged(new Object(), new ClockEventArgs(StartTime));
            //}

            if (restartAfterFinishedSetting)
            {
                _stopWatch.Start();
                _timer.Start();
            }
        }

        public static bool CountUp { get; set; }
        private static TimeSpan StartTime { get; set; }
    }
}
