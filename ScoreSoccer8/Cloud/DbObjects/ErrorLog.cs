using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Cloud.DbObjects
{
    public class ErrorLog
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Method { get; set; }
        public string ErrorMessage { get; set; }
    }
}
