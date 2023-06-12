using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.Messages.Item.Move;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Extensions.Configuration;
using Azure;
using Microsoft.Extensions.Configuration;
using Console_MicrosoftGraphEmail.Models.ConfigurationModels;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Sockets;


//Objective 
// - Look for recieved mail with certain subject in mail folder
// - Change subject of the the mail.
// - Move that mail to a new folder

internal class Program
{
    private static async Task Main(string[] args)
    {

        DateTime lastRun = DateTime.UtcNow;


        string? tenantId = "85f34518-abc7-4f24-9504-2622b0520aee"; 
        string? clientId = "71189445-d378-4435-b5c2-a8a8b892774a";
        string? clientSecrete = "IGw8Q~VUwkiFiFWcEIfuXPRUPcMZ0HQOP3eCab4n";

        string? connectWiseClientId = "c7d7e1a8-9961-4803-bd1a-9221369382a2";
        string? connectWiseServer = "cw.doneit.co.za";
        string? connectWiseCompany = "doneit";
        string? connectWisePublicKey = "vqf5I1qb9oNjBCBQ";
        string? connectWisePrivateKey = "ibV9fUvNkLP1epv9";

        string[] emailSubjectsTrims = new string[] { "FWD:", "RE:", "FW:" };//string 

        ClientSecretCredential _credentials;
        GraphServiceClient _client;

        var x = DateTime.UtcNow.Subtract(lastRun).TotalSeconds; //Chang to minutes
        while (x > 360)
        {
            await Run();
            lastRun = DateTime.UtcNow;
        }


        async Task Run()
        {

            

            //Run task every 30 minutes.

            TokenCredentialOptions options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            _credentials = new ClientSecretCredential(tenantId, clientId, clientSecrete, options);
            _client = new GraphServiceClient(_credentials);

            string email = "sechaba@bigfoot-it.com";
            string mailFolderName = "Inbox";
            string folderToMoveToName = "Tests";
            string keyWord = "Ticket#";
            string splitSeperator = " ";

            MailFolder? folder = await GetMailFolderAsync(email, mailFolderName);
            MailFolder? moveToFolder = await GetMailFolderAsync(email, folderToMoveToName);


            if (folder != null && moveToFolder != null)
            {
                List<Message> emails = await GetUserEmails(email);
                List<GetTicketDTOResponse> tickets = await GetTickets();

                if(emails.Any())
                {
                    Console.WriteLine($"We got all {emails.Count} emails for => [{email}].");
                    emails = await ReturnUserEmailsWithNoKeyword(emails, keyWord, folder.Id);
                    List<Message> emailsToEffect = new List<Message>();


                    if (emails.Any())
                    {
                        foreach (Message? msg in emails)
                        {
                           GetTicketDTOResponse? ticketFound;
                           int degreeMatch = 0;
                           string? subject = msg.Subject;
                           string? ticketNo = ExtractTicketNumberFromImput(subject, keyWord, splitSeperator);

                            if (string.IsNullOrEmpty(ticketNo))
                            {
                                //Reove words like RE: FWD: and more from the subject field
                                string? newSubject = RemoveKeywordsFromImput(subject, emailSubjectsTrims);
                                if (!string.IsNullOrEmpty(newSubject))
                                {
                                    //Find a ticket with the given subject
                                    ticketFound = tickets.FirstOrDefault(t => t.summary.Equals(subject));
                                    if (ticketFound != null)
                                    {
                                        msg.Subject = "Ticket# " + ticketFound.id;
                                        emailsToEffect.Add(msg);
                                    }
                                    else
                                    {

                                    }

                                }
                                
                            }
                            
                        }

                        Console.WriteLine($"Found {emailsToEffect.Count} emails with keyword '[{keyWord}]' in '[{email}]' '[{mailFolderName}]' mail folder.");
                        List<Message> updatedMessages = new List<Message>();

                        foreach (Message msg in emailsToEffect)
                        {


                            Message? updatedMessage = await UpdateMessageSubject(email, msg, "Updated message" + Guid.NewGuid().ToString());

                            if(updatedMessage != null)
                            {
                                updatedMessages.Add(updatedMessage);
                                Console.WriteLine($"Email subject updated from '[{msg.Subject}]' to '[{updatedMessage.Subject}]' in  [{email}] '[{mailFolderName}]' mail folder.");

                            }
                        }
                        Console.WriteLine($"Job complete => {updatedMessages.Count} renamed in {mailFolderName} folder of {email} mail box");

                        List<Message> movedMessages = new List<Message>();

                        foreach (Message msg in updatedMessages)
                        {

                            Message? message = await MoveMessageToFolder(email, msg.Id, moveToFolder.Id);
                            if(message != null)
                            {
                                Console.WriteLine($"Message {msg.Subject} moved from '[{mailFolderName}]' => '[{folderToMoveToName}]' in '[{email}]' mail box.");
                                movedMessages.Add(message);
                            }

                        }

                        Console.WriteLine($"Job complete => {movedMessages.Count} moved to {folderToMoveToName} in {email} mail box");

                    }
                    else
                    {
                        Console.WriteLine($"There are no emails with keyword '[{keyWord}]' in '[{email}]' '[{mailFolderName}]' mail folder.");
                    }
                }
                else
                {
                    Console.WriteLine($"There are no emails for => [{email}].");
                }

            }
            else
            {
                if(folder == null)
                    Console.WriteLine($"Could not find email folder => [{mailFolderName}].");
                if(moveToFolder == null)
                    Console.WriteLine($"Could not find email folder => [{folderToMoveToName}].");

            }

            Console.ReadLine();
        }

        async Task<MailFolder?> GetMailFolderAsync(string email, string mailFolder)
        {

            try
            {
                var respone = await _client.Users[email]
                    .MailFolders.GetAsync();
                if (respone != null && respone.Value != null)
                {
                    return respone.Value.Find(mf => mf.DisplayName.Equals(mailFolder));
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        async Task<List<Message>> GetUserEmails(string userEmail)
        {
            // - Wrong email (That does not exist) will throw a {Microsoft.Graph.Models.ODataErrors.MainError} exception. 
            // -

            List<Message> messages = new List<Message>() { };

            try
            {
                MessageCollectionResponse? response = await _client.Users[userEmail].Messages.GetAsync();
                if (response != null && response.Value != null)
                {
                    messages = response.Value;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not get emails for => [{userEmail}].");
                Console.WriteLine($"--------------------------------------------------");
                Console.WriteLine(ex.Message);
            }

            return messages;
        }

        async Task<List<Message>> ReturnUserEmailsWithNoKeyword(List<Message> messages, string condition, string folderId)
        {
            List<Message> messagesToReturn = new List<Message>() { };

            try
            {
                messagesToReturn = messages.FindAll(msg 
                    => !msg.Subject.Contains(condition) 
                    && msg.ParentFolderId.Equals(folderId));
            }
            catch (Exception)
            {
                throw;
            }

            return messagesToReturn;
        }

        async Task<Message?> UpdateMessageSubject(string userEmail, Message message, string subject)
        {
            message.Subject = subject;
            return await _client.Users[userEmail].Messages[message.Id].PatchAsync(message);

        }

        async Task<Message?> MoveMessageToFolder(string userEmail, string messageId, string folderName = "Tests Folder")
        {
                
                var requestBody = new MovePostRequestBody
                {
                    DestinationId = folderName,
                };
                return await _client.Users[userEmail].Messages[messageId].Move.PostAsync(requestBody);
        }

        string? ExtractTicketNumberFromImput(string input, string keyword, string splitSeporator)
        {
            if (!string.IsNullOrEmpty(input) && input.Contains(keyword))
            {
                string[] words = input.Split(splitSeporator);
                if (words.Length > 0)
                {
                    string? ticketNo = words.FirstOrDefault(w
                        => w.StartsWith("#"));
                    if (!string.IsNullOrEmpty(ticketNo))
                    {
                        return ticketNo.Remove(0, 1);
                    }
                }
            }
           

            return null;
        }

        string? RemoveKeywordsFromImput(string input, string[] keywords)
        {
            foreach (string word in keywords) {

                if (input.Contains(word))
                {
                    int start = input.IndexOf(word[0]);
                    int end = input.IndexOf(word[word.ToArray().Length - 1]);

                    input = input.Remove(start, end);
                }
            }
            return input;
        }

        int DegreeOfEmailToTicketMatch(Message? email, List<GetTicketDTOResponse> tickets)
        {
            List<string> emailCorrespondents = new List<string>();
            
            emailCorrespondents.Add(email.Sender.EmailAddress.Address);
            emailCorrespondents.Add(email.From.EmailAddress.Address);
            
            foreach (Recipient cr in email.CcRecipients)
            {
                emailCorrespondents.Add(cr.EmailAddress.Address);
            };
            foreach (Recipient cr in email.BccRecipients)
            {
                emailCorrespondents.Add(cr.EmailAddress.Address);
            }

            foreach (GetTicketDTOResponse ticket in tickets) 
            {
                
            }
            return 0;
        }

        async Task<List<GetTicketDTOResponse>> GetTickets()
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{connectWiseCompany}+{connectWisePublicKey}:{connectWisePrivateKey}");
            string authorization = Convert.ToBase64String(plainTextBytes);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                //client.BaseAddress = new Uri($"https://{connectWiseServer}/v4_6_release/apis/3.0");
                client.DefaultRequestHeaders.Add("clientId", connectWiseClientId);
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authorization}");

                //HttpResponseMessage response = await client.GetAsync($"service/ticket{ticketId}");
                HttpResponseMessage response = await client.GetAsync("https://cw.doneit.co.za/v4_6_release/apis/3.0/service/tickets");
                string stringContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    try
                    {
                        List<GetTicketDTOResponse>? ticket = JsonConvert.DeserializeObject<List<GetTicketDTOResponse>>(stringContent);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

            }

            return null;
        }

    }
}