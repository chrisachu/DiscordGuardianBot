using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DiscordBotGuardian
{
    /// <summary>
    /// Main Net Core Application
    /// </summary>
    class Program
    {
        private DiscordSocketClient _client;
        private static string curDir = Directory.GetCurrentDirectory();
        private static string json = File.ReadAllText(curDir + "/sms.json");
        private List<SMS> providers = JsonConvert.DeserializeObject<List<SMS>>(json);
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        private static SheetsService service;
        private static string spreadsheetId;
        private static List<UserData> users = new List<UserData>();
        private static string range;
        private static SmtpClient client;
        private static MailAddress mailaccount;
        private static string sheetname;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            if (File.Exists(curDir + "/botsettings.json") == false)
            {
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
                Console.WriteLine("No BotSettings detected. One has been generated. Press any button to quit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            if (File.Exists(curDir + "/credentials.json") == false)
            {
                Console.WriteLine("No Google Sheets Credentials file detected (Go download your creds from Google). Press any button to quit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            if (File.Exists(curDir + "/sms.json") == false)
            {
                Console.WriteLine("No SMS DB detected (You must download this from email2sms.info). Press any button to quit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            string credsfile = File.ReadAllText(curDir + "/botsettings.json");
            Credentials creds = JsonConvert.DeserializeObject<Credentials>(credsfile);
            client = new SmtpClient(creds.SMTPEndpoint)
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Port = 587,
                Credentials = new NetworkCredential(creds.SMTPUsername, creds.SMTPPassword)
            };
            mailaccount = new MailAddress(creds.SMTPEmail);
            range = creds.SheetName + "!A:I";
            sheetname = creds.SheetName;
            spreadsheetId = creds.SpreadSheetID;
            Logintosheets();
            ReadDB();

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
        private static void Logintosheets()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine(_client.CurrentUser + " is connected!");

            return Task.CompletedTask;
        }


        private static void ReadDB()
        {
            lock (users)
            {
                List<UserData> newuser = new List<UserData>();
                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    int newuserrow = 0;
                    int userrow = 0;
                    bool headers = false;
                    foreach (var row in values)
                    {
                        if (headers == true)
                        {
                            if (users.ElementAtOrDefault(userrow) != null)
                            {
                                if (row[0].ToString() != "NULL")
                                {
                                    users[userrow].RTUsername = row[0].ToString();
                                }
                                if (row[1].ToString() != "NULL")
                                {
                                    users[userrow].Team = row[1].ToString();
                                }
                                if (row[2].ToString() != "NULL")
                                {
                                    users[userrow].PhoneEndpoint = row[2].ToString();
                                }
                                if (row[3].ToString() != "NULL")
                                {
                                    users[userrow].Authenticated = Convert.ToBoolean(row[3].ToString());
                                }
                                if (row[4].ToString() != "NULL")
                                {
                                    users[userrow].DiscordUsername = row[4].ToString();
                                }
                                if (row[5].ToString() != "NULL")
                                {
                                    users[userrow].Roles = row[5].ToString().Split(',').ToList();
                                }
                                if (row[6].ToString() != "NULL")
                                {
                                    users[userrow].SMS = Convert.ToBoolean(row[6].ToString());
                                }
                                if (row[7].ToString() != "NULL")
                                {
                                    users[userrow].Channels = row[7].ToString().Split(',').ToList();
                                }
                                if (row[8].ToString() != "NULL")
                                {
                                    users[userrow].Event = row[8].ToString().Split(',').ToList();
                                }
                            }
                            else
                            {
                                if (newuser.ElementAtOrDefault(newuserrow) == null)
                                {
                                    UserData tempnewuser = new UserData();
                                    newuser.Add(tempnewuser);
                                }
                                if (row[0].ToString() != "NULL")
                                {
                                    newuser[newuserrow].RTUsername = row[0].ToString();
                                }
                                if (row[1].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Team = row[1].ToString();
                                }
                                if (row[2].ToString() != "NULL")
                                {
                                    newuser[newuserrow].PhoneEndpoint = row[2].ToString();
                                }
                                if (row[3].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Authenticated = Convert.ToBoolean(row[3].ToString());
                                }
                                if (row[4].ToString() != "NULL")
                                {
                                    newuser[newuserrow].DiscordUsername = row[4].ToString();
                                }
                                if (row[5].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Roles = row[5].ToString().Split(',').ToList();
                                }
                                if (row[6].ToString() != "NULL")
                                {
                                    newuser[newuserrow].SMS = Convert.ToBoolean(row[6].ToString());
                                }
                                if (row[7].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Channels = row[7].ToString().Split(',').ToList();
                                }
                                if (row[8].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Event = row[8].ToString().Split(',').ToList();
                                }
                                newuserrow++;
                            }
                            userrow++;
                        }
                        else
                        {
                            headers = true;
                        }
                    }
                }
                if (newuser.Count > 0)
                {
                    foreach (UserData user in newuser)
                    {
                        users.Add(user);
                    }
                }
            }
        }

        private static void UpdateUser(string user, string type, string value, [Optional] List<string> list)
        {
            bool updated = false;
            UserData userinfo = new UserData();
            foreach (UserData person in users)
            {
                try
                {
                    if (type == "DiscordUsername")
                    {
                        if (person.RTUsername == user)
                        {
                            person.DiscordUsername = value;
                            break;
                        }
                    }
                    else
                    {
                        if (person.DiscordUsername != null)
                        {
                            if (person.DiscordUsername.ToLower() == user)
                            {
                                if (type == "PhoneEndpoint")
                                {
                                    person.PhoneEndpoint = value;
                                }
                                else if (type == "Authenticated")
                                {
                                    person.Authenticated = Convert.ToBoolean(value);
                                }
                                else if (type == "Roles")
                                {
                                    person.Roles = list;
                                }
                                else if (type == "SMS")
                                {
                                    person.SMS = Convert.ToBoolean(value);
                                }
                                else if (type == "Channels")
                                {
                                    person.Channels = list;
                                }
                                updated = true;
                                userinfo = person;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            if (updated == true)
            {
                WriteDB(userinfo);
            }
        }

        private static void WriteDB(UserData user)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            int rownumber = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == user.RTUsername)
                    {
                        break;
                    }
                    else
                    {
                        rownumber++;
                    }
                }
            }

            String range2 = sheetname + "!A" + rownumber.ToString();

            ValueRange requestBody = new ValueRange();

            SpreadsheetsResource.ValuesResource.UpdateRequest request2 = service.Spreadsheets.Values.Update(requestBody, spreadsheetId, range2);
            request2.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            string team;
            if (user.Team == null)
            {
                team = "NULL";
            }
            else
            {
                team = user.Team;
            }
            string phoneendpoint;
            if (user.PhoneEndpoint == null)
            {
                phoneendpoint = "NULL";
            }
            else
            {
                phoneendpoint = user.PhoneEndpoint;
            }
            string roles;
            if (user.Roles == null)
            {
                roles = "NULL";
            }
            else
            {
                roles = string.Join(",", user.Roles);
            }
            string channels;
            if (user.Channels == null || user.Channels.Count == 0)
            {
                channels = "NULL";
            }
            else
            {
                channels = string.Join(",", user.Channels);
            }
            string events;
            if (user.Event == null || user.Event.Count == 0)
            {
                events = "NULL";
            }
            else
            {
                events = string.Join(",", user.Event);
            }
            var oblist = new List<object>() { user.RTUsername, team, phoneendpoint, user.Authenticated.ToString(), user.DiscordUsername, roles, user.SMS.ToString(), channels, events };
            requestBody.Values = new List<IList<object>> { oblist };
            UpdateValuesResponse response2 = request2.Execute();

        }
        private static bool IsUserAuthenticated(string user)
        {
            foreach (UserData person in users)
            {
                if (person.DiscordUsername != null)
                {
                    if (user.ToLower() == person.DiscordUsername.ToLower() && person.Authenticated == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task RoleTask(CommandContext Context, string role)
        {
            var roles = Context.Guild.Roles;
            ulong roleId = 000000;
            foreach (var singlerole in roles)
            {
                if (singlerole.Name == role)
                {
                    roleId = singlerole.Id;
                    break;
                }
            }
            IRole roleid = Context.Guild.GetRole(roleId);
            await ((SocketGuildUser)Context.User).AddRoleAsync(roleid);
        }


        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            var usermessage = message as SocketUserMessage;
            if (usermessage == null) return;

            if (message.Content.Length > 0)
            {
                if (message.Content[0].ToString() == "!")
                {
                    var context = new CommandContext(_client, usermessage);
                    List<string> splitmessage = message.Content.Split().ToList();
                    if (splitmessage[0].ToLower() == "!registerphone" && IsUserAuthenticated(message.Author.Id.ToString()))
                    {
                        var parsedmessage = message.Content.Split('"')
                                             .Select((element, index) => index % 2 == 0  // If even index
                                                                   ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                                                   : new string[] { element })  // Keep the entire item
                                             .SelectMany(element => element).ToList();
                        if (parsedmessage.Count == 4 && Validation.Isphonenumber(parsedmessage[1]) && Validation.Isvalidregion(parsedmessage[2], providers) && Validation.Isvalidcarrier(parsedmessage[3], providers))
                        {
                            string country = "";
                            //Need to add logic for adding user to DB of where they should be getting messages
                            if (parsedmessage[2].Length <= 3)
                            {
                                RegionInfo region = new RegionInfo(parsedmessage[2]);
                                foreach (SMS carrier in providers)
                                {
                                    if (region.EnglishName == carrier.Country)
                                    {
                                        country = region.EnglishName;
                                    }
                                }
                            }
                            else
                            {
                                country = parsedmessage[2];
                            }
                            foreach (SMS carrier in providers)
                            {
                                if (country.ToLower() == carrier.Country.ToLower())
                                {
                                    if (parsedmessage[3].ToLower() == carrier.Carrier.ToLower())
                                    {
                                        string output = carrier.EmailToSms.Substring(carrier.EmailToSms.IndexOf('@'));
                                        UpdateUser(message.Author.Id.ToString().ToLower(), "PhoneEndpoint", parsedmessage[1] + output);
                                        MailMessage mailMessage = new MailMessage
                                        {
                                            From = new MailAddress("email"),
                                        };
                                        mailMessage.Bcc.Add(parsedmessage[1] + output);
                                        mailMessage.Body = "This is a test notification for the Guardian Discord.";
                                        string userState = "";
                                        client.SendAsync(mailMessage, userState);
                                        await message.Channel.SendMessageAsync("Registered: " + message.Author.Mention + Environment.NewLine + "Number: " + parsedmessage[1] + output + Environment.NewLine + "A test message has been sent, if you do not reieve it please try registering again" + Environment.NewLine + "If you want to get messages for a specific channel type !SMS in the channel");
                                        break;
                                    }
                                }

                            }
                        }
                        else if (parsedmessage.Count != 4)
                        {
                            await message.Channel.SendMessageAsync("Non Complete Command.");
                        }
                        else if (Validation.Isphonenumber(parsedmessage[1]) == false)
                        {
                            await message.Channel.SendMessageAsync("Invalid Phone Number. Please enter only numbers.");
                        }
                        else if (Validation.Isvalidregion(parsedmessage[2], providers) == false)
                        {
                            await message.Channel.SendMessageAsync("Invalid Country/Country Code");
                        }
                        else if (Validation.Isvalidcarrier(parsedmessage[3], providers) == false && Validation.Isvalidregion(parsedmessage[2], providers) == true)
                        {
                            string country = "";
                            if (parsedmessage[2].Length <= 3)
                            {
                                RegionInfo region = new RegionInfo(parsedmessage[2]);
                                foreach (SMS carrier in providers)
                                {
                                    if (region.EnglishName == carrier.Country)
                                    {
                                        country = region.EnglishName;
                                    }
                                }
                            }
                            else
                            {
                                country = parsedmessage[2];
                            }
                            string responselist = "";
                            foreach (SMS carrier in providers)
                            {
                                if (country.ToLower() == carrier.Country.ToLower())
                                {
                                    responselist = responselist + Environment.NewLine + carrier.Carrier;// + " " + carrier.Notes;
                                }
                            }
                            await message.Channel.SendMessageAsync("Invalid Provider, the valid providers for your country: " + country + ", are ```" + responselist + "```");
                        }
                        return;
                    }
                    else if (splitmessage[0].ToLower() == "!notify" && IsUserAuthenticated(message.Author.Id.ToString()))
                    {
                        foreach (UserData person in users)
                        {
                            if (person.DiscordUsername != null)
                            {
                                if (person.DiscordUsername.ToLower() == message.Author.Id.ToString().ToLower())
                                {
                                    if (person.Channels != null)
                                    {
                                        if (person.Channels.Contains(message.Channel.Name))
                                        {
                                            person.Channels.Remove(message.Channel.Name);
                                            UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", person.Channels);
                                            await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages for this channel");
                                        }
                                        else
                                        {
                                            person.Channels.Add(message.Channel.Name);
                                            UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", person.Channels);
                                            await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                        }
                                    }
                                    else
                                    {
                                        person.Channels = new List<string>
                                    {
                                        message.Channel.Name
                                    };
                                        UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", person.Channels);
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                    }
                                }
                            }
                        }
                    }
                    else if (splitmessage[0].ToLower() == "!sms" && IsUserAuthenticated(message.Author.Id.ToString()))
                    {
                        foreach (UserData person in users)
                        {
                            if (person.DiscordUsername != null)
                            {
                                if (person.DiscordUsername.ToLower() == message.Author.Id.ToString().ToLower())
                                {
                                    if (person.SMS == false)
                                    {
                                        UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "true");
                                        if (person.Channels != null)
                                        {
                                            if (person.Channels.Contains("announcements") == false)
                                            {
                                                person.Channels.Add(message.Channel.Name);
                                                UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", person.Channels);
                                            }

                                        }
                                        else
                                        {
                                            person.Channels = new List<string>
                                        {
                                            "announcements"
                                        };
                                            UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", person.Channels);
                                        }
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages, please rememeber to set up your phone provider if you have not");
                                    }
                                    else
                                    {
                                        UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "false");
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages");
                                    }


                                }
                            }
                        }
                    }
                    else if (splitmessage[0].ToLower() == "!rt" && message.Channel.Name == "roll-call")
                    {
                        if (splitmessage.Count == 2)
                        {
                            ReadDB();
                            bool found = false;
                            foreach (UserData user in users)
                            {
                                if(user.DiscordUsername == message.Author.Id.ToString() && user.RTUsername != message.Content.Split()[1].ToLower())
                                {
                                    found = false;
                                }
                                else if (user.RTUsername == message.Content.Split()[1].ToLower() && user.Authenticated == false)
                                {
                                    //add checking if you are already auth'd
                                    UpdateUser(message.Content.Split()[1].ToLower(), "DiscordUsername", message.Author.Id.ToString().ToLower());
                                    UpdateUser(message.Author.Id.ToString().ToLower(), "Authenticated", "TRUE");
                                    List<string> roles = new List<string>();
                                    if (user.Roles != null)
                                    {
                                        if (user.Roles.Count > 0)
                                        {
                                            foreach (string role in user.Roles)
                                            {
                                                roles.Add(role);
                                            }
                                        }
                                    }
                                    if (roles.Contains("Guardian-US-2018") == false)
                                    {
                                        roles.Add("Guardian-US-2018");
                                    }
                                    UpdateUser(message.Author.Id.ToString().ToLower(), "Roles", "", roles);
                                    await RoleTask(context, "Guardian-US-2018");
                                    found = true;
                                    break;
                                }
                            }
                            if (found == true)
                            {
                                await message.Channel.SendMessageAsync(message.Author.Mention + " Your Discord user has now been authenticated as a Guardian");
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync(message.Author.Mention + " Your RT Username is already authenticated as another user or you are not registered as a Guardian");
                            }
                        }
                        return;
                    }
                    else if (splitmessage[0].ToLower() == "!help")
                    {
                        if (message.Channel.Name == "roll-call")
                        {
                            await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!rt USERNAME - to log in to be authenticated");
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!sms - To enable or disable SMS for your device (You will have to register your phone if you have not yet)" + Environment.NewLine + "!notify - Type it in the channel you want to receive SMS notifications for" + Environment.NewLine + "!registerphone 1234567890 \"United States\" \"Verizon Wireless\" - Use this command to register your phone as a device to get texts on. (Number, Country, Carrier)");
                        }
                    }
                    else if (splitmessage[0].ToLower() == "!testevent")
                    {
                        NewYear test = new NewYear();
                        new Thread(async () =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            /* run your code here */
                            await test.CreateNewYearAsync(context, "Austin", 19);
                        }).Start();
                    }
                    else
                    {
                        Sendtext(message.Content, message.Author.Id.ToString().ToLower(), message.Channel.Name);
                    }

                }
                else
                {
                    Sendtext(message.Content, message.Author.Id.ToString().ToLower(), message.Channel.Name);
                }
            }
        }
        private static void Sendtext(string message, string author, string channel)
        {
            if (message.Trim() != "")
            {
                foreach (UserData person in users)
                {
                    if (person.DiscordUsername != null)
                    {
                        if (person.DiscordUsername.ToLower() != author.ToLower())
                        {
                            if (person.Channels != null && person.PhoneEndpoint != null)
                            {
                                if (person.Channels.Contains(channel.ToString()) && person.SMS == true)
                                {
                                    MailMessage mailMessage = new MailMessage
                                    {
                                        From = mailaccount,
                                    };
                                    mailMessage.Bcc.Add(person.PhoneEndpoint);
                                    if (message.Length > 160)
                                    {
                                        mailMessage.Body = message.ToString().Substring(0, 160);
                                    }
                                    else
                                    {
                                        mailMessage.Body = message;
                                    }
                                    string userState = "";
                                    client.SendAsync(mailMessage, userState);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
