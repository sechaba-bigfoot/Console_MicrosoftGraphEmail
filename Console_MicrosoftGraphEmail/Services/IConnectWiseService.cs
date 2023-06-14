using Console_MicrosoftGraphEmail.Models.ConnectWise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Services
{
    public interface IConnectWiseService
    {
        Task<List<Ticket>> GetServiceTickets();
    }
}
