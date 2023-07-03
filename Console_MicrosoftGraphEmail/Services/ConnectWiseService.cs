using Console_MicrosoftGraphEmail.Helpers;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Console_MicrosoftGraphEmail.Data;

namespace Console_MicrosoftGraphEmail.Services
{
    public class ConnectWiseService : IConnectWiseService
    {
        private readonly ApplicationDbContext _dbContext;

        public ConnectWiseService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Ticket GetServiceTicket
            (string serviveBoard, string Emailsubject, List<string> emailcorrespondents)
        {

            foreach (var ticket in _dbContext.Tickets.ToList())
            {
                //Add the person who must be contacted for the ticket.
                ticket.Correspondents.Add(ticket.Email_Address);

                List<string> ticketCorrespondents = 
                    StringHelpers.RemoveStringValuesThatEndWithRuleOut(ticket.Correspondents, "@doneit.co.za");
                emailcorrespondents =
                    StringHelpers.RemoveStringValuesThatEndWithRuleOut(ticket.Correspondents, "@doneit.co.za");


                bool isCoreectServiceBoard = ticket.Board_Name.Equals(serviveBoard);
                bool summaryMatchesSubject = ticket.Summary.Equals(Emailsubject);
                bool correspondentsMatch = StringHelpers.DoesStringListOneExistInStringListTwo(ticketCorrespondents, emailcorrespondents);

                if (isCoreectServiceBoard && summaryMatchesSubject && correspondentsMatch) 
                    return ticket;
      
            }

            return null;
        }


    }
}
