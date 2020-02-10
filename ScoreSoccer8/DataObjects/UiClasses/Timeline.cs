using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class Timeline : Notification
    {

        public Timeline()
        {
            
            _rowSpam = 1;
            _imageHeight = 20;
            _imageWidth = 15;
            _fontSize = 25;
            _awaySecondYellow = "Collapsed";
            _homeSecondYellow = "Collapsed";
            _showGameMarks = "Collapsed";
        }

        private string _showGameMarks;
        public string ShowGameMarks
        {
            get { return _showGameMarks; }
            set { _showGameMarks = value; NotifyPropertyChanged("ShowGameMarks"); }
        }

        private string _gameMarks;
        public string GameMarks
        {
            get { return _gameMarks; }
            set { _gameMarks = value; NotifyPropertyChanged("GameMarks"); }
        }

        private string _awaySecondYellow;
        public string AwaySecondYellow
        {
            get { return _awaySecondYellow; }
            set { _awaySecondYellow = value; NotifyPropertyChanged("AwaySecondYellow"); }
        }

        private string _homeSecondYellow;
        public string HomeSecondYellow
        {
            get { return _homeSecondYellow; }
            set { _homeSecondYellow = value; NotifyPropertyChanged("HomeSecondYellow"); }
        }

        private string _time;
        public string Time
        {
            get { return _time; }
            set { _time = value; NotifyPropertyChanged("Time"); }
        }


        private int _period;
        public int Period
        {
            get { return _period; }
            set { _period = value; NotifyPropertyChanged("Period"); }
        }


        private int _rowSpam;
        public int RowSpam
        {
            get { return _rowSpam; }
            set { _rowSpam = value; NotifyPropertyChanged("RowSpam"); }
        }

        private string _awayPlayerName;
        public string AwayPlayerName
        {
            get { return _awayPlayerName; }
            set { _awayPlayerName = value; NotifyPropertyChanged("AwayPlayerName"); }
        }

        private string _homePlayerName;
        public string HomePlayerName
        {
            get { return _homePlayerName; }
            set { _homePlayerName = value; NotifyPropertyChanged("HomePlayerName"); }
        }


        private string _awayImagePath;
        public string AwayImagePath
        {
            get { return _awayImagePath; }
            set { _awayImagePath = value; NotifyPropertyChanged("AwayImagePath"); }
        }

        private string _homeImagePath;
        public string HomeImagePath
        {
            get { return _homeImagePath; }
            set { _homeImagePath = value; NotifyPropertyChanged("HomeImagePath"); }
        }


        private string _awayPlayerSecondName;
        public string AwayPlayerSecondName
        {
            get { return _awayPlayerSecondName; }
            set { _awayPlayerSecondName = value; NotifyPropertyChanged("AwayPlayerSecondName"); }
        }

        private string _homePlayerSecondName;
        public string HomePlayerSecondName
        {
            get { return _homePlayerSecondName; }
            set { _homePlayerSecondName = value; NotifyPropertyChanged("HomePlayerSecondName"); }
        }


        private string _awayPlayerScore;
        public string AwayPlayerScore
        {
            get { return _awayPlayerScore; }
            set { _awayPlayerScore = value; NotifyPropertyChanged("AwayPlayerScore"); }
        }

        private string _homePlayerScore;
        public string HomePlayerScore
        {
            get { return _homePlayerScore; }
            set { _homePlayerScore = value; NotifyPropertyChanged("HomePlayerScore"); }
        }

        private int _imageWidth;
        public int ImageWidth
        {
            get { return _imageWidth; }
            set { _imageWidth = value; NotifyPropertyChanged("ImageWidth"); }
        }

        private int _imageHeight;
        public int ImageHeight
        {
            get { return _imageHeight; }
            set { _imageHeight = value; NotifyPropertyChanged("ImageHeight"); }
        }

        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; NotifyPropertyChanged("FontSize"); }
        }

    }
}
