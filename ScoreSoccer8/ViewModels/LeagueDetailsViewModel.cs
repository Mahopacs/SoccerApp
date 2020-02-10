using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.ViewModels
{   
    public class LeagueDetailsViewModel : Notification
    {

        public LeagueDetailsViewModel()
        {
            LeagueDetails = new LeagueModel();  
        }

        #region "Properties"

        private LeagueModel _leagueDetails;
        public LeagueModel LeagueDetails
        {
            get { return _leagueDetails; }
            set { _leagueDetails = value; NotifyPropertyChanged("LeagueDetails"); }
        }
     
        #endregion "Properties"

        #region "Methods"

        public void Initialize(int leagueID)
        {
            if (leagueID != 0)       
            {
                LeagueDetails = DAL.Instance().GetLeague(leagueID);
            }
        }

        public void SaveToDatabase()
        {
            BaseTableDataAccess.Instance().UpsertLeague(this.LeagueDetails.League);
        }
  
        #endregion "Methods"
    }
}
