using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.ViewModels;

namespace ScoreSoccer8.Views
{
    public partial class PopupClock : UserControl
    {
        PopupClockViewModel vm = new PopupClockViewModel();
        public delegate void CloseOrHidePopupDelegate(object sender, EventArgs e);
        public event CloseOrHidePopupDelegate CloseOrHidePopup;

        public PopupClock()
        {
            InitializeComponent();
            DataContext = vm;
            vm.ClosePopupWindow += vm_ClosePopupWindow;
            vm.ShowPopupAfterClockSelected += vm_ShowPopupAfterClockSelected;
        }

        void vm_ShowPopupAfterClockSelected(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void ResetClockDefault()
        {
            vm.ResetClockDefault();
        }

        private void vm_ClosePopupWindow(object sender, EventArgs e)
        {
            if (CloseOrHidePopup != null)
            {
                CloseOrHidePopup(this, new EventArgs());
            }
        }

        private void PickerTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
