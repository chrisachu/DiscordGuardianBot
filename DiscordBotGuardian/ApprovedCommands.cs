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
        // ToDo: Add Angela Lansbury Tagging commands
        // ToDo: Prefer phone commands through DM rather than in chat
        /// <summary>
        /// Task for command parsing that only authenticated users can run
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> ParsePrivateCommandsAsync(SocketMessage message, List<UserData> users, SmtpClient client, CommandContext context, MailAddress mailaccount)
        {
            // Split the message so we can parse it
            List<string> splitmessage = message.Content.Split().ToList();
            // Check if the user is authenticated
            if (Validation.IsUserAuthenticated(message.Author.Id.ToString(), users) == true)
            {
                // Check if the command is for registering their phone
                if (splitmessage[0].ToLower() == "!registerphone")
                {
                    // Parse their message based off spaces and quotes so we can recombine if needed
                    var parsedmessage = message.Content.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries): new string[] { element }).SelectMany(element => element).ToList();
                    // Make sure we have 4 parts of the message set, and validate the params passed
                    if (parsedmessage.Count == 4 && Validation.Isphonenumber(parsedmessage[1]) && Validation.Isvalidregion(parsedmessage[2]) && Validation.Isvalidcarrier(parsedmessage[3]))
                    {
                        string country = "";
                        // Check the length of the message and either assign it direct or parse the carrier name
                        if (parsedmessage[2].Length <= 3)
                        {
                            country = Validation.ParseCarrierName(parsedmessage[2]);
                        }
                        else
                        {
                            country = parsedmessage[2];
                        }
                        // Check against all of the SMS endpoints in the SMS.json file
                        foreach (SMS carrier in Validation.LoadSMSData())
                        {
                            if (country.ToLower() == carrier.Country.ToLower())
                            {
                                // If the Carrier and Country match
                                if (parsedmessage[3].ToLower() == carrier.Carrier.ToLower())
                                {
                                    // Get everything after the @ sign so we can assign the users number to it
                                    string output = carrier.EmailToSms.Substring(carrier.EmailToSms.IndexOf('@'));
                                    // Update the DB to the correct phone endpoint
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "PhoneEndpoint", parsedmessage[1] + output, users);
                                    // Create a test mail message for testing
                                    MailMessage mailMessage = new MailMessage
                                    {
                                        From = mailaccount,
                                    };
                                    // Add the users phone endpoint as the Bcc
                                    mailMessage.Bcc.Add(parsedmessage[1] + output);
                                    // Set the body as a test notification alert
                                    mailMessage.Body = "This is a test notification for the Guardian Discord.";
                                    string userState = "";
                                    // Send the text message
                                    client.SendAsync(mailMessage, userState);
                                    // Reply in the Discord chat saying they have been authenticated
                                    await message.Channel.SendMessageAsync("Registered: " + message.Author.Mention + Environment.NewLine + "Number: " + parsedmessage[1] + output + Environment.NewLine + "A test message has been sent, if you do not reieve it please try registering again" + Environment.NewLine + "If you want to get messages for a specific channel type !SMS in the channel");
                                    break;
                                }
                            }

                        }
                    }
                    // If the command is too short tell them its not complete
                    else if (parsedmessage.Count != 4)
                    {
                        await message.Channel.SendMessageAsync("Non Complete Command.");
                    }
                    // If the command failed phone validation warn them
                    else if (Validation.Isphonenumber(parsedmessage[1]) == false)
                    {
                        await message.Channel.SendMessageAsync("Invalid Phone Number. Please enter only numbers.");
                    }
                    // Warn the user based off invalid country code
                    else if (Validation.Isvalidregion(parsedmessage[2]) == false)
                    {
                        await message.Channel.SendMessageAsync("Invalid Country/Country Code");
                    }
                    // If its a valid region but the carrier is incorrect tell them the correct list of providers they can send
                    else if (Validation.Isvalidcarrier(parsedmessage[3]) == false && Validation.Isvalidregion(parsedmessage[2]) == true)
                    {
                        string country = "";
                        // Parse the carrier name
                        if (parsedmessage[2].Length <= 3)
                        {
                            country = Validation.ParseCarrierName(parsedmessage[2]);
                        }
                        else
                        {
                            country = parsedmessage[2];
                        }
                        string responselist = "";
                        // Check the SMS.json database for the providers supported for their region
                        foreach (SMS carrier in Validation.LoadSMSData())
                        {
                            if (country.ToLower() == carrier.Country.ToLower())
                            {
                                // set the result with the correct params
                                responselist = responselist + Environment.NewLine + carrier.Carrier;// + " " + carrier.Notes;
                            }
                        }
                        // Send a reply in chat with their valid providers
                        await message.Channel.SendMessageAsync("Invalid Provider, the valid providers for your country: " + country + ", are ```" + responselist + "```");
                    }
                    return true;
                }
                // If the command sent is notify
                else if (splitmessage[0].ToLower() == "!notify")
                {
                    // Check every user in the userdatabase
                    foreach (UserData person in users)
                    {
                        if (person.DiscordUsername != null)
                        {
                            // Match based off their user ID
                            if (person.DiscordUsername.ToLower() == message.Author.Id.ToString().ToLower())
                            {
                                if (person.Channels != null)
                                {
                                    // If the user already has the channel in the list of channels remove it
                                    if (person.Channels.Contains(message.Channel.Name))
                                    {
                                        person.Channels.Remove(message.Channel.Name);
                                        // Update the DB
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        // Send a message saying it was removed from sending messages
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages for this channel");
                                    }
                                    else
                                    {
                                        // Else add the channel to the list of alerting channels
                                        person.Channels.Add(message.Channel.Name);
                                        // Update the DB
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        // Send a message saying they will recieve messages in that channel now
                                        await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                    }
                                }
                                else
                                {
                                    // Create a new list of channels if no channels are already existing starting with the one they are in
                                    person.Channels = new List<string>
                                    {
                                        message.Channel.Name
                                    };
                                    // Update the DB
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                    // Send the message that they will now get the alerts
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages for this channel");
                                }
                            }
                        }
                    }
                    return true;
                }
                // If the message sent was SMS
                else if (splitmessage[0].ToLower() == "!sms")
                {
                    // Check every user in the DB to see if it matches the sending user
                    foreach (UserData person in users)
                    {
                        if (person.DiscordUsername != null)
                        {
                            // Compare their ID's
                            if (person.DiscordUsername.ToLower() == message.Author.Id.ToString().ToLower())
                            {
                                // Check if the person has SMS disabled
                                if (person.SMS == false)
                                {
                                    // If its disabled, enable it and write to the DB
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "true", users);
                                    // If they have no channels assigned add the alerts channel as a requirement
                                    if (person.Channels != null)
                                    {
                                        if (person.Channels.Contains("alerts") == false)
                                        {
                                            person.Channels.Add(message.Channel.Name);
                                            // Update the DB
                                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                        }

                                    }
                                    else
                                    {
                                        // If they have no pre existing channels force add the alerts channel
                                        person.Channels = new List<string>
                                        {
                                            "alerts"
                                        };
                                        // Update the DB
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Channels", "", users, person.Channels);
                                    }
                                    // Notify the user they now have SMS enabled
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will now recieve SMS messages, please rememeber to set up your phone provider if you have not");
                                }
                                else
                                {
                                    // If they already have SMS enabled disable it
                                    users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "SMS", "false", users);
                                    // Notify the user they will no longer get SMS
                                    await message.Channel.SendMessageAsync(message.Author.Mention + " You will no longer recieve SMS messages");
                                }


                            }
                        }
                    }
                    return true;
                }
                // ToDo: Update this command so it actually parses and is only an admin based command
                // ToDo: Add a force read of the DB command for when they add roles
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
