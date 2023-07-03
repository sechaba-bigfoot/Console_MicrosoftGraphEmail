using System.ComponentModel.DataAnnotations.Schema;

namespace Console_MicrosoftGraphEmail.Models.ConnectWise
{
    public class Ticket
    {
            [Column("SR_Service_RecID")]
            public int SR_Service_RecID { get; set; }

            [Column("SR_Status_RecID")]
            public int SR_Status_RecID { get; set; }

            [Column("SR_Board_RecID")]
            public int SR_Board_RecID { get; set; }
            
            [Column("Board_Name")]
            public string Board_Name { get; set; }

            [Column("Contact_Name")]
            public string Contact_Name { get; set; }

            [Column("Email_Address")]
            public string Email_Address { get; set; }

            [Column("Summary")]
            public string Summary { get; set; }

            [Column("EmailCc_Flag")]
            public bool EmailCc_Flag { get; set; }
            
            [Column("EmailCC")]
            public string EmailCC { get; set; }

            [Column("Date_Entered_UTC")]
            public DateTime Date_Entered_UTC { get; set; }

            [Column("Last_Update_UTC")]
            public DateTime Last_Update_UTC { get; set; }

            [Column("Date_Closed_UTC")]
            public DateTime Date_Closed_UTC { get; set; }

            [Column("Parent_RecID")]
            public int Parent_RecID { get; set; }

            [Column("Closed_By")]
            public string Closed_By { get; set; }

            [Column("IsClosed_Flag")]
            public bool IsClosed { get; set; }

            [NotMapped]
            public List<string> Correspondents { get => EmailCC.Split(";").ToList(); }

    }

    //public class Tickets
    //{
    //    [Column("SR_Service_RecID")]
    //    public int SR_Service_RecID { get; set; }

    //    [Column("SR_Status_RecID")]
    //    public int SR_Status_RecID { get; set; }

    //    [Column("SR_Board_RecID")]
    //    public int SR_Board_RecID { get; set; }

    //    [Column("Board_Name")]
    //    public string Board_Name { get; set; }

    //    [Column("Contact_Name")]
    //    public string Contact_Name { get; set; }

    //    [Column("Email_Address")]
    //    public string Email_Address { get; set; }

    //    [Column("Summary")]
    //    public string Summary { get; set; }

    //    [Column("EmailCc_Flag")]
    //    public bool EmailCc_Flag { get; set; }

    //    [Column("EmailCC")]
    //    public string EmailCC { get; set; }

    //    [Column("Date_Entered_UTC")]
    //    public DateTime Date_Entered_UTC { get; set; }

    //    [Column("Last_Update_UTC")]
    //    public DateTime Last_Update_UTC { get; set; }

    //    [Column("Date_Closed_UTC")]
    //    public DateTime Date_Closed_UTC { get; set; }

    //    [Column("Parent_RecID")]
    //    public int Parent_RecID { get; set; }

    //    [Column("Closed_By")]
    //    public string Closed_By { get; set; }

    //    [Column("IsClosed_Flag")]
    //    public bool IsClosed { get; set; }

    //}
}
