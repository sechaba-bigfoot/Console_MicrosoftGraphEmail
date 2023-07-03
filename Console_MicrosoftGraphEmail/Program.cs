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
using System.Text.RegularExpressions;
using Console_MicrosoftGraphEmail.Data;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {

        string fileLogPath = Directory.GetCurrentDirectory() + "/log.txt";
        string heartBeatLogPath = Directory.GetCurrentDirectory() + "/heartbeat.txt";
        string fileErrorLogPath = Directory.GetCurrentDirectory() + "/Errors.txt";

        CustomLoggerHelper.StartNewLog(fileLogPath);

        #region Global Properties

        IConfiguration _configuration = null;
        IServiceProvider _serviceProvider = null;
        IConnectWiseService _connectWiseService = null;

        ApplicationConfigurations logicConfigs = null;
        ConnectWiseConfigurations connectWiseConfigs = null;
        GraphMailConfigurations graphMailConfigs = null;

        ClientSecretCredential _credentials = null;
        GraphServiceClient _client = null;

        string connectionString = "";
        bool detailLogs = false;

        string tenantId = "";
        string clientId = "";
        string clientSecrete = "";

        string emailToMonitor = "";
        string folderToMonitorName = "";
        string folderToMoveToName = "";
        string keyWord = "";

        int failedExecutionCOunt = 0;
        int effectedEmailsCount = 0;

        MailFolder folderToMonitor = null;
        MailFolder folderToMoveTo = null;
        List<Message> messageList = new List<Message>();
        List<Ticket> ticketList = new List<Ticket>();

        #endregion

        ConfigureApplication();

        //Congfigure befor you start
        Thread thread = new Thread(RunApplicationHeartBeat);
        thread.Start();

        //Logical loop..
        while (true)
        {
            CustomLoggerHelper.WriteInLog(fileLogPath, "Loop running", false);
            try
            {
                if(failedExecutionCOunt < 3)
                {
                    await Task.Run(StartOrganisingEmails);
                    CustomLoggerHelper.WriteInLog(fileLogPath, $"Next interval in: {logicConfigs.IntervalRunsInSeconds} seconds.", false);

                    Thread.Sleep(new TimeSpan(0, 0, logicConfigs.IntervalRunsInSeconds));
                }
            }
            catch (Exception ex)
            {
                failedExecutionCOunt++;

                CustomLoggerHelper.WriteInLog(fileLogPath, 
                    $"Attempted to organise emailes, but failed. Attempts:[{failedExecutionCOunt}]", true);
                CustomLoggerHelper.WriteInLog(fileErrorLogPath, $"{ex.Message}", true);
            }

            CustomLoggerHelper.WriteInLog(fileLogPath, "Loop just ended.", false);
            CustomLoggerHelper.LogHeartBeat(heartBeatLogPath);
        }

        #region Methods

        async void StartOrganisingEmails()
        {
            CustomLoggerHelper.WriteInLog(fileLogPath, "Now organising emails", false);

            //1.Get all the information needed
            folderToMonitor = await GetMailFolderAsync(emailToMonitor, folderToMonitorName);
            folderToMoveTo = await GetMailFolderAsync(emailToMonitor, folderToMoveToName);
            messageList = await GetUserEmails(emailToMonitor);


            //Are there any problems?
            if (!AnyForProblems())
            {
                CustomLoggerHelper.WriteInLog(fileLogPath, "Mannaged to get all the information needed to organise emails", false);
                CustomLoggerHelper.WriteInLog(fileLogPath, $"Monitoring {messageList.Count} emails in => [{emailToMonitor}].", false);

                //1. Loop through messages
                foreach (Message message in messageList)
                {
                    string newSubject = ClearSubject(message.Subject);
                    List<string> emailCorrespondents = ReturnEmailsInMessage(message);

                    //2. Find a ticket that matches the email subject and has the same list of correspondents
                    var ticket = _connectWiseService.GetServiceTicket(connectWiseConfigs.ServiceBoard, newSubject, emailCorrespondents);
                    if(ticket != null)
                    {
                        //3. Rename the email to a suited convetion.
                        //4. Update it in the Graph Mail API.
                        //5. Move email to specified folder.
                        message.Subject = $"Existing ticket#{ticket.ServiceRecordId} email - [{ticket.Summary}]";
                        await UpdateMessage(emailToMonitor, message);
                        await MoveMessageToFolder(emailToMonitor, message.Id, folderToMoveTo.Id);

                        effectedEmailsCount++;

                        CustomLoggerHelper.WriteInLog(fileLogPath, $"Match found for email with subject => [{newSubject}]. Moving on...", false);


                    }
                    else
                    {
                        CustomLoggerHelper.WriteInLog(fileLogPath, $"Found no matching tickets for email with subject => [{newSubject}]. Moving on...", false);
                    }
                   
                }

                string noProblemMessage = $"No problems, application ran successfully. Effected emails is => [{effectedEmailsCount}].";
                CustomLoggerHelper.WriteInLog(fileLogPath, noProblemMessage, false);
            }
            else
            {

                string problemMessage = $"Please address above problems before running application again.";
                CustomLoggerHelper.WriteInLog(fileLogPath, problemMessage, false);

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
                MessageCollectionResponse response = await _client.Users[userEmail].Messages.GetAsync(options =>
                {
                    options.QueryParameters.Top = 1000;
                });
                if (response != null && response.Value != null)
                {
                    messages = response.Value;
                }

            }
            catch (Exception ex)
            {
                CustomLoggerHelper.WriteInLog(fileErrorLogPath, ex.Message, true);
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
            //email.
            List<string> emailCorrespondents = new List<string>
            {
                email.Sender?.EmailAddress.Address,
                email.From?.EmailAddress.Address
            };

            foreach (Recipient cr in email.ToRecipients)
            {
                emailCorrespondents.Add(cr.EmailAddress.Address);
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

        static string ClearSubject(string originalSubject)
        {
            Regex regex = new Regex(@"^([\[\(] *)?(RE?S?|FYI|RIF|I|FS|VB|RV|ENC|ODP|PD|YNT|ILT|SV|VS|VL|AW|WG|ΑΠ|ΣΧΕΤ|ΠΡΘ|תגובה|הועבר|主题|转发|FWD?) *([-:;)\]][ :;\])-]*|$)|\]+ *$", RegexOptions.IgnoreCase);
            originalSubject = regex.Replace(originalSubject, string.Empty);

            if (regex.IsMatch(originalSubject))
            {
                return ClearSubject(originalSubject);
            }

            return originalSubject;
        }

        bool AnyForProblems()
        {
            if (folderToMonitor == null)
            {

                string folderMissing = $"(Problem) => Cannot find the folder {folderToMonitorName} in {emailToMonitor} email box.";

                CustomLoggerHelper.WriteInLog(fileLogPath, folderMissing, true);
                CustomLoggerHelper.WriteInLog(fileLogPath, $"(Solution) => Either change the folder name in your configurations to a folder that exists in the monitored email. If the promblems persisits contact support.", false);
                return true;

            }

            if (folderToMoveTo == null) 
            {
                CustomLoggerHelper.WriteInLog(fileLogPath, $"(Problem) => Cannot find the folder {folderToMoveToName} in {emailToMonitor} email box.", true);
                CustomLoggerHelper.WriteInLog(fileLogPath, $"(Solution) => Either change the folder name in your configurations to a folder that exists in the monitored email. If the promblems persisits contact support.", false);
                return true;
            }

            return false;
        }

        string GetConnectionString()
        {
            return _configuration.GetValue<string>("ConnectionString");
        }

        void ConfigureApplication()
        {
            //1.
            CustomLoggerHelper.WriteInLog(fileLogPath, 
                "Setting up application configurations.", false);

            try
            {

                string jsonFilePath = Directory.GetCurrentDirectory() + "/appsettings.json";
                using (FileStream fileStream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
                {
                    _configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonStream(fileStream)
                   .Build();
                }

                //2.
                CustomLoggerHelper.WriteInLog(fileLogPath,
                    "Configurations built successfuly.", false);

                connectionString = GetConnectionString();


                if (!string.IsNullOrEmpty(connectionString))
                {
                    //3.
                    CustomLoggerHelper.WriteInLog(fileLogPath,
                        "Database connection string retrieved.", false);
                }

                _serviceProvider = new ServiceCollection()
               .AddSingleton<IConnectWiseService, ConnectWiseService>()
               .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))
               .AddOptions()
               .Configure<ApplicationConfigurations>(_configuration.GetSection(nameof(ApplicationConfigurations)))
               .Configure<ConnectWiseConfigurations>(_configuration.GetSection(nameof(ConnectWiseConfigurations)))
               .Configure<GraphMailConfigurations>(_configuration.GetSection(nameof(GraphMailConfigurations)))
               .BuildServiceProvider();

                //4.
                CustomLoggerHelper.WriteInLog(fileLogPath,
                    "Services configured successfully.", false);


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

                detailLogs = logicConfigs.ShowDetailedLog;

                //5.
                CustomLoggerHelper.WriteInLog(fileLogPath,
                    "Application configured successfully.", false);
            }
            catch (Exception ex)
            {
                string configMessage = "Something went wrong trying co configure the application. Ensure that you are not missing any information in your appsetting.json file";
                string trace = ex.StackTrace;

                CustomLoggerHelper.WriteInLog(fileLogPath, configMessage, true);
                CustomLoggerHelper.WriteInLog(fileErrorLogPath, trace, true);

                
                Console.WriteLine("Application will close in 10 seconds.");
                Thread.Sleep(new TimeSpan(0, 0, 10000));
                Environment.Exit(0); //Terminate application
            }
        }

        void RunApplicationHeartBeat()
        {
            while (true)
            {
                CustomLoggerHelper.LogHeartBeat(heartBeatLogPath);
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }
        #endregion
    }


}