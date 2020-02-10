using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class BoxscoreItem
    {
        public BoxscoreItem()
        {
            BackgroundColor = "Transparent";
            BackgroundOpacity = 0.3;

            UniformNumber_Pos = 0;
            PlayerName_Pos = 1;

            MIN_Pos = 0;
            Shot_Pos = 1;
            Shot_Miss_Pos = 2;
            Shot_Blocked_Pos = 3;
            Shot_Goal_Pos = 4;
            Hit_Post_Pos = 5;
            Pass_Pos = 6;
            Pass_Excellent_Pos = 7;
            Pass_Good_Pos = 8;
            Pass_Poor_Pos = 9;
            Turnover_Pos = 10;
            Turnover_IllegalThrowIn_Pos = 11;
            Turnover_LostDribble_Pos = 12;
            Offsides_Pos = 13;
            FoulCommitted_Pos = 14;
            FoulCommitted_Kicking_Pos = 15;
            FoulCommitted_Tripping_Pos = 16;
            FoulCommitted_Charging_Pos = 17;
            FoulCommitted_Pushing_Pos = 18;
            FoulCommitted_Holding_Pos = 19;
            FoulCommitted_IllegalTackle_Pos = 20;
            OutOfBounds_Pos = 21;
            Cross_Pos = 22;
            Cross_Excellent_Pos = 23;
            Cross_Good_Pos = 24;
            Cross_Poor_Pos = 25;
            ThrowIn_Pos = 26;
            CornerKick_Pos = 27;
            CornerKick_Excellent_Pos = 28;
            CornerKick_Good_Pos = 29;
            CornerKick_Poor_Pos = 30;
            CornerKick_ForGoal_Pos = 31;
            Tackle_Pos = 32;
            GoalieKick_Pos = 33;
            OwnGoal_Pos = 34;
            FoulDrawn_Pos = 35;
            DirectFreeKick_Pos = 36;
            DirectFreeKick_ForGoal_Pos = 37;
            DirectFreeKick_NotForGoal_Pos = 38;
            IndirectFreeKick_Pos = 39;
            PenaltyKick_Pos = 40;
            PenaltyKick_Miss_Pos = 41;
            PenaltyKick_HitPost_Pos = 42;
            PenaltyKick_Blocked_Pos = 43;
            PenaltyKick_Goal_Pos = 44;
            YellowCard_Pos = 45;
            YellowCard_UnsportsmanlikeConduct_Pos = 46;
            YellowCard_DelayingRestartofPlay_Pos = 47;
            RedCard_Pos = 48;
            RedCard_FoulPlay_Pos = 49;
            RedCard_ViolentConduct_Pos = 50;
            RedCard_IllegalHands_Pos = 51;
            RedCard_SecondYellowCard_Pos = 52;
            DropKick_Pos = 53;
            DropKick_Excellent_Pos = 54;
            DropKick_Good_Pos = 55;
            DropKick_Poor_Pos = 56;
            Dribble_Pos = 57;
            ShootoutKick_Pos = 58;
            ShootoutKick_Miss_Pos = 59;
            ShootoutKick_HitPost_Pos = 60;
            ShootoutKick_Blocked_Pos = 61;
            ShootoutKick_Goal_Pos = 62;
            Goal_Allowed_Pos = 63;
            Saves_Pos = 64;
            Shots_On_Goal_Pos = 65;
            PlusMinus_Pos = 66;

        }

        public int PlayerId { get; set; }
        
        public string BackgroundColor { get; set; }
        public double BackgroundOpacity { get; set; }

        public string UniformNumber { get; set; }
        public string PlayerName { get; set; }
        public string MIN { get; set; }
        public string Shot { get; set; }
        public string Shot_Miss { get; set; }
        public string Shot_Blocked { get; set; }
        public string Shot_Goal { get; set; }
        public string Hit_Post { get; set; }
        public string Pass { get; set; }
        public string Pass_Excellent { get; set; }
        public string Pass_Good { get; set; }
        public string Pass_Poor { get; set; }
        public string Turnover { get; set; }
        public string Turnover_IllegalThrowIn { get; set; }
        public string Turnover_LostDribble { get; set; }
        public string Offsides { get; set; }
        public string FoulCommitted { get; set; }
        public string FoulCommitted_Kicking { get; set; }
        public string FoulCommitted_Tripping { get; set; }
        public string FoulCommitted_Charging { get; set; }
        public string FoulCommitted_Pushing { get; set; }
        public string FoulCommitted_Holding { get; set; }
        public string FoulCommitted_IllegalTackle { get; set; }
        public string OutOfBounds { get; set; }
        public string Cross { get; set; }
        public string Cross_Excellent { get; set; }
        public string Cross_Good { get; set; }
        public string Cross_Poor { get; set; }
        public string ThrowIn { get; set; }
        public string CornerKick { get; set; }
        public string CornerKick_Excellent { get; set; }
        public string CornerKick_Good { get; set; }
        public string CornerKick_Poor { get; set; }
        public string CornerKick_ForGoal { get; set; }
        public string Tackle { get; set; }
        public string GoalieKick { get; set; }
        public string OwnGoal { get; set; }
        public string FoulDrawn { get; set; }
        public string DirectFreeKick { get; set; }
        public string DirectFreeKick_ForGoal { get; set; }
        public string DirectFreeKick_NotForGoal { get; set; }
        public string IndirectFreeKick { get; set; }
        public string PenaltyKick { get; set; }
        public string PenaltyKick_Miss { get; set; }
        public string PenaltyKick_HitPost { get; set; }
        public string PenaltyKick_Blocked { get; set; }
        public string PenaltyKick_Goal { get; set; }
        public string YellowCard { get; set; }
        public string YellowCard_UnsportsmanlikeConduct { get; set; }
        public string YellowCard_DelayingRestartofPlay { get; set; }
        public string RedCard { get; set; }
        public string RedCard_FoulPlay { get; set; }
        public string RedCard_ViolentConduct { get; set; }
        public string RedCard_IllegalHands { get; set; }
        public string RedCard_SecondYellowCard { get; set; }
        public string DropKick { get; set; }
        public string DropKick_Excellent { get; set; }
        public string DropKick_Good { get; set; }
        public string DropKick_Poor { get; set; }
        public string Dribble { get; set; }
        public string ShootoutKick { get; set; }
        public string ShootoutKick_Miss { get; set; }
        public string ShootoutKick_HitPost { get; set; }
        public string ShootoutKick_Blocked { get; set; }
        public string ShootoutKick_Goal { get; set; }
        public string Goal_Allowed { get; set; }
        public string Saves { get; set; }
        public string Shots_On_Goal { get; set; }
        public string PlusMinus { get; set; }


        public int UniformNumber_Pos { get; set; }
        public int PlayerName_Pos { get; set; }
        public int MIN_Pos { get; set; }
        public int Shot_Pos { get; set; }
        public int Shot_Miss_Pos { get; set; }
        public int Shot_Blocked_Pos { get; set; }
        public int Shot_Goal_Pos { get; set; }
        public int Hit_Post_Pos { get; set; }
        public int Pass_Pos { get; set; }
        public int Pass_Excellent_Pos { get; set; }
        public int Pass_Good_Pos { get; set; }
        public int Pass_Poor_Pos { get; set; }
        public int Turnover_Pos { get; set; }
        public int Turnover_IllegalThrowIn_Pos { get; set; }
        public int Turnover_LostDribble_Pos { get; set; }
        public int Offsides_Pos { get; set; }
        public int FoulCommitted_Pos { get; set; }
        public int FoulCommitted_Kicking_Pos { get; set; }
        public int FoulCommitted_Tripping_Pos { get; set; }
        public int FoulCommitted_Charging_Pos { get; set; }
        public int FoulCommitted_Pushing_Pos { get; set; }
        public int FoulCommitted_Holding_Pos { get; set; }
        public int FoulCommitted_IllegalTackle_Pos { get; set; }
        public int OutOfBounds_Pos { get; set; }
        public int Cross_Pos { get; set; }
        public int Cross_Excellent_Pos { get; set; }
        public int Cross_Good_Pos { get; set; }
        public int Cross_Poor_Pos { get; set; }
        public int ThrowIn_Pos { get; set; }
        public int CornerKick_Pos { get; set; }
        public int CornerKick_Excellent_Pos { get; set; }
        public int CornerKick_Good_Pos { get; set; }
        public int CornerKick_Poor_Pos { get; set; }
        public int CornerKick_ForGoal_Pos { get; set; }
        public int Tackle_Pos { get; set; }
        public int GoalieKick_Pos { get; set; }
        public int OwnGoal_Pos { get; set; }
        public int FoulDrawn_Pos { get; set; }
        public int DirectFreeKick_Pos { get; set; }
        public int DirectFreeKick_ForGoal_Pos { get; set; }
        public int DirectFreeKick_NotForGoal_Pos { get; set; }
        public int IndirectFreeKick_Pos { get; set; }
        public int PenaltyKick_Pos { get; set; }
        public int PenaltyKick_Miss_Pos { get; set; }
        public int PenaltyKick_HitPost_Pos { get; set; }
        public int PenaltyKick_Blocked_Pos { get; set; }
        public int PenaltyKick_Goal_Pos { get; set; }
        public int YellowCard_Pos { get; set; }
        public int YellowCard_UnsportsmanlikeConduct_Pos { get; set; }
        public int YellowCard_DelayingRestartofPlay_Pos { get; set; }
        public int RedCard_Pos { get; set; }
        public int RedCard_FoulPlay_Pos { get; set; }
        public int RedCard_ViolentConduct_Pos { get; set; }
        public int RedCard_IllegalHands_Pos { get; set; }
        public int RedCard_SecondYellowCard_Pos { get; set; }
        public int DropKick_Pos { get; set; }
        public int DropKick_Excellent_Pos { get; set; }
        public int DropKick_Good_Pos { get; set; }
        public int DropKick_Poor_Pos { get; set; }
        public int Dribble_Pos { get; set; }
        public int ShootoutKick_Pos { get; set; }
        public int ShootoutKick_Miss_Pos { get; set; }
        public int ShootoutKick_HitPost_Pos { get; set; }
        public int ShootoutKick_Blocked_Pos { get; set; }
        public int ShootoutKick_Goal_Pos { get; set; }
        public int Goal_Allowed_Pos { get; set; }
        public int Saves_Pos { get; set; }
        public int Shots_On_Goal_Pos { get; set; }
        public int PlusMinus_Pos { get; set; }


    }
}
