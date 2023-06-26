using Console_MicrosoftGraphEmail.Helpers;
using Console_MicrosoftGraphEmail.Models.ConfigurationModels;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Console_MicrosoftGraphEmail.Data;

namespace Console_MicrosoftGraphEmail.Services
{
    public class ConnectWiseService : IConnectWiseService
    {
        private readonly ConnectWiseConfigurations _connectWiseConfigs;
        private readonly ApplicationConfigurations _applicationConfigs;
        private readonly ApplicationDbContext _dbContext;

        private string company;
        private string publicKey;
        private string privateKey;
        private string clientId;
        private string server;
        private string serviveBoard;
        string fileLogPath = Directory.GetCurrentDirectory() + "/log.txt";
        string fileErrorLogPath = Directory.GetCurrentDirectory() + "/Errors.txt";


        public ConnectWiseService(IOptions<ConnectWiseConfigurations> ConnectWiseOptions,
            IOptions<ApplicationConfigurations> ApplicationOpions, ApplicationDbContext dbContext)
        {
            _connectWiseConfigs = ConnectWiseOptions.Value;
            _applicationConfigs = ApplicationOpions.Value;
            _dbContext = dbContext;

            company = _connectWiseConfigs.Company;
            publicKey = _connectWiseConfigs.PublicKey;
            privateKey = _connectWiseConfigs.PrivateKey;
            clientId = _connectWiseConfigs.ClientId;
            server = _connectWiseConfigs.Server;
            serviveBoard = _connectWiseConfigs.ServiceBoard;
        }


        public CustomTicket GetServiceTicket
            (string serviveBoard, string summary, List<string> correspondents)
        {

            foreach (var ticket in _dbContext.Tickets)
            {
                List<string> ticketCorrespondents = ticket.Correspondents;
                //ticketCorrespondents.Add(ticket.ContactEmail);

                bool a = ticket.BoardName.Equals(serviveBoard);
                bool b = ticket.Summary.Equals(summary);
                bool c = ticket.Correspondents.OrderDescending().SequenceEqual(correspondents.OrderDescending());

                if (a && b && c) return ticket;
      
            }

            return null;
        }

        public async Task<List<Ticket>> GetServiceTickets(string serviveBoard, string summary)
        {
            List<Ticket> tickets = new List<Ticket>();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{company}+{publicKey}:{privateKey}");
            string authorization = Convert.ToBase64String(plainTextBytes);
            string conditions = $"Board/Name=\"{serviveBoard}\" AND Summary=\"{summary}\" AND Status/Name!= \" >Closed\"";

            using (HttpClient client = new HttpClient())       
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri($"https://{server}/v4_6_release/apis/3.0");
                client.DefaultRequestHeaders.Add("clientId", clientId);
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authorization}");

                string query = $"https://cw.doneit.co.za/v4_6_release/apis/3.0/service/tickets?conditions={conditions}&pageSize=1000";

                HttpResponseMessage response = await client.GetAsync(query);
                string stringContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        tickets = JsonConvert.DeserializeObject<List<Ticket>>(stringContent);
                    }
                    catch (Exception ex)
                    {
                        if (_applicationConfigs.ShowDetailedLog)
                        {
                            string message = "Unexpected result while retrieveing tickets with following query string.";
                            CustomLogger.WriteNewLog(fileErrorLogPath, message);
                            CustomLogger.WriteNewLog(fileErrorLogPath, query);
                            CustomLogger.WriteNewLog(fileErrorLogPath, ex.StackTrace);

                        }
             
                    }
                }
                else
                {
                    if (_applicationConfigs.ShowDetailedLog)
                    {
                        ConnectWisteErrorResponse error = JsonConvert.DeserializeObject<ConnectWisteErrorResponse>(stringContent);
                        string message = "Unexpected result while retrieveing tickets with following query string.";
                        CustomLogger.WriteNewLog(fileErrorLogPath, message);
                        CustomLogger.WriteNewLog(fileErrorLogPath, query);
                        CustomLogger.WriteNewLog(fileErrorLogPath, $"API response => {error.Message}");
                        CustomLogger.WriteNewLog(fileErrorLogPath, "Moving on..");
                    }
                }

            }

            return tickets;
        }

        public async Task<List<Ticket>> GetServiceTickets()
        {
            List<Ticket> tickets = new List<Ticket>();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{company}+{publicKey}:{privateKey}");
            string authorization = Convert.ToBase64String(plainTextBytes);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri($"https://{server}/v4_6_release/apis/3.0");
                client.DefaultRequestHeaders.Add("clientId", clientId);
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authorization}");

                string query = "https://cw.doneit.co.za/v4_6_release/apis/3.0/service/tickets?pageSize=1000";
                HttpResponseMessage response = await client.GetAsync(query);
                string stringContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        tickets = JsonConvert.DeserializeObject<List<Ticket>>(stringContent);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }

            return tickets;
        }

        public async Task<List<Board>> GetServiceBoards()
        {
            List<Board> boards = new List<Board>();

            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{company}+{publicKey}:{privateKey}");
            string authorization = Convert.ToBase64String(plainTextBytes);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri($"https://{server}/v4_6_release/apis/3.0");
                client.DefaultRequestHeaders.Add("clientId", clientId);
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authorization}");

                string query = "https://cw.doneit.co.za/v4_6_release/apis/3.0/service/boards?pageSize=1000";
                HttpResponseMessage response = await client.GetAsync(query);
                string stringContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        boards = JsonConvert.DeserializeObject<List<Board>>(stringContent);
                    }
                    catch (Exception ex)
                    {
                        if (_applicationConfigs.ShowDetailedLog)
                        {
                            string message = "Unexpected result while retrieveing boards with following query string.";
                            CustomLogger.WriteNewLog(fileErrorLogPath, message);
                            CustomLogger.WriteNewLog(fileErrorLogPath, query);
                            CustomLogger.WriteNewLog(fileErrorLogPath, ex.StackTrace);
                        }
                    }
                }

            }
            return boards;

        }


    }
}
