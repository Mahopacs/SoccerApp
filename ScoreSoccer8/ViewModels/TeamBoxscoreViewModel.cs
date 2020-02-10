using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.Classes;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataObjects.DbClasses;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Globalization;

namespace ScoreSoccer8.ViewModels
{
    public class TeamBoxscoreViewModel : Notification
    {
        public int _gameId;
        private string _gameTitleTemplate = "{0}";

        public void Initialize(int gameId, int teamId)
        {

            _gameId = gameId;
            _game = DAL.Instance().GetGame(_gameId);

            if (_game.AwayTeam.TeamID == teamId)
            {
                AwayTeamName = _game.AwayTeam.TeamName;
                AwayTeamShortName = Game.AwayTeam.TeamShortName;
                AwayTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.AwayTeam.JerseyID).ImagePath;
                GameTitle = string.Format(_gameTitleTemplate, Game.AwayTeam.TeamName);
                ShowAwayCircle = "Visible";
                ShowHomeCircle = "Collapsed";
            }
            else
            {
                HomeTeamName = _game.HomeTeam.TeamName;
                HomeTeamShortName = Game.HomeTeam.TeamShortName;
                HomeTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.HomeTeam.JerseyID).ImagePath;
                GameTitle = string.Format(_gameTitleTemplate, Game.HomeTeam.TeamName);
                ShowHomeCircle = "Visible";
                ShowAwayCircle = "Collapsed";
            }            
      
