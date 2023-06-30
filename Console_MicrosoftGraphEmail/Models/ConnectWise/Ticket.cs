using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Models.ConnectWise
{
    public class Ticket
    {
        private string summary;
        private int serviceRecordId;
        private DateTime dateEntered;
        private DateTime lastUpdate;
        private int isClosed;
        private string correspondentsString;
        private List<string> listOfCorrespondents;
        private string contactEmail;

        [Column("Board_Name")]
        [AllowNull]
        public string BoardName { get; set; }

        [Column("Summary")]
        [AllowNull]
        public string Summary { get => summary; set => summary = value; }

        [Column("SR_Service_RecID")]
        [AllowNull]
        public int ServiceRecordId { get => serviceRecordId; set => serviceRecordId = value; }

        [Column("Date_Entered_UTC")]
        [AllowNull]
        public DateTime DateEntered { get => dateEntered; set => dateEntered = value; }

        [AllowNull]
        [Column("Last_Update_UTC")]
        public DateTime LastUpdate { get => lastUpdate; set => lastUpdate = value; }

        [AllowNull]
        [Column("IsClosed_Flag")]
        public int IsClosed { get => isClosed; set => isClosed = value; }

        [Column("EmailCC")]
        [AllowNull]
        public string CorrespondentsString { get => correspondentsString; set => correspondentsString = value; }

        [NotMapped]
        public List<string> Correspondents { get => CorrespondentsString.Split(";").ToList(); private set => listOfCorrespondents = value; }

        [Column("Email_Address")]
        [AllowNull]
        public string ContactEmail { get => contactEmail; set => contactEmail = value; }

    }
}
