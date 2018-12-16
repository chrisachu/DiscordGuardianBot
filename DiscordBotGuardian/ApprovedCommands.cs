using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;

namespace DiscordBotGuardian
{
    public class ApprovedCommands
    {
        // ToDo: Comments for the ApprovedCommands Class
        public static async System.Threading.Tasks.Task<bool> ParsePrivateCommandsAsync(SocketMessage message, List<UserData> users, SmtpClient client, CommandContext context)
        {
            List<string> splitmessage = message.Content.Split().ToList();
            bool authenticated = Validation.IsUserAuthenticated(message.Author.Id.ToString(), users);
            if (authenticated == true)
            {
                if (splitmessage[0].ToLower() == "!registerphone" && authenticated)
                {
                    var parsedmessage = message.Content.Split('"')
                                         .Select((element, index) => index % 2 == 0  // If even index
                                                               ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                                               : new string[] { element })  // Keep the entire item
                                         .SelectMany(element => element).ToList();
                    if (parsedmessage.Count == 4 && Validation.Isphonenumber(parsedmessage[1]) && Validation.Isvalidregion(parsedmessage[2]) && Validation.Isvalidcarrier(parsedmessage[3]))
                    {
                        string country = "";
                        if (parsedmessage[2].Length <= 3)
                        {
                            country = Validation.ParseCarrierName(parsedmessage[2]);
                        }
                        else
                        {
                            country = parsedmessage[2];
                        }

                        foreach (SMS carrier in Validation.LoadSMSData())
                        {
                            if (country.ToLower() == carrier.Country.ToLower())
                            {
                                if (parsedmessage[3].ToLower() == carrier.Carrier.ToLower())
                                {
                                    string output = carrier.EmailToSms.Substring(carrier.EmailToSms.IndexOf('@'));
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "PhoneEndpoint", parsedmessage[1] + output, users);
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
                    else if (Validation.Isvalidregion(parsedmessage[2]) == false)
                    {
                        await message.Channel.SendMessageAsync("Invalid Country/Country Code");
                    }
                    else if (Validation.Isvalidcarrier(parsedmessage[3]) == false && Validation.Isvalidregion(parsedmessage[2]) == true)
                    {
                        string country = "";
                        if (parsedmessage[2].Length <= 3)
                        {
                            country = Validation.ParseCarrierName(parsedmessage[2]);
                        }
                        else
                        {
                            country = parsedmessage[2];
                        }
                        string responselist = "";
                        foreach (SMS carrier in Validation.LoadSMSData())
                        {
                            if (country.ToLower() == carrier.Country.ToLower())
                            {
                                responselist = responselist + Environment.NewLine + carrier.Carrier;// + " " + carrier.Notes;
                            }
                        }
                        await message.Channel.SendMessageAsync("Invalid Provider, the valid providers for your country: " + country + ", are ```" + responselist + "```");
                    }
                    return true;
                }
                else if (splitmessage[0].ToLower() == "!notify" && authenticated)
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
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages for this channel");
                                    }
                                    else
                                    {
                                        person.Channels.Add(message.Channel.Name);
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                    }
                                }
                                else
                                {
                                    person.Channels = new List<string>
                                    {
                                        message.Channel.Name
                                    };
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                }
                            }
                        }
                    }
                    return true;
                }
                else if (splitmessage[0].ToLower() == "!sms" && authenticated)
                {
                    foreach (UserData person in users)
                    {
                        if (person.DiscordUsername != null)
                        {
                            if (person.DiscordUsername.ToLower() == message.Author.Id.ToString().ToLower())
                            {
                                if (person.SMS == false)
                                {
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "true", users);
                                    if (person.Channels != null)
                                    {
                                        if (person.Channels.Contains("announcements") == false)
                                        {
                                            person.Channels.Add(message.Channel.Name);
                                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        }

                                    }
                                    else
                                    {
                                        person.Channels = new List<string>
                                        {
                                            "announcements"
                                        };
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                    }
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages, please rememeber to set up your phone provider if you have not");
                                }
                                else
                                {
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "false", users);
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages");
                                }


                            }
                        }
                    }
                    return true;
                }
                else if (splitmessage[0].ToLower() == "!testevent")
                {
                    NewYear test = new NewYear();
                    new Thread(async () =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        await test.CreateNewYearAsync(context, "Austin", 19);
                    }).Start();
                    return true;
                }
            }
            return false;
        }
    }
}
