using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Models.ConfigurationModels
{
    public class ConnectWiseConfigurations
    {
        public string ClientId { get; set; }
        public string Server { get; set; }
        public string Company { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
