using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class NotesViewModel: Notification
    {

        #region "Commands"

        private ICommand _okClickCommand;
        public ICommand OkClickCommand
        {
            get
            {
                if (_okClickCommand == null)
                {
                    _okClickCommand = new DelegateCommand(param => this.OkClicked(), param => true);
                }

                return _okClickCommand;
            }
        }

        private ICommand _cancelClickCommand;
        public ICommand CancelClickCommand
        {
            get
            {
                if (_cancelClickCommand == null)
                {
                    _cancelClickCommand = new DelegateCommand(param => this.CancelClicked(), param => true);
                }

                return _cancelClickCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"

        private void OkClicked()
        {

        }

        private void CancelClicked()
        {

        }

        #endregion "Methods"
    }
}
