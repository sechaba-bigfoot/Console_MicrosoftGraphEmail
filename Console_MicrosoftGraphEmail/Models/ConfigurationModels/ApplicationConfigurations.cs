using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Models.ConfigurationModels
{
    public class ApplicationConfigurations
    {
        public string EmailToMonitor { get; set; }
        public string KeyWord { get; set; }
        public string MailFolderToMonitor { get; set; }
        public int IntervalRunsInMinutes { get; set; }
        public string MailFolderToMoveTo { get; set; }
        public bool ShowDetailedLog { get ; set; }
        public string DbConnectionString { get; set; }
    }
}
