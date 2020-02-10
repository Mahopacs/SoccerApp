using ScoreSoccer8.Classes;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class PopupClockViewModel : Notification, IDisposable
    {
        public delegate void ClosePopup(object sender, EventArgs e);
        public event ClosePopup ClosePopupWindow;

        //We need this here because we have to hide the popup because for some reason the
        //popup shows on top of the date time picker and it looks horrible.  We hide the popup when
        //we are touching the time picker, and then we have to "re-show" it after.
        public delegate void ShowPopupAfterClockSelectionDelegate(object sender, EventArgs e);
        public event ShowPopupAfterClockSelectionDelegate ShowPopupAfterClockSelected;

        private bool _initializing;

        public PopupClockViewModel()
        {
            ResetClockDefault();
        }

        public void ResetClockDefault()
        {
            _initializing = true;

            if (Clock.CountUp)
            {
                ClockTime = Clock.GetElapsed();
            }
            else
            {
                ClockTime = Clock.GetCountDownTimeLeft();
            }

            _initializing = false;
        }

        #region properties

        private TimeSpan _clockTime;
        public TimeSpan ClockTime
        {
            get { return _clockTime; }
            set 
            { 
                _clockTime = value; 
                NotifyPropertyChanged("ClockTime");
                if (!_initializing)
                {
                    Clock.SetClock(value);

                    if (ShowPopupAfterClockSelected != null)
                    {
                        ShowPopupAfterClockSelected(this, new EventArgs());
                    }
                    if (ClosePopupWindow != null)
                    {
                        ClosePopupWindow(this, new EventArgs());
                    }

                }
            }
        }
        

        #endregion properties

        private ICommand _startClockCommand;
        public ICommand StartClockCommand
        {
            get
            {
                if (_startClockCommand == null)
                {
                    _startClockCommand = new DelegateCommand(param => this.StartClock(), param => true);
                }

                return _startClockCommand;
            }
        }

        public void StartClock()
        {
            Clock.StartClock();

            if (ClosePopupWindow != null)
            {
                ClosePopupWindow(this, new EventArgs());
            }
        }

        private ICommand _stopClockCommand;
        public ICommand StopClockCommand
        {
            get
            {
                if (_stopClockCommand == null)
                {
                    _stopClockCommand = new DelegateCommand(param => this.StopClock(), param => true);
                }

                return _stopClockCommand;
            }
        }

        public void StopClock()
        {
            //Clock.StopClock();

            if (ClosePopupWindow != null)
            {
                ClosePopupWindow(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            
        }
    }
}
