using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Utilities
{
    public class Messaging
    {
        public delegate void ShowLoadingScreenDelegate(object sender, EventArgs e);
        public static ShowLoadingScreenDelegate ShowLoadingScreen;

        public static void RaiseShowLoadingScreen(object sender, EventArgs e)
        {
            if (ShowLoadingScreen != null)
            {
                ShowLoadingScreen(sender, e);
            }
        }

        public delegate void HideLoadingScreenDelegate(object sender, EventArgs e);
        public static HideLoadingScreenDelegate HideLoadingScreen;

        public static void RaiseHideLoadingScreen(object sender, EventArgs e)
        {
            if (HideLoadingScreen != null)
            {
                HideLoadingScreen(sender, e);
            }
        }
        
    }
}
