using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBotGuardian
{
    /// <summary>
    /// Main Net Core Application
    /// </summary>
    class Program
    {
        // Discord Client for actual connection
        private DiscordSocketClient _client;
        // Connection for SMTP
        private static SmtpClient client;
        private static MailAddress mailaccount;
        // Holds userdata for the Sheets DB
        private static List<UserData> users = new List<UserData>();

        // Used for enabling or disabling SMS THIS IS HARD CODED
        public bool SMSDisabled = true;

        /// <summary>
        /// Entry way to start the net core application
        /// </summary>
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Used for starting the connection to discord and loading settings
        /// </summary>
        public async Task MainAsync()
        {
            // Get the current directory
            string curDir = Directory.GetCurrentDirectory();
            // Check if the settings file exists
            if (File.Exists(curDir + "/botsettings.json") == false)
            {
                // If it dosen't generate a temporary file
                Credentials tempcreds = new Credentials
                {
                    SheetName = "PageNameOnDoc",
                    SMTPEmail = "ComingFrom@gmail.com",
                    SMTPEndpoint = "smtp.example.com",
                    SMTPUsername = "LoginUsername@gmail.com",
                    SpreadSheetID = "SPREADSHEET-ID-URL",
                    SMTPPassword = "EmailPassword",
                    BotToken = "DiscordBotToken"
                };
                File.WriteAllText(curDir + "/botsettings.json", JsonConvert.SerializeObject(tempcreds));
                // Kick back an exit warning to update creds
                Console.WriteLine("No BotSettings detected. One has been generated. Press any button to quit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            if (SMSDisabled == false)
            {
                // Check if google sheets creds file exists
                if (File.Exists(curDir + "/credentials.json") == false)
                {
                    Console.WriteLine("No Google Sheets Credentials file detected (Go download your creds from Google). Press any button to quit");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
                // Check if the email2sms.info json file exists
                if (File.Exists(curDir + "/sms.json") == false)
                {
                    Console.WriteLine("No SMS DB detected (You must download this from email2sms.info). Press any button to quit");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            // Assuming everything exists load in the json file
            string credsfile = File.ReadAllText(curDir + "/botsettings.json");
            Credentials creds = JsonConvert.DeserializeObject<Credentials>(credsfile);
            if (SMSDisabled == false)
            {
                // After parsing, set up the SMTP client for text message
                client = new SmtpClient(creds.SMTPEndpoint)
                {
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Port = 2525,
                    Credentials = new NetworkCredential(creds.SMTPUsername, creds.SMTPPassword)
                };
                mailaccount = new MailAddress(creds.SMTPEmail);
            }
            // Load the Database from google sheets
            users = Database.ReadDB(users);

            // Initalize the client
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;

            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, creds.BotToken);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Write log messages to the console
        /// </summary>
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// The Ready event indicates that the client has opened a connection and it is now safe to access the cache.
        /// </summary>
        private Task ReadyAsync()
        {
            Console.WriteLine(_client.CurrentUser + " is connected!");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Thread entry where the messages from Discord are passed under
        /// </summary>
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;
            await Task.Run(() => ReadmessagesAsync(message));
  
        }
        public async void ReadmessagesAsync(SocketMessage message)
        {          
            // Set up the Socket User Message so we can use it for context for later
            var usermessage = message as SocketUserMessage;
            // Double check the message is not empty
            if (usermessage == null) return;
            if (message.Content.Length > 0)
            {
                // Log Everything
                WriteLog(message.Channel + ": " + message.Author + " - " + message.Content);
                // Check if the message is a command based off if a bang is the first letter
                if (message.Content[0].ToString() == "!")
                {
                    // Set up Command Context
                    var context = new CommandContext(_client, usermessage);
                    // Check if the message sent matches an actual command in the approved or public commands 
                    if (await PublicCommands.ParsePublicCommandsAsync(message, users, context, SMSDisabled) == false && await ApprovedCommands.ParsePrivateCommandsAsync(message, users, client, context, mailaccount, SMSDisabled) == false)
                    {
                        if (SMSDisabled == false)
                        {
                            // If it dosen't match just send a text message of the info
                            Sendtext(message.Content, message.Author.Username.ToString().ToLower(), message.Channel.Name, message.Author.Id.ToString().ToLower());
                        }
                    }

                }
                // Check if there were any mentions
                else if (message.MentionedUsers.Count > 0)
                {
                    bool found = false;
                    // Check if the user mentioned the bot
                    foreach (var mention in message.MentionedUsers)
                    {
                        if (mention.Id == _client.CurrentUser.Id)
                        {
                            // If so send a gif
                            await message.Channel.SendMessageAsync(Angela.RandomAngela());
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        string rebuiltstring = message.Content;
                        foreach (var mention in message.MentionedUsers)
                        {
                            rebuiltstring = rebuiltstring.Replace("<@" + mention.Id + ">", "@" + (mention as SocketGuildUser).Nickname);
                        }
                        if (SMSDisabled == false)
                        {
                            Sendtext(rebuiltstring, message.Author.Username.ToString().ToLower(), message.Channel.Name, message.Author.Id.ToString().ToLower());
                        }
                    }
                }
                // If its not just send a text message
                else
                {
                    if (SMSDisabled == false)
                    {
                        Sendtext(message.Content, message.Author.Username.ToString().ToLower(), message.Channel.Name, message.Author.Id.ToString().ToLower());
                    }
                }
            }

        }
        /// <summary>
        /// Write a string to the log file
        /// </summary>
        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            // Get the current directory
            string curDir = Directory.GetCurrentDirectory();
            string logFilePath = curDir + "/";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }

        /// <summary>
        /// Used for sending a text message via email
        /// </summary>
        private static void Sendtext(string message, string author, string channel, string id)
        {
            // Double check the message is not empty (empty in the case of pictures)
            if (message.Trim() != "")
            {
                // Set up the mail message so we know who to send it to
                MailMessage mailMessage = new MailMessage
                {
                    From = mailaccount,
                };
                // Check the DB who is supposed to get the related message
                foreach (UserData person in users)
                {
                    // Make sure they are actually authenticated
                    if (person.DiscordUsername != null)
                    {
                        // Make sure the message is not coming from the author
                        if (person.DiscordUsername.ToLower() != id.ToLower())
                        {
                            // Check what channels and endpoint they have assigned
                            if (person.Channels != null && person.PhoneEndpoint != null)
                            {
                                // Finally make sure they 100% have SMS enabled
                                if (person.Channels.Contains(channel.ToString()) && person.SMS == true)
                                {
                                    // Add the user to the text list
                                    mailMessage.Bcc.Add(person.PhoneEndpoint);
                                }
                            }
                        }
                    }
                }
                // If the message is greator than 160 characters truncate it
                if (message.Length > 155)
                {
                    int channellength = channel.Length + 1;
                    int chanl = 155 - channellength;
                    int authorlength = author.Length + 1;
                    int messagelength = chanl - authorlength;
                    mailMessage.Body = channel.ToUpper() + ":" + author + "-" + message.ToString().Substring(0, messagelength);
                }
                // Else just set the body as the message
                else
                {
                    mailMessage.Body = channel.ToUpper() + ":" + author + "-" + message;
                }
                // Send the text
                string userState = "";
                try
                {
                    if (mailMessage.Bcc.Count > 0)
                    {
                        client.SendAsync(mailMessage, userState);
                    }
                }
                catch { }
            }
        }
    }

}
