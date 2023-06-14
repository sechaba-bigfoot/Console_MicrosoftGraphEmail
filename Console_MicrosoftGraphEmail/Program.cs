using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.Messages.Item.Move;
using Microsoft.Graph.Models;
using Console_MicrosoftGraphEmail.Models.ConfigurationModels;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Console_MicrosoftGraphEmail.Services;
using Console_MicrosoftGraphEmail.Helpers;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
    {


        IConfiguration _configuration = null;
        IServiceProvider _serviceProvider = null;
        IConnectWiseService _connectWiseService = null;

        ApplicationConfigurations logicConfigs = null;
        ConnectWiseConfigurations connectWiseConfigs = null;
        GraphMailConfigurations graphMailConfigs = null;

        ClientSecretCredential _credentials = null;
        GraphServiceClient _client = null;

        string fileLogPath = Directory.GetCurrentDirectory() + "/log.txt";
        string tenantId = "";
        string clientId = "";
        string clientSecrete = "";

        string emailToMonitor = "";
        string folderToMonitorName = "";
        string folderToMoveToName = "";
        string keyWord = "";

        try
        {

           _configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .Build();

            _serviceProvider = new ServiceCollection()
                .AddSingleton<IConnectWiseService, ConnectWiseService>()
                .AddOptions()
                .Configure<ApplicationConfigurations>(_configuration.GetSection(nameof(ApplicationConfigurations)))
                .Configure<ConnectWiseConfigurations>(_configuration.GetSection(nameof(ConnectWiseConfigurations)))
                .Configure<GraphMailConfigurations>(_configuration.GetSection(nameof(GraphMailConfigurations)))
                .BuildServiceProvider();

            logicConfigs = _serviceProvider.GetService<IOptions<ApplicationConfigurations>>().Value;
            connectWiseConfigs = _serviceProvider.GetService<IOptions<ConnectWiseConfigurations>>().Value;
            graphMailConfigs = _serviceProvider.GetService<IOptions<GraphMailConfigurations>>().Value;

            tenantId = graphMailConfigs.TenantId;
            clientId = graphMailConfigs.ClientId;
            clientSecrete = graphMailConfigs.ClientSecret;

            emailToMonitor = logicConfigs.EmailToMonitor;
            folderToMonitorName = logicConfigs.MailFolderToMonitor;
            folderToMoveToName = logicConfigs.MailFolderToMoveTo;
            keyWord = logicConfigs.KeyWord;

            _connectWiseService = _serviceProvider.GetService<IConnectWiseService>();

            TokenCredentialOptions options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            _credentials = new ClientSecretCredential(tenantId, clientId, clientSecrete, options);
            _client = new GraphServiceClient(_credentials);
        }
        catch (Exception ex)
        {
            string configMessage = $"Something went wrong trying co configure the application. Could not get application running.";
            string trace = ex.StackTrace;

            //Console.WriteLine(configMessage);
            //Console.WriteLine(ex);

            CustomLogger.WriteNewLog(fileLogPath, configMessage);
            CustomLogger.WriteNewLog(fileLogPath, trace);
        }

        CustomLogger.StartNewLog(fileLogPath);

        int failedExecutionCOunt = 0;
        int effectedEmailsCount = 0;

        MailFolder folderToMonitor = null;
        MailFolder folderToMoveTo = null;
        List<Message> messageList = new List<Message>();
        List<Ticket> ticketList = new List<Ticket>();

        while (true)
        {
            try
            {
                if(failedExecutionCOunt < 3)
                {
                    await Task.Run(StartOrganisingEmails);
                    Thread.Sleep(new TimeSpan(0, logicConfigs.IntervalRunsInMinutes, 0)); //TODO:Change before publish.
                }
            }
            catch (Exception ex)
            {
                failedExecutionCOunt++;
                string message = $"Application attempted to run, but failed. Attempts:[{failedExecutionCOunt}]";

                //Console.WriteLine(message);
                CustomLogger.WriteNewLog(fileLogPath, message);
                CustomLogger.WriteInLog(fileLogPath, $"{ex.Message}");
            }
        }

        #region Methods

        async void StartOrganisingEmails()
        {
            //1.Get all the information needed
            folderToMonitor = await GetMailFolderAsync(emailToMonitor, folderToMonitorName);
            folderToMoveTo = await GetMailFolderAsync(emailToMonitor, folderToMoveToName);
            messageList = await GetUserEmails(emailToMonitor);
            ticketList = await _connectWiseService.GetServiceTickets();

            //List<Ticket> ticketList = await GetTickets();

            //Are there any problems?
            if (!AnyForProblems())
            {
                string runningMessage = $"No problems, application is running.";

                //Console.WriteLine(runningMessage);
                CustomLogger.WriteNewLog(fileLogPath, runningMessage);

                //2.Loop trough the company tickets
                foreach (Ticket ticket in ticketList)
                {

                    foreach (Message message in messageList)
                    {
                        //Console.WriteLine($"Monitoring {messageList.Count} emails in => [{emailToMonitor}].");
                        List<string> listOfEmailsInMessage = ReturnEmailsInMessage(message);

                        //2.1 Does the given message subject contain have the ticket summary in it's contents?
                        //2.2 Does the given message already have the ticket# it's subject?
                        //2.3 Does the given message in its many correspondents have the email of the person who must be contacted for the ticket?

                        bool subjectMatch = message.Subject.Contains(ticket.summary);
                        bool subjectHasTicketNo = message.Subject.Contains(keyWord);
                        bool emailMatch = listOfEmailsInMessage.Contains(ticket.contactEmailAddress);
                        //bool emailMatch = listOfEmailsInMessage.Contains("sechabamot@gmail.com");//Only for testing

                        if (subjectMatch && emailMatch && !subjectHasTicketNo)
                        {
                            //3. Rename the email to a suited convetion.
                            //4. Update it in the Graph Mail API.
                            //5.Move email to specified folder.

                            message.Subject = $"Existing ticket#{ticket.id} email - [{ticket.summary}]";
                            await UpdateMessage(emailToMonitor, message);
                            await MoveMessageToFolder(emailToMonitor, message.Id, folderToMoveTo.Id);

                            effectedEmailsCount++;

                        }

                    }
                }

                string noProblemMessage = $"No problems, application ran successfully. Effected emails is => [{effectedEmailsCount}].";

                //Console.WriteLine(noProblemMessage);
                CustomLogger.WriteNewLog(fileLogPath, noProblemMessage);
                Console.ReadLine();
            }
            else
            {
                string problemMessage = $"Please address above problems-------";

                //Console.WriteLine(problemMessage);
                CustomLogger.WriteNewLog(fileLogPath, problemMessage);

            }

        }

        async Task<MailFolder> GetMailFolderAsync(string email, string mailFolder)
        {
            MailFolder folder = null;

            try
            {
                //Look through top level folders.
                var respone = await _client.Users[email]
                    .MailFolders.GetAsync((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Top = 100;
                        requestConfiguration.QueryParameters.IncludeHiddenFolders = "true";
                    });

                if (respone != null && respone.Value != null)
                {
                    folder = respone.Value.Find(mf => mf.DisplayName.Equals(mailFolder));
                    
                    //Look thruogh subfolders
                    if(folder == null)
                    {
                        foreach (var item in respone.Value)
                        {
                            var responeTwo = await _client.Users[email]
                                    .MailFolders[item.Id].ChildFolders.GetAsync();

                            if (responeTwo != null && responeTwo.Value != null)
                            {
                                MailFolder foundFolder = responeTwo.Value.Find(mf => mf.DisplayName.Equals(mailFolder)); ;
                                
                                if(foundFolder != null)
                                    folder = foundFolder;
                            }

                        }
                    }
                }


            }
            catch (Exception)
            {

            }

            return folder;
        }

        async Task<List<Message>> GetUserEmails(string userEmail)
        {

            List<Message> messages = new List<Message>() { };

            try
            {
                MessageCollectionResponse response = await _client.Users[userEmail].Messages.GetAsync();
                if (response != null && response.Value != null)
                {
                    messages = response.Value;
                }

            }
            catch (Exception ex)
            {
                //TODO: Add to problems
            }

            return messages;
        }

        async Task<Message> UpdateMessage(string userEmail, Message message)
        {
            return await _client.Users[userEmail].Messages[message.Id].PatchAsync(message);
        }

        async Task<Message> MoveMessageToFolder(string userEmail, string messageId, string folderId)
        {

            var requestBody = new MovePostRequestBody
            {
                DestinationId = folderId,
            };
            return await _client.Users[userEmail].Messages[messageId].Move.PostAsync(requestBody);
        }

        List<string> ReturnEmailsInMessage(Message email)
        {
            List<string> emailCorrespondents = new List<string>
            {
                email.Sender.EmailAddress.Address,
                email.From.EmailAddress.Address
            };

            foreach (Recipient cr in email.CcRecipients)
            {
                emailCorrespondents.Add(cr.EmailAddress.Address);
            };
            foreach (Recipient cr in email.BccRecipients)
            {
                emailCorrespondents.Add(cr.EmailAddress.Address);
            }

            return emailCorrespondents;
        }

        bool AnyForProblems()
        {
            if (folderToMonitor == null)
            {

                string folderMissing = $"(Problem) => Cannot find the folder {folderToMonitorName} in {emailToMonitor} email box.";

                CustomLogger.WriteNewLog(fileLogPath, folderMissing);
                CustomLogger.WriteInLog(fileLogPath, $"(Solution) => Either change the folder name in your configurations to a folder that exists in the monitored email. If the promblems persisits contact support.");
                return true;

            }

            if (folderToMoveTo == null) 
            {
                CustomLogger.WriteNewLog(fileLogPath, $"(Problem) => Cannot find the folder {folderToMoveToName} in {emailToMonitor} email box ");
                CustomLogger.WriteInLog(fileLogPath, $"(Solution) => Either change the folder name in your configurations to a folder that exists in the monitored email. If the promblems persisits contact support.");
                return true;
            }
            return false;
        }

        #endregion
    }
}