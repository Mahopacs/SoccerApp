using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class FlatTotals : Notification
    {

        public FlatTotals()
        {

            MinutesPlayedTotal_Pos = 1;
            PlusMinusTotal_Pos = 2;
            TotalGoals_Pos = 3;
            AssistTotal_Pos = 4;
            ShotOnGoalTotal_Pos = 5;
            ShotTotal_Pos = 6;
            ShotMiss_Pos = 7;
            ShotHitPost_Pos = 8;
            ShotBlocked_Pos = 9;
            PassTotal_Pos = 10;
            PassGood_Pos = 11;
            PassExcellent_Pos = 12;
            PassPoor_Pos = 13;
            TurnoverTotal_Pos = 14;
            TurnoverIllegalThrowIn_Pos = 15;
            TurnoverLostDribble_Pos = 16;
            OffsidesTotal_Pos = 17;
            FoulCommittedTotal_Pos = 18;
            FoulCommittedKicking_Pos = 19;
            FoulCommittedTripping_Pos = 20;
            FoulCommittedCharging_Pos = 21;
            FoulCommittedPushing_Pos = 22;
            FoulCommittedHolding_Pos = 23;
            FoulCommittedIllegalTackle_Pos = 24;
            OutOfBoundsTotal_Pos = 25;
            CrossTotal_Pos = 26;
            CrossExcellent_Pos = 27;
            CrossPoor_Pos = 28;
            CrossGood_Pos = 29;
            ThrowInTotal_Pos = 30;
            CornerKickTotal_Pos = 31;
            CornerKickExcellent_Pos = 32;
            CornerKickGood_Pos = 33;
            CornerKickPoor_Pos = 34;
            CornerKickForGoal_Pos = 35;
            TackleTotal_Pos = 36;
            GoalieKickTotal_Pos = 37;
            OwnGoalTotal_Pos = 38;
            FoulDrawnTotal_Pos = 39;
            DirectFreeKickTotal_Pos = 40;
            DirectFreeKickNotForGoal_Pos = 41;
            DirectFreeKickForGoal_Pos = 42;
            IndirectFreeKickTotal_Pos = 43;
            PenaltyKickTotal_Pos = 44;
            PenaltyKickMiss_Pos = 45;
            PenaltyKickHitPost_Pos = 46;
            PenaltyKickBlocked_Pos = 47;
            PenaltyKickGoal_Pos = 48;
            YellowCardTotal_Pos = 49;
            YellowCardUnsportsmanLikeConduct_Pos = 50;
            YellowCardDelayingRestartOfPlay_Pos = 51;
            RedCardTotal_Pos = 52;
            RedCardFoulPlay_Pos = 53;
            RedCardViolentConduct_Pos = 54;
            RedCardIllegalHands_Pos = 55;
            RedCardSecondYellowCard_Pos = 56;
            DropKickTotal_Pos = 57;
            DropKickExcellent_Pos = 58;
            DropKickGood_Pos = 59;
            DropKickPoor_Pos = 60;
            DribbleTotal_Pos = 61;
            ShootoutKickTotal_Pos = 62;
            ShootoutKickMiss_Pos = 63;
            ShootoutKickHitPost_Pos = 64;
            ShootoutKickBlocked_Pos = 65;
            ShootoutKickGoal_Pos = 66;
            BlockedTotal_Pos = 67;
            GoalAllowedTotal_Pos = 68;
            SaveTotal_Pos = 69;
            SubstitutionTotal_Pos = 70;
            MoveTotal_Pos = 71;


        }

        #region "Properties"

        private int _gameStarted;
        public int GameStarted
        {
            get { return _gameStarted; }
            set { _gameStarted = value; NotifyPropertyChanged("GameStarted"); }
        }

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        private int _playerID;
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; NotifyPropertyChanged("PlayerID"); }
        }

        private int _teamID;
        public int TeamID
        {
            get { return _teamID; }
            set { _teamID = value; NotifyPropertyChanged("TeamID"); }
        }

        private int _totalGoals;
        public int TotalGoals
        {
            get { return _totalGoals; }
            set { _totalGoals = value; NotifyPropertyChanged("TotalGoals"); }
        }

        private int _shotTotal;
        public int ShotTotal
        {
            get { return _shotTotal; }
            set { _shotTotal = value; NotifyPropertyChanged("ShotTotal"); }
        }

        private int _shotMiss;
        public int ShotMiss
        {
            get { return _shotMiss; }
            set { _shotMiss = value; NotifyPropertyChanged("ShotMiss"); }
        }

        private int _shotHitPost;
        public int ShotHitPost
        {
            get { return _shotHitPost; }
            set { _shotHitPost = value; NotifyPropertyChanged("ShotHitPost"); }
        }

        private int _shotBlocked;
        public int ShotBlocked
        {
            get { return _shotBlocked; }
            set { _shotBlocked = value; NotifyPropertyChanged("ShotBlocked"); }
        }

        private int _shotGoal;
        public int ShotGoal
        {
            get { return _shotGoal; }
            set { _shotGoal = value; NotifyPropertyChanged("ShotGoal"); }
        }

        private int _passTotal;
        public int PassTotal
        {
            get { return _passTotal; }
            set { _passTotal = value; NotifyPropertyChanged("PassTotal"); }
        }

        private int _passGood;
        public int PassGood
        {
            get { return _passGood; }
            set { _passGood = value; NotifyPropertyChanged("PassGood"); }
        }

        private int _passExcellent;
        public int PassExcellent
        {
            get { return _passExcellent; }
            set { _passExcellent = value; NotifyPropertyChanged("PassExcellent"); }
        }

        private int _passPoor;
        public int PassPoor
        {
            get { return _passPoor; }
            set { _passPoor = value; NotifyPropertyChanged("PassPoor"); }
        }

        private int _turnoverTotal;
        public int TurnoverTotal
        {
            get { return _turnoverTotal; }
            set { _turnoverTotal = value; NotifyPropertyChanged("TurnoverTotal"); }
        }

        private int _turnoverIllegalThrowIn;
        public int TurnoverIllegalThrowIn
        {
            get { return _turnoverIllegalThrowIn; }
            set { _turnoverIllegalThrowIn = value; NotifyPropertyChanged("TurnoverIllegalThrowIn"); }
        }

        private int _turnoverLostDribble;
        public int TurnoverLostDribble
        {
            get { return _turnoverLostDribble; }
            set { _turnoverLostDribble = value; NotifyPropertyChanged("TurnoverLostDribble"); }
        }

        private int _offsidesTotal;
        public int OffsidesTotal
        {
            get { return _offsidesTotal; }
            set { _offsidesTotal = value; NotifyPropertyChanged("OffsidesTotal"); }
        }

        private int _foulCommittedTotal;
        public int FoulCommittedTotal
        {
            get { return _foulCommittedTotal; }
            set { _foulCommittedTotal = value; NotifyPropertyChanged("FoulCommittedTotal"); }
        }

        private int _foulCommittedKicking;
        public int FoulCommittedKicking
        {
            get { return _foulCommittedKicking; }
            set { _foulCommittedKicking = value; NotifyPropertyChanged("FoulCommittedKicking"); }
        }

        private int _foulCommittedTripping;
        public int FoulCommittedTripping
        {
            get { return _foulCommittedTripping; }
            set { _foulCommittedTripping = value; NotifyPropertyChanged("FoulCommittedTripping"); }
        }

        private int _foulCommittedCharging;
        public int FoulCommittedCharging
        {
            get { return _foulCommittedCharging; }
            set { _foulCommittedCharging = value; NotifyPropertyChanged("FoulCommittedCharging"); }
        }

        private int _foulCommittedPushing;
        public int FoulCommittedPushing
        {
            get { return _foulCommittedPushing; }
            set { _foulCommittedPushing = value; NotifyPropertyChanged("FoulCommittedPushing"); }
        }

        private int _foulCommittedHolding;
        public int FoulCommittedHolding
        {
            get { return _foulCommittedHolding; }
            set { _foulCommittedHolding = value; NotifyPropertyChanged("FoulCommittedHolding"); }
        }

        private int _foulCommittedIllegalTackle;
        public int FoulCommittedIllegalTackle
        {
            get { return _foulCommittedIllegalTackle; }
            set { _foulCommittedIllegalTackle = value; NotifyPropertyChanged("FoulCommittedIllegalTackle"); }
        }

        private int _outOfBoundsTotal;
        public int OutOfBoundsTotal
        {
            get { return _outOfBoundsTotal; }
            set { _outOfBoundsTotal = value; NotifyPropertyChanged("OutOfBoundsTotal"); }
        }

        private int _crossTotal;
        public int CrossTotal
        {
            get { return _crossTotal; }
            set { _crossTotal = value; NotifyPropertyChanged("CrossTotal"); }
        }

        private int _crossExcellent;
        public int CrossExcellent
        {
            get { return _crossExcellent; }
            set { _crossExcellent = value; NotifyPropertyChanged("CrossExcellent"); }
        }

        private int _crossPoor;
        public int CrossPoor
        {
            get { return _crossPoor; }
            set { _crossPoor = value; NotifyPropertyChanged("CrossPoor"); }
        }

        private int _crossGood;
        public int CrossGood
        {
            get { return _crossGood; }
            set { _crossGood = value; NotifyPropertyChanged("CrossGood"); }
        }

        private int _throwInTotal;
        public int ThrowInTotal
        {
            get { return _throwInTotal; }
            set { _throwInTotal = value; NotifyPropertyChanged("ThrowInTotal"); }
        }

        private int _cornerKickTotal;
        public int CornerKickTotal
        {
            get { return _cornerKickTotal; }
            set { _cornerKickTotal = value; NotifyPropertyChanged("CornerKickTotal"); }
        }

        private int _cornerKickExcellent;
        public int CornerKickExcellent
        {
            get { return _cornerKickExcellent; }
            set { _cornerKickExcellent = value; NotifyPropertyChanged("CornerKickExcellent"); }
        }

        private int _cornerKickGood;
        public int CornerKickGood
        {
            get { return _cornerKickGood; }
            set { _cornerKickGood = value; NotifyPropertyChanged("CornerKickGood"); }
        }

        private int _cornerKickPoor;
        public int CornerKickPoor
        {
            get { return _cornerKickPoor; }
            set { _cornerKickPoor = value; NotifyPropertyChanged("CornerKickPoor"); }
        }

        private int _cornerKickForGoal;
        public int CornerKickForGoal
        {
            get { return _cornerKickForGoal; }
            set { _cornerKickForGoal = value; NotifyPropertyChanged("CornerKickForGoal"); }
        }

        private int _tackleTotal;
        public int TackleTotal
        {
            get { return _tackleTotal; }
            set { _tackleTotal = value; NotifyPropertyChanged("TackleTotal"); }
        }

        private int _goalieKickTotal;
        public int GoalieKickTotal
        {
            get { return _goalieKickTotal; }
            set { _goalieKickTotal = value; NotifyPropertyChanged("GoalieKickTotal"); }
        }

        private int _ownGoalTotal;
        public int OwnGoalTotal
        {
            get { return _ownGoalTotal; }
            set { _ownGoalTotal = value; NotifyPropertyChanged("OwnGoalTotal"); }
        }

        private int _foulDrawnTotal;
        public int FoulDrawnTotal
        {
            get { return _foulDrawnTotal; }
            set { _foulDrawnTotal = value; NotifyPropertyChanged("FoulDrawnTotal"); }
        }

        private int _directFreeKickTotal;
        public int DirectFreeKickTotal
        {
            get { return _directFreeKickTotal; }
            set { _directFreeKickTotal = value; NotifyPropertyChanged("DirectFreeKickTotal"); }
        }

        private int _directFreeKickNotForGoal;
        public int DirectFreeKickNotForGoal
        {
            get { return _directFreeKickNotForGoal; }
            set { _directFreeKickNotForGoal = value; NotifyPropertyChanged("DirectFreeKickNotForGoal"); }
        }
    
        private int _directFreeKickForGoal;
        public int DirectFreeKickForGoal
        {
            get { return _directFreeKickForGoal; }
            set { _directFreeKickForGoal = value; NotifyPropertyChanged("DirectFreeKickForGoal"); }
        }

        private int _indirectFreeKickTotal;
        public int IndirectFreeKickTotal
        {
            get { return _indirectFreeKickTotal; }
            set { _indirectFreeKickTotal = value; NotifyPropertyChanged("IndirectFreeKickTotal"); }
        }

        private int _penaltyKickTotal;
        public int PenaltyKickTotal
        {
            get { return _penaltyKickTotal; }
            set { _penaltyKickTotal = value; NotifyPropertyChanged("PenaltyKickTotal"); }
        }

        private int _penaltyKickMiss;
        public int PenaltyKickMiss
        {
            get { return _penaltyKickMiss; }
            set { _penaltyKickMiss = value; NotifyPropertyChanged("PenaltyKickMiss"); }
        }

        private int _penaltyKickHitPost;
        public int PenaltyKickHitPost
        {
            get { return _penaltyKickHitPost; }
            set { _penaltyKickHitPost = value; NotifyPropertyChanged("PenaltyKickHitPost"); }
        }

        private int _penaltyKickBlocked;
        public int PenaltyKickBlocked
        {
            get { return _penaltyKickBlocked; }
            set { _penaltyKickBlocked = value; NotifyPropertyChanged("PenaltyKickBlocked"); }
        }

        private int _penaltyKickGoal;
        public int PenaltyKickGoal
        {
            get { return _penaltyKickGoal; }
            set { _penaltyKickGoal = value; NotifyPropertyChanged("PenaltyKickGoal"); }
        }

        private int _yellowCardTotal;
        public int YellowCardTotal
        {
            get { return _yellowCardTotal; }
            set { _yellowCardTotal = value; NotifyPropertyChanged("YellowCardTotal"); }
        }

        private int _yellowCardUnsportsmanLikeConduct;
        public int YellowCardUnsportsmanLikeConduct
        {
            get { return _yellowCardUnsportsmanLikeConduct; }
            set { _yellowCardUnsportsmanLikeConduct = value; NotifyPropertyChanged("YellowCardUnsportsmanLikeConduct"); }
        }

        private int _yellowCardDelayingRestartOfPlay;
        public int YellowCardDelayingRestartOfPlay
        {
            get { return _yellowCardDelayingRestartOfPlay; }
            set { _yellowCardDelayingRestartOfPlay = value; NotifyPropertyChanged("YellowCardDelayingRestartOfPlay"); }
        }

        private int _redCardTotal;
        public int RedCardTotal
        {
            get { return _redCardTotal; }
            set { _redCardTotal = value; NotifyPropertyChanged("RedCardTotal"); }
        }

        private int _redCardFoulPlay;
        public int RedCardFoulPlay
        {
            get { return _redCardFoulPlay; }
            set { _redCardFoulPlay = value; NotifyPropertyChanged("RedCardFoulPlay"); }
        }

        private int _redCardViolentConduct;
        public int RedCardViolentConduct
        {
            get { return _redCardViolentConduct; }
            set { _redCardViolentConduct = value; NotifyPropertyChanged("RedCardViolentConduct"); }
        }

        private int _redCardIllegalHands;
        public int RedCardIllegalHands
        {
            get { return _redCardIllegalHands; }
            set { _redCardIllegalHands = value; NotifyPropertyChanged("RedCardIllegalHands"); }
        }

        private int _redCardSecondYellowCard;
        public int RedCardSecondYellowCard
        {
            get { return _redCardSecondYellowCard; }
            set { _redCardSecondYellowCard = value; NotifyPropertyChanged("RedCardSecondYellowCard"); }
        }

        private int _dropKickTotal;
        public int DropKickTotal
        {
            get { return _dropKickTotal; }
            set { _dropKickTotal = value; NotifyPropertyChanged("DropKickTotal"); }
        }

        private int _dropKickExcellent;
        public int DropKickExcellent
        {
            get { return _dropKickExcellent; }
            set { _dropKickExcellent = value; NotifyPropertyChanged("DropKickExcellent"); }
        }

        private int _dropKickGood;
        public int DropKickGood
        {
            get { return _dropKickGood; }
            set { _dropKickGood = value; NotifyPropertyChanged("DropKickGood"); }
        }

        private int _dropKickPoor;
        public int DropKickPoor
        {
            get { return _dropKickPoor; }
            set { _dropKickPoor = value; NotifyPropertyChanged("DropKickPoor"); }
        }

        private int _dribbleTotal;
        public int DribbleTotal
        {
            get { return _dribbleTotal; }
            set { _dribbleTotal = value; NotifyPropertyChanged("DribbleTotal"); }
        }

        private int _shootoutKickTotal;
        public int ShootoutKickTotal
        {
            get { return _shootoutKickTotal; }
            set { _shootoutKickTotal = value; NotifyPropertyChanged("ShootoutKickTotal"); }
        }

        private int _shootoutKickMiss;
        public int ShootoutKickMiss
        {
            get { return _shootoutKickMiss; }
            set { _shootoutKickMiss = value; NotifyPropertyChanged("ShootoutKickMiss"); }
        }

        private int _shootoutKickHitPost;
        public int ShootoutKickHitPost
        {
            get { return _shootoutKickHitPost; }
            set { _shootoutKickHitPost = value; NotifyPropertyChanged("ShootoutKickHitPost"); }
        }

        private int _shootoutKickBlocked;
        public int ShootoutKickBlocked
        {
            get { return _shootoutKickBlocked; }
            set { _shootoutKickBlocked = value; NotifyPropertyChanged("ShootoutKickBlocked"); }
        }

        private int _shootoutKickGoal;
        public int ShootoutKickGoal
        {
            get { return _shootoutKickGoal; }
            set { _shootoutKickGoal = value; NotifyPropertyChanged("ShootoutKickGoal"); }
        }

        private int _secondsPlayedTotal;
        public int SecondsPlayedTotal
        {
            get { return _secondsPlayedTotal; }
            set { _secondsPlayedTotal = value; NotifyPropertyChanged("SecondsPlayedTotal"); }
        }

        private int _minutesPlayedTotal;
        public int MinutesPlayedTotal
        {
            get { return _minutesPlayedTotal; }
            set { _minutesPlayedTotal = value; NotifyPropertyChanged("MinutesPlayedTotal"); }
        }

        
        public string MinutesPlayedTotal_mmm_ss
        {
            get 
            {
                string toReturn = "00:00";
                int m = _secondsPlayedTotal / 60;

                toReturn = m.ToString() + ":";

                if (m < 10)
                {
                    toReturn = m.ToString().PadLeft(2, '0') + ":";
                }


                TimeSpan t = TimeSpan.FromSeconds(_secondsPlayedTotal);
                toReturn = toReturn + t.Seconds.ToString().PadLeft(2, '0');


                return toReturn; 
            }

        }

        private int _plusMinusTotal;
        public int PlusMinusTotal
        {
            get { return _plusMinusTotal; }
            set { _plusMinusTotal = value; NotifyPropertyChanged("PlusMinusTotal"); }
        }

        private int _blockedTotal;
        public int BlockedTotal
        {
            get { return _blockedTotal; }
            set { _blockedTotal = value; NotifyPropertyChanged("BlockedTotal"); }
        }

        private int _goalAllowedTotal;
        public int GoalAllowedTotal
        {
            get { return _goalAllowedTotal; }
            set { _goalAllowedTotal = value; NotifyPropertyChanged("GoalAllowedTotal"); }
        }

        private int _saveTotal;
        public int SaveTotal
        {
            get { return _saveTotal; }
            set { _saveTotal = value; NotifyPropertyChanged("SaveTotal"); }
        }

        private int _assistTotal;
        public int AssistTotal
        {
            get { return _assistTotal; }
            set { _assistTotal = value; NotifyPropertyChanged("AssistTotal"); }
        }

        private int _shotOnGoalTotal;
        public int ShotOnGoalTotal
        {
            get { return _shotOnGoalTotal; }
            set { _shotOnGoalTotal = value; NotifyPropertyChanged("ShotOnGoalTotal"); }
        }

        private int _substitutionTotal;
        public int SubstitutionTotal
        {
            get { return _substitutionTotal; }
            set { _substitutionTotal = value; NotifyPropertyChanged("SubstitutionTotal"); }
        }

        private int _moveTotal;
        public int MoveTotal
        {
            get { return _moveTotal; }
            set { _moveTotal = value; NotifyPropertyChanged("MoveTotal"); }
        }

        private string _onCloud;
        public string OnCloud
        {
            get { return _onCloud; }
            set { _onCloud = value; NotifyPropertyChanged("OnCloud"); }
        }
        
        public int UniformNumber_Pos { get; set; }
        public int PlayerName_Pos { get; set; }
        public int GameStarted_Pos { get; set; }
        public int MinutesPlayedTotal_Pos { get; set; }
        public int SecondsPlayedTotal_Pos { get; set; }
        public int PlusMinusTotal_Pos { get; set; }
        public int TotalGoals_Pos { get; set; }
        public int ShotTotal_Pos { get; set; }
        public int ShotMiss_Pos { get; set; }
        public int ShotHitPost_Pos { get; set; }
        public int ShotBlocked_Pos { get; set; }
        public int ShotGoal_Pos { get; set; }
        public int PassTotal_Pos { get; set; }
        public int PassGood_Pos { get; set; }
        public int PassExcellent_Pos { get; set; }
        public int PassPoor_Pos { get; set; }
        public int TurnoverTotal_Pos { get; set; }
        public int TurnoverIllegalThrowIn_Pos { get; set; }
        public int TurnoverLostDribble_Pos { get; set; }
        public int OffsidesTotal_Pos { get; set; }
        public int FoulCommittedTotal_Pos { get; set; }
        public int FoulCommittedKicking_Pos { get; set; }
        public int FoulCommittedTripping_Pos { get; set; }
        public int FoulCommittedCharging_Pos { get; set; }
        public int FoulCommittedPushing_Pos { get; set; }
        public int FoulCommittedHolding_Pos { get; set; }
        public int FoulCommittedIllegalTackle_Pos { get; set; }
        public int OutOfBoundsTotal_Pos { get; set; }
        public int CrossTotal_Pos { get; set; }
        public int CrossExcellent_Pos { get; set; }
        public int CrossPoor_Pos { get; set; }
        public int CrossGood_Pos { get; set; }
        public int ThrowInTotal_Pos { get; set; }
        public int CornerKickTotal_Pos { get; set; }
        public int CornerKickExcellent_Pos { get; set; }
        public int CornerKickGood_Pos { get; set; }
        public int CornerKickPoor_Pos { get; set; }
        public int CornerKickForGoal_Pos { get; set; }
        public int TackleTotal_Pos { get; set; }
        public int GoalieKickTotal_Pos { get; set; }
        public int OwnGoalTotal_Pos { get; set; }
        public int FoulDrawnTotal_Pos { get; set; }
        public int DirectFreeKickTotal_Pos { get; set; }
        public int DirectFreeKickNotForGoal_Pos { get; set; }
        public int DirectFreeKickForGoal_Pos { get; set; }
        public int IndirectFreeKickTotal_Pos { get; set; }
        public int PenaltyKickTotal_Pos { get; set; }
        public int PenaltyKickMiss_Pos { get; set; }
        public int PenaltyKickHitPost_Pos { get; set; }
        public int PenaltyKickBlocked_Pos { get; set; }
        public int PenaltyKickGoal_Pos { get; set; }
        public int YellowCardTotal_Pos { get; set; }
        public int YellowCardUnsportsmanLikeConduct_Pos { get; set; }
        public int YellowCardDelayingRestartOfPlay_Pos { get; set; }
        public int RedCardTotal_Pos { get; set; }
        public int RedCardFoulPlay_Pos { get; set; }
        public int RedCardViolentConduct_Pos { get; set; }
        public int RedCardIllegalHands_Pos { get; set; }
        public int RedCardSecondYellowCard_Pos { get; set; }
        public int DropKickTotal_Pos { get; set; }
        public int DropKickExcellent_Pos { get; set; }
        public int DropKickGood_Pos { get; set; }
        public int DropKickPoor_Pos { get; set; }
        public int DribbleTotal_Pos { get; set; }
        public int ShootoutKickTotal_Pos { get; set; }
        public int ShootoutKickMiss_Pos { get; set; }
        public int ShootoutKickHitPost_Pos { get; set; }
        public int ShootoutKickBlocked_Pos { get; set; }
        public int ShootoutKickGoal_Pos { get; set; }
        public int BlockedTotal_Pos { get; set; }
        public int GoalAllowedTotal_Pos { get; set; }
        public int SaveTotal_Pos { get; set; }
        public int AssistTotal_Pos { get; set; }
        public int ShotOnGoalTotal_Pos { get; set; }
        public int SubstitutionTotal_Pos { get; set; }
        public int MoveTotal_Pos { get; set; }


        #endregion "Properties"
    }
}