            BoxscoreItems = LoadPlayerSats(teamId);
            LoadLegend();

        }

        private void LoadLegend()
        {
            LegendList = Common.Instance().GetLegendList();
        }

        private ObservableCollection<FlatTotalsModel> LoadPlayerSats(int teamId)
        {

            ObservableCollection<FlatTotalsModel> players = DAL.Instance().GetGamesPlayerFlatStats(_gameId, teamId);

            int i = 0;
            foreach (FlatTotalsModel item in players)
            {
                if (i % 2 != 0)
                {
                    item.BackgroundColor = "White";
                    item.BackgroundOpacity = 0.3;
                }

                i++;
            }

            return players;

        }

        private ICommand _showLegendCommand;
        public ICommand ShowLegendCommand
        {
            get
            {
                if (_showLegendCommand == null)
                {
                    _showLegendCommand = new DelegateCommand(param => this.ToShowPopup(), param => true);
                }

                return _showLegendCommand;
            }
        }


        private ICommand _exportToExcelCommand;
        public ICommand ExportToExcelCommand
        {
            get
            {
                if (_exportToExcelCommand == null)
                {
                    _exportToExcelCommand = new DelegateCommand(param => this.ExportToExcel(), param => true);
                }

                return _exportToExcelCommand;
            }
        }

        private void ToShowPopup()
        {
            if (ShowPopup == "Visible")
            {
                ShowPopup = "Collapsed";
            }
            else
            {
                ShowPopup = "Visible";
            }
        }

        private void ExportToExcel2()
        {
            List<ExportBoxScoreItem> a = new List<ExportBoxScoreItem>();
            ExportBoxScoreItem b = new ExportBoxScoreItem();
            ExportBoxScoreItem c = new ExportBoxScoreItem();

            b.a = "a";
            b.b = "aa";

            c.a = "b";
            c.b = "bb";

            a.Add(b);
            a.Add(c);
            
            //var csv = new ExportToCSV<ExportBoxScoreItem>(Common.GetFlatBoxscoreItems(BoxscoreItems));

            //var csv = new ExportToCSV<ExportBoxScoreItem>(a);
            // csv.ExportToFile("myexportresult.xlsx");
        }


        private void ExportToExcel()
        {
          //  SpreadsheetDocument doc = new SpreadsheetDocument();
            //doc.ApplicationName = "SilverSpreadsheet";
            //doc.Creator = "Chris Klug";
            //doc.Company = "Intergen";

            //SharedStringDefinition str1 = doc.Workbook.SharedStrings.AddString("Column 1");
            //SharedStringDefinition str2 = doc.Workbook.SharedStrings.AddString("Column 2");
            //SharedStringDefinition str3 = doc.Workbook.SharedStrings.AddString("Column 3");

            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[0].SetValue(str1);
            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[1].SetValue(str2);
            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[2].SetValue(str3);

            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[0].SetValue("Column 1");
            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[1].SetValue("Column 2");
            //doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[2].SetValue("Column 3");

            //doc.Workbook.Sheets[0].Sheet.Rows[1].Cells[0].SetValue("Value 1");
            //doc.Workbook.Sheets[0].Sheet.Rows[1].Cells[1].SetValue(1);
            //doc.Workbook.Sheets[0].Sheet.Rows[1].Cells[2].SetValue(1001);

            //doc.Workbook.Sheets[0].Sheet.Rows[2].Cells[0].SetValue("Value 2");
            //doc.Workbook.Sheets[0].Sheet.Rows[2].Cells[1].SetValue(2);
            //doc.Workbook.Sheets[0].Sheet.Rows[2].Cells[2].SetValue(1002);

            //doc.Workbook.Sheets[0].Sheet.Rows[3].Cells[0].SetValue("Value 3");
            //doc.Workbook.Sheets[0].Sheet.Rows[3].Cells[1].SetValue(3);
            //doc.Workbook.Sheets[0].Sheet.Rows[3].Cells[2].SetValue(1003);

            //doc.Workbook.Sheets[0].Sheet.Rows[4].Cells[0].SetValue("Value 4");
            //doc.Workbook.Sheets[0].Sheet.Rows[4].Cells[1].SetValue(4);
            //doc.Workbook.Sheets[0].Sheet.Rows[4].Cells[2].SetValue(1004);

            //TablePart table = doc.Workbook.Sheets[0].Sheet.AddTable("My Table", "My Table", doc.Workbook.Sheets[0].Sheet.Rows[0].Cells[0], doc.Workbook.Sheets[0].Sheet.Rows[4].Cells[2]);
            //table.TableColumns[0].Name = str1.String;
            //table.TableColumns[1].Name = str2.String;
            //table.TableColumns[2].Name = str3.String;

            //doc.Workbook.Sheets[0].Sheet.AddColumnSizeDefinition(0, 2, 20);

            //doc.Workbook.Sheets[0].Sheet.Rows[5].Cells[1].SetValue("Sum:");
            //doc.Workbook.Sheets[0].Sheet.Rows[5].Cells[2].Formula = "SUM(" + doc.Workbook.Sheets[0].Sheet.Rows[1].Cells[2].CellName + ":" + doc.Workbook.Sheets[0].Sheet.Rows[4].Cells[2].CellName + ")";

            
            //List<ExportBoxScoreItem> a = new List<ExportBoxScoreItem>();
            //var csv = new ExportToCSV<ExportBoxScoreItem>(a);

            //csv.SaveXlsxToIsoStoreAndLaunchInExcel(Common.GetFlatBoxscoreItems(BoxscoreItems));
        }













        #region Properties

        private GameModel _game;
        public GameModel Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
        }

        private ObservableCollection<FlatTotalsModel> _boxscoreItems = new ObservableCollection<FlatTotalsModel>();
        public ObservableCollection<FlatTotalsModel> BoxscoreItems
        {
            get { return _boxscoreItems; }
            set { _boxscoreItems = value; NotifyPropertyChanged("BoxscoreItems"); }
        }

        private ObservableCollection<Legend> _legendList = new ObservableCollection<Legend>();
        public ObservableCollection<Legend> LegendList
        {
            get { return _legendList; }
            set { _legendList = value; NotifyPropertyChanged("LegendList"); }
        }

        private string _showPopup = "Collapsed";
        public string ShowPopup
        {
            get { return _showPopup; }
            set { _showPopup = value; NotifyPropertyChanged("ShowPopup"); }
        }

        private string _awayTeamName;
        public string AwayTeamName
        {
            get { return _awayTeamName; }
            set { _awayTeamName = value; NotifyPropertyChanged("AwayTeamName"); }
        }

        private string _homeTeamName;
        public string HomeTeamName
        {
            get { return _homeTeamName; }
            set { _homeTeamName = value; NotifyPropertyChanged("HomeTeamName"); }
        }

        private string _awayTeamShortName;
        public string AwayTeamShortName
        {
            get { return _awayTeamShortName; }
            set { _awayTeamShortName = value; NotifyPropertyChanged("AwayTeamShortName"); }
        }

        private string _homeTeamShortName;
        public string HomeTeamShortName
        {
            get { return _homeTeamShortName; }
            set { _homeTeamShortName = value; NotifyPropertyChanged("HomeTeamShortName"); }
        }

        private string _gameTitle;
        public string GameTitle
        {
            get { return _gameTitle; }
            set { _gameTitle = value; NotifyPropertyChanged("GameTitle"); }
        }

        private string _awayTeamJerseyPath;
        public string AwayTeamJerseyPath
        {
            get { return _awayTeamJerseyPath; }
            set { _awayTeamJerseyPath = value; NotifyPropertyChanged("AwayTeamJerseyPath"); }
        }

        private string _homeTeamJerseyPath;
        public string HomeTeamJerseyPath
        {
            get { return _homeTeamJerseyPath; }
            set { _homeTeamJerseyPath = value; NotifyPropertyChanged("HomeTeamJerseyPath"); }
        }

        private string _showHomeCircle = "Visible";
        public string ShowHomeCircle
        {
            get { return _showHomeCircle; }
            set { _showHomeCircle = value; NotifyPropertyChanged("ShowHomeCircle"); }
        }

        private string _showAwayCircle = "Visible";
        public string ShowAwayCircle 
        {
            get { return _showAwayCircle; }
            set { _showAwayCircle = value; NotifyPropertyChanged("ShowAwayCircle"); }
        }

        #endregion

    }
}
