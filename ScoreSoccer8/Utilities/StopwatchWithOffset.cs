using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Utilities
{
    public class StopWatchWithOffset
    {
        private Stopwatch _stopwatch = new Stopwatch();
        TimeSpan _offsetTimeSpan;

        public StopWatchWithOffset()
        {

        }

        public StopWatchWithOffset(TimeSpan offsetElapsedTimeSpan)
        {
            _offsetTimeSpan = offsetElapsedTimeSpan;
            _stopwatch = new Stopwatch();
        }

        public void SetElapsed(TimeSpan ts)
        {
            _offsetTimeSpan = ts;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public bool IsRunning()
        {
            return _stopwatch.IsRunning;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return _stopwatch.Elapsed + _offsetTimeSpan;
            }
            set
            {
                _offsetTimeSpan = value;
            }
        }
    }
}
