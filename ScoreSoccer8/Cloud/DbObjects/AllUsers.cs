using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Cloud.DbObjects
{
    public class AllUsers
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public DateTime LastLogin { get; set; }
        public TimeSpan TotalTimeInApp { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Paid { get; set; }
        public bool TrialVersion { get; set; }
        public bool FreeVersion { get; set; }
        public int TimesOpened { get; set; }
    }
}
