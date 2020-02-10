using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.Classes
{
    public class Icons
    {
        public List<AppIcon> IconList = new List<AppIcon>();

        public Icons()
        {

            // Generic Buttons and Buttons not specific to one screen.
            IconList.Add(new AppIcon(Enums.IconButtonType.Delete, Common.GetIconText(Enums.IconButtonType.Delete), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.All }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Plus, Common.GetIconText(Enums.IconButtonType.Plus), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.All }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Edit, Common.GetIconText(Enums.IconButtonType.Edit), "/Assets/Edit3.png", new List<Enums.Screen>() { Enums.Screen.All }));
            IconList.Add(new AppIcon(Enums.IconButtonType.OK, Common.GetIconText(Enums.IconButtonType.OK), "/Assets/ok.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Cancel, Common.GetIconText(Enums.IconButtonType.Cancel), "/Assets/cancel.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.Boxscore }));
            IconList.Add(new AppIcon(Enums.IconButtonType.YellowCard, Common.GetIconText(Enums.IconButtonType.YellowCard), "/Assets/yellowCard.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.TimeLine, Enums.Screen.GameStats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.RedCard, Common.GetIconText(Enums.IconButtonType.RedCard), "/Assets/redCard.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.TimeLine, Enums.Screen.GameStats }));

            // Main Screen
            IconList.Add(new AppIcon(Enums.IconButtonType.Teams, Common.GetIconText(Enums.IconButtonType.Teams), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Main }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Leagues, Common.GetIconText(Enums.IconButtonType.Leagues), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Main }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Players, Common.GetIconText(Enums.IconButtonType.Players), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Main }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Games, Common.GetIconText(Enums.IconButtonType.Games), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Main }));

            // Leagues Screen
            IconList.Add(new AppIcon(Enums.IconButtonType.AddLeague, Common.GetIconText(Enums.IconButtonType.AddLeague), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.Leagues }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DeleteLeauge, Common.GetIconText(Enums.IconButtonType.DeleteLeauge), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.Leagues }));

            // Players Screen
            IconList.Add(new AppIcon(Enums.IconButtonType.DeletePlayerFromPlayerList, Common.GetIconText(Enums.IconButtonType.DeletePlayerFromPlayerList), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.Players }));
            IconList.Add(new AppIcon(Enums.IconButtonType.AddNewPlayerToPlayerList, Common.GetIconText(Enums.IconButtonType.AddNewPlayerToPlayerList), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Players }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Search, Common.GetIconText(Enums.IconButtonType.Search), "/Assets/search.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Players }));

            // PlayersRosters Screen
            IconList.Add(new AppIcon(Enums.IconButtonType.AddPlayer, Common.GetIconText(Enums.IconButtonType.AddPlayer), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.PlayersRosters }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DeletePlayer, Common.GetIconText(Enums.IconButtonType.DeletePlayer), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.PlayersRosters }));
            IconList.Add(new AppIcon(Enums.IconButtonType.AddNewPlayerToPlayerList, Common.GetIconText(Enums.IconButtonType.AddNewPlayerToPlayerList), null, new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayersRosters }));

            // Rosters Screen
            IconList.Add(new AppIcon(Enums.IconButtonType.AddPlayer, Common.GetIconText(Enums.IconButtonType.AddPlayer), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.Rosters }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DeletePlayer, Common.GetIconText(Enums.IconButtonType.DeletePlayer), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.Rosters }));
      
            // Player Details
            IconList.Add(new AppIcon(Enums.IconButtonType.LeftFoot, Common.GetIconText(Enums.IconButtonType.LeftFoot), "/Assets/footLeft.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayerDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.RightFoot, Common.GetIconText(Enums.IconButtonType.RightFoot), "/Assets/footRight.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayerDetails }));

            // Teams
            IconList.Add(new AppIcon(Enums.IconButtonType.AddTeams, Common.GetIconText(Enums.IconButtonType.AddTeams), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.Teams }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DelteTeams, Common.GetIconText(Enums.IconButtonType.DelteTeams), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.Teams }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Roster, Common.GetIconText(Enums.IconButtonType.Roster), "/Assets/rosters.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Teams }));

            // Games
            IconList.Add(new AppIcon(Enums.IconButtonType.AddGame, Common.GetIconText(Enums.IconButtonType.AddGame), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.Games }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DeleteGame, Common.GetIconText(Enums.IconButtonType.DeleteGame), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.Games }));
            IconList.Add(new AppIcon(Enums.IconButtonType.EditGame, Common.GetIconText(Enums.IconButtonType.EditGame), "/Assets/Edit3.png", new List<Enums.Screen>() { Enums.Screen.Games }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Stats, Common.GetIconText(Enums.IconButtonType.Stats), "/Assets/statistics.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Games, Enums.Screen.GameManager }));
            IconList.Add(new AppIcon(Enums.IconButtonType.GM, Common.GetIconText(Enums.IconButtonType.GM), "/Assets/GameManager.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Games }));

            // Game Details
            IconList.Add(new AppIcon(Enums.IconButtonType.ClockUp, Common.GetIconText(Enums.IconButtonType.ClockUp), "/Assets/clockUp.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.ClockDown, Common.GetIconText(Enums.IconButtonType.ClockDown), "/Assets/clockDown.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameDetails }));

            // Play List
            IconList.Add(new AppIcon(Enums.IconButtonType.AddPlay, Common.GetIconText(Enums.IconButtonType.AddPlay), "/Assets/plus.png", new List<Enums.Screen>() { Enums.Screen.PlayList }));
            IconList.Add(new AppIcon(Enums.IconButtonType.DeltePlay, Common.GetIconText(Enums.IconButtonType.DeltePlay), "/Assets/delete.png", new List<Enums.Screen>() { Enums.Screen.PlayList }));
            
            // Stats
            IconList.Add(new AppIcon(Enums.IconButtonType.StatTimeline, Common.GetIconText(Enums.IconButtonType.StatTimeline), "/Assets/timeline.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.StatGameStats, Common.GetIconText(Enums.IconButtonType.StatGameStats), "/Assets/gameStats.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.StatAwayBoxscore, Common.GetIconText(Enums.IconButtonType.StatAwayBoxscore), "/Assets/awayGameStats.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.StatHomeBoxscore, Common.GetIconText(Enums.IconButtonType.StatHomeBoxscore), "/Assets/homeGameStats.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.StatPlayByPlay, Common.GetIconText(Enums.IconButtonType.StatPlayByPlay), "/Assets/pbp.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats }));
            IconList.Add(new AppIcon(Enums.IconButtonType.StatShare, Common.GetIconText(Enums.IconButtonType.StatShare), "/Assets/share.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Stats, Enums.Screen.GameStats }));

            // Timeline
            IconList.Add(new AppIcon(Enums.IconButtonType.Goal, Common.GetIconText(Enums.IconButtonType.Goal), "/Assets/soccerBall2.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.TimeLine }));
            IconList.Add(new AppIcon(Enums.IconButtonType.PenaltyGoal, Common.GetIconText(Enums.IconButtonType.PenaltyGoal), "/Assets/goalP.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.TimeLine }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Substitution, Common.GetIconText(Enums.IconButtonType.Substitution), "/Assets/sub.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.TimeLine }));

            // Boxscore
            IconList.Add(new AppIcon(Enums.IconButtonType.Legend, Common.GetIconText(Enums.IconButtonType.Legend), "/Assets/legend.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.Boxscore }));

            // Play Details / Shot Details
            IconList.Add(new AppIcon(Enums.IconButtonType.ShotWithLeftFoot, Common.GetIconText(Enums.IconButtonType.ShotWithLeftFoot), "/Assets/ShotWithLeftFoot.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.ShotWithRightFoot, Common.GetIconText(Enums.IconButtonType.ShotWithRightFoot), "/Assets/ShotWithRightFoot.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.ShotOnGoal, Common.GetIconText(Enums.IconButtonType.ShotOnGoal), "/Assets/ShotOnGoal.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.ShotNotOnGoal, Common.GetIconText(Enums.IconButtonType.ShotNotOnGoal), "/Assets/ShotNotOnGoal.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Header, Common.GetIconText(Enums.IconButtonType.Header), "/Assets/header.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            
            IconList.Add(new AppIcon(Enums.IconButtonType.Miss, Common.GetIconText(Enums.IconButtonType.Miss), "/Assets/miss.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.HitPost, Common.GetIconText(Enums.IconButtonType.HitPost), "/Assets/post.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.Blocked, Common.GetIconText(Enums.IconButtonType.Blocked), "/Assets/blocked.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));
            IconList.Add(new AppIcon(Enums.IconButtonType.ShotGoal, Common.GetIconText(Enums.IconButtonType.ShotGoal), "/Assets/goal.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.PlayDetails, Enums.Screen.ShotDetails }));

            IconList.Add(new AppIcon(Enums.IconButtonType.GMPeriod, Common.GetIconText(Enums.IconButtonType.GMPeriod), "/Assets/gmPeriod.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameManager }));
            IconList.Add(new AppIcon(Enums.IconButtonType.GMClock, Common.GetIconText(Enums.IconButtonType.GMClock), "/Assets/gmClock.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameManager }));
            IconList.Add(new AppIcon(Enums.IconButtonType.GMStartStop, Common.GetIconText(Enums.IconButtonType.GMStartStop), "/Assets/gmPlay.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameManager }));

            IconList.Add(new AppIcon(Enums.IconButtonType.Undo, Common.GetIconText(Enums.IconButtonType.Undo), "/Assets/Undo.png", new List<Enums.Screen>() { Enums.Screen.All, Enums.Screen.GameManager }));
            IconList.Add(new AppIcon(Enums.IconButtonType.EditPlayByPlay, Common.GetIconText(Enums.IconButtonType.EditPlayByPlay), "/Assets/Edit3.png", new List<Enums.Screen>() { Enums.Screen.GameManager }));

        }
    }
}
