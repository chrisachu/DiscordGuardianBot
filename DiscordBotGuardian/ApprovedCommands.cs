using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class ApprovedCommands
    {
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
                    if (message.Channel is SocketDMChannel)
                    {
                        // Parse their message based off spaces and quotes so we can recombine if needed
                        var parsedmessage = message.Content.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
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
                                        await message.Channel.SendMessageAsync("Registered: " + message.Author.Mention + Environment.NewLine + "Number: " + parsedmessage[1] + output + Environment.NewLine + "A test message has been sent, if you do not reieve it please try registering again" + Environment.NewLine + "If you want to get messages for a specific channel type !notify in the channel" + Environment.NewLine + "While you have activated your phone endpoint, you still may need to enable getting SMS messages, type !sms to activate");
                                        break;
                                    }
                                }

                            }
                        }
                        // If the command is too short tell them its not complete
                        else if (parsedmessage.Count != 4)
                        {
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Non Complete Command.");
                        }
                        // If the command failed phone validation warn them
                        else if (Validation.Isphonenumber(parsedmessage[1]) == false)
                        {
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Invalid Phone Number. Please enter only numbers.");
                        }
                        // Warn the user based off invalid country code
                        else if (Validation.Isvalidregion(parsedmessage[2]) == false)
                        {
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Invalid Country/Country Code");
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
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Invalid Provider, the valid providers for your country: " + country + ", are ```" + responselist + "```");
                        }
                    }
                    else
                    {
                        await SentDiscordCommands.DeleteLastMessage(context, message.Channel.ToString());
                        await message.Channel.SendMessageAsync(message.Author.Mention + " Please DM the bot to enable this, for you protection we deleted your message so your phone number is not shared.");
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
                else if (splitmessage[0].ToLower() == "!newevent")
                {
                    // Check if the user is an admin or head guardian
                    if (Validation.IsHGorAdminAsync(context, message).Result)
                    {
                        // Make sure we have all parts
                        if (splitmessage.Count == 3)
                        {
                            // Send a message saying we sre starting
                            await message.Channel.SendMessageAsync(message.Author.Mention + " New event is either generating or has been generated. Please give it a few minutes (5+) to complete.");

                            // Parse the message so we can send the correct values
                            var parsedmessage = message.Content.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();

                            // Set up the task to run it
                            NewYear newyeartask = new NewYear();
                            // Start multiple threads to generate the events
                            // Roles
                            new Thread(async () =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                await newyeartask.GenerateRolesAsync(context, parsedmessage[1], Convert.ToInt32(parsedmessage[2]));

                            }).Start();
                            // Make sure we wait so parts can generate before the next starts
                            await Task.Delay(15000);
                            // Categories
                            new Thread(async () =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                await newyeartask.GenerateCategoriesAsync(context, parsedmessage[1], Convert.ToInt32(parsedmessage[2]));

                            }).Start();
                            await Task.Delay(5000);
                            // Channels
                            new Thread(async () =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                await newyeartask.GenerateChannelsAsync(context, parsedmessage[1], Convert.ToInt32(parsedmessage[2]));

                            }).Start();
                        }
                        else
                        {
                            // Warn the user it was a bad command
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Incomplete command. (EX: !newevent Austin 19)");
                        }
                    }
                    else
                    {
                        // Warn the user they dont have perms
                        await message.Channel.SendMessageAsync(message.Author.Mention + " You need to be a head guardian or admin to run this command.");
                    }
                    return true;
                }
                // If the command is to read the DB roles
                else if (splitmessage[0].ToLower() == "!readroles")
                {
                    // Check if the user requesting is an admin or head guardian
                    if (Validation.IsHGorAdminAsync(context, message).Result)
                    {
                        // Read the DB and update the user list
                        users = Database.ReadDB(users);
                        // Notify the user the DB reload has completed
                        await message.Channel.SendMessageAsync(message.Author.Mention + " The DB has been reloaded");
                    }
                    else
                    {
                        // Warn the user they dont have perms
                        await message.Channel.SendMessageAsync(message.Author.Mention + " You need to be a head guardian or admin to run this command.");
                    }
                    return true;
                }
                else if (splitmessage[0].ToLower() == "!deleteevent")
                {
                    // Check if the user is an admin or head guardian
                    if (Validation.IsHGorAdminAsync(context, message).Result)
                    {
                        // Make sure we have all parts
                        if (splitmessage.Count == 3)
                        {
                            // Send a message saying we sre starting
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Previous event is being deleted.");

                            // Parse the message so we can send the correct values
                            var parsedmessage = message.Content.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();

                            // Dump Channels NOT Guardian Bar
                            foreach(Discord.IGuildChannel channel in await context.Guild.GetChannelsAsync())
                            {
                                if (channel.Name.ToLower().Trim().Contains(parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim()))
                                {
                                    if (channel.Name.ToLower().Trim() != "guardian-bar-" + parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim())
                                    {
                                        try
                                        {
                                            await channel.DeleteAsync();
                                        }
                                        catch { }
                                        await Task.Delay(500);

                                    }
                                    else
                                    {
                                        // Get the list of categories
                                        IReadOnlyCollection<IGuildChannel> categories = await context.Guild.GetCategoriesAsync();
                                        ulong categoryId = 000000;
                                        // Check if the category name matches the id if it does return the ID
                                        foreach (var categoryname in categories)
                                        {
                                            if (categoryname.Name.ToLower().Trim() == "archive")
                                            {
                                                categoryId = categoryname.Id;
                                                break;
                                            }
                                        }

                                        // Add it to the category
                                        await channel.ModifyAsync(x =>
                                        {
                                            x.CategoryId = categoryId;
                                        });
                                        await Task.Delay(500);
                                    }
                                }
                            }

                            // Dump Voice Channels
                            foreach (Discord.IVoiceChannel channel in await context.Guild.GetVoiceChannelsAsync())
                            {
                                if (channel.Name.ToLower().Trim().Contains(parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim()))
                                {
                                    try
                                    {
                                        await channel.DeleteAsync();
                                    }
                                    catch { }
                                    await Task.Delay(50);

                                }
                            }

                            // Dump Categories
                            foreach (Discord.ICategoryChannel channel in await context.Guild.GetCategoriesAsync())
                            {
                                if (channel.Name.ToLower().Trim().Contains(parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim()) || channel.Name.ToLower().Trim().Contains(parsedmessage[1].ToLower().Trim() + "-commons-" + parsedmessage[2].ToLower().Trim()))
                                {
                                    try
                                    {
                                        await channel.DeleteAsync();
                                    }
                                    catch { }
                                    await Task.Delay(50);
                                }
                            }

                            // Dump Roles
                            foreach (Discord.IRole existingrole in context.Guild.Roles)
                            {
                                if (existingrole.Name.ToLower().Trim().Contains(parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim()))
                                {
                                    if (existingrole.Name.ToLower().Trim() != "guardian-" + parsedmessage[1].ToLower().Trim() + "-" + parsedmessage[2].ToLower().Trim())
                                    {
                                        try
                                        {
                                            await existingrole.DeleteAsync();
                                        }
                                        catch { }
                                        await Task.Delay(50);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Warn the user it was a bad command
                            await message.Channel.SendMessageAsync(message.Author.Mention + " Incomplete command. (EX: !deleteevent Austin 19)");
                        }
                    }
                    else
                    {
                        // Warn the user they dont have perms
                        await message.Channel.SendMessageAsync(message.Author.Mention + " You need to be a head guardian or admin to run this command.");
                    }
                    return true;
                }
                // If the command is to update a squad lead
                else if (splitmessage[0].ToLower() == "!squadlead")
                {
                    if (Validation.IsTLAsync(context, message).Result)
                    {
                        // Make sure we have all parts
                        if (splitmessage.Count == 4)
                        {
                            var parsedmessage = message.Content.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
                            if (parsedmessage[1].Contains("@") == true)
                            {
                                if (Validation.IsTLForYearAsync(context, message, parsedmessage[2].ToLower().Trim(), parsedmessage[3].ToLower().Trim()).Result)
                                {
                                    var info = await context.Guild.GetUserAsync(message.Author.Id);
                                    bool found = false;
                                    foreach (var role in info.RoleIds)
                                    {
                                        if (found == false)
                                        {
                                            foreach (var rolename in context.Guild.Roles)
                                            {
                                                if (role == rolename.Id && rolename.Name.ToLower().Trim() == "center-stage-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-CS-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                else if (role == rolename.Id && rolename.Name.ToLower().Trim() == "registration-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-RG-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                else if (role == rolename.Id && rolename.Name.ToLower().Trim() == "response-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-RS-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                else if (role == rolename.Id && rolename.Name.ToLower().Trim() == "signatures-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-SG-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                else if (role == rolename.Id && rolename.Name.ToLower().Trim() == "special-rooms-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-SR-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                else if (role == rolename.Id && rolename.Name.ToLower().Trim() == "happy-hour-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim())
                                                {
                                                    foreach (var mention in message.MentionedUsers)
                                                    {
                                                        await SentDiscordCommands.SquadTask(context, "Lead-HH-" + parsedmessage[2].ToLower().Trim() + "-" + parsedmessage[3].ToLower().Trim(), mention.Id);
                                                        await message.Channel.SendMessageAsync(message.Author.Mention + " User has been added as a squad lead");
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync(message.Author.Mention + " You need to @mention a user.");
                            }
                        }
                    }
                    else
                    {
                        // Warn the user they dont have perms
                        await message.Channel.SendMessageAsync(message.Author.Mention + " You need to be a team lead to run this command.");
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
