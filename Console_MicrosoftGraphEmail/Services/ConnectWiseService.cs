using Console_MicrosoftGraphEmail.Models.ConfigurationModels;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Services
{
    public class ConnectWiseService :IConnectWiseService
    {
        private readonly ConnectWiseConfigurations _connectWiseConfig;
        private string company;
        private string publicKey;
        private string privateKey;
        private string clientId;
        private string server;
        private string serviveBoard;

        public ConnectWiseService(IOptions<ConnectWiseConfigurations> ConnectWiseOptions)
        {
            _connectWiseConfig = ConnectWiseOptions.Value;
            company = _connectWiseConfig.Company;
            publicKey = _connectWiseConfig.PublicKey;
            privateKey = _connectWiseConfig.PrivateKey;
            clientId = _connectWiseConfig.ClientId;
            server = _connectWiseConfig.Server;
            serviveBoard = _connectWiseConfig.ServiceBoard;
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

                HttpResponseMessage response = await client.GetAsync("https://cw.doneit.co.za/v4_6_release/apis/3.0/service/tickets?pageSize=1000");
                string stringContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        tickets = JsonConvert.DeserializeObject<List<Ticket>>(stringContent);
                    }
                    catch (Exception)
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

                HttpResponseMessage response = await client.GetAsync("https://cw.doneit.co.za/v4_6_release/apis/3.0/service/boards?pageSize=1000");
                string stringContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        boards = JsonConvert.DeserializeObject<List<Board>>(stringContent);
                    }
                    catch (Exception)
                    {

                    }
                }

            }
            return boards;

        }

    }
}
