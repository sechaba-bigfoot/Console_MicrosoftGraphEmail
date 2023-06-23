using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Models.ConnectWise
{
    public class CustomTicketView
    {
        [Column("Summary")]
        public string Summary { get; set; }
       
        [Column("SR_Service_RecID")]
        public int ServiceRecordId { get; set; }

        //[Column("EmailCC")]
        //public string Correspondents { get; set; }
    }
}
