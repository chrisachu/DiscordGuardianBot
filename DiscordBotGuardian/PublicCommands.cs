using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class PublicCommands
    {
        /// <summary>
        /// Task for commands that everyone can run, not just authenticated users
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> ParsePublicCommandsAsync(SocketMessage message, List<UserData> users, CommandContext context, bool SMSDisabled)
        {
            // Split the message per word so we can parse it
            List<string> splitmessage = message.Content.Split().ToList();
            // Check if the command is to register using rt and they are in the channel landing
            if (splitmessage[0].ToLower() == "!rt" && message.Channel.Name == "landing")
            {
                // Check that the amount of words is correct
                if (splitmessage.Count == 2 || splitmessage.Count == 3)
                {
                    bool found = false;
                    bool updateonly = false;
                    // Check every user to see if the DiscordUsername matches the author ID
                    foreach (UserData user in users)
                    {
                        // Keep looping on false if it dosent match the author
                        if (user.RTUsername.ToLower().Trim() == message.Content.Split()[1].ToLower())
                        {
                            if (user.Event != null)
                            {
                                if (user.Event.Count != 0)
                                {
                                    if (user.DiscordUsername == null)
                                    {
                                        if (user.AuthCode != null)
                                        {
                                            if (splitmessage[2].ToLower().Trim() == user.AuthCode.ToLower().Trim())
                                            {
                                                found = true;
                                                updateonly = false;
                                            }
                                            else
                                            {
                                                found = false;
                                            }
                                        }
                                        else
                                        {
                                            found = false;
                                        }
                                    }
                                    else if (user.DiscordUsername != null)
                                    {
                                        if (user.DiscordUsername == message.Author.Id.ToString().ToLower())
                                        {
                                            found = true;
                                            updateonly = true;
                                        }
                                        else
                                        {
                                            found = false;
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    found = false;
                                    continue;
                                }
                            }
                            else
                            {
                                found = false;
                                continue;
                            }
                        }
                        // Find that the user has not been authenticated yet
                        if (user.RTUsername.ToLower().Trim() == message.Content.Split()[1].ToLower().Trim() && found == true)
                        {
                            // Generate list of roles based off what the user has in the DB
                            List<string> roles = new List<string>();
                            if (user.Roles != null)
                            {
                                if (user.Roles.Count > 0)
                                {
                                    // Assign the users Roles
                                    foreach (string role in user.Roles)
                                    {
                                        roles.Add(role);
                                        // Actually assign the old roles
                                        await SentDiscordCommands.RoleTask(context, role);
                                    }
                                }
                            }
                            if (user.Event != null)
                            {
                                if (user.Event.Count != 0)
                                {
                                    bool updateduser = false;
                                    List<string> events = new List<string>();
                                    foreach (var eventstring in user.Event)
                                    {
                                        if (roles.Contains("Guardian-" + eventstring) == false)
                                        {
                                            roles.Add("Guardian-" + eventstring);
                                            // Update the users DB to contain the new role
                                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Roles", "", users, roles);
                                            // Actually assign the new role
                                            await SentDiscordCommands.RoleTask(context, "Guardian-" + eventstring);
                                            events.Add(eventstring);
                                            found = true;
                                            updateduser = true;
                                        }
                                    }
                                    if (updateduser == true)
                                    {
                                        // Update the variable
                                        foreach (var eventstring in events)
                                        {
                                            user.Event.RemoveAt(user.Event.IndexOf(eventstring));
                                        }
                                        // Change their nickname
                                        try
                                        {
                                            var messageuser = message.Author as SocketGuildUser;
                                            await messageuser.ModifyAsync(x =>
                                            {
                                                x.Nickname = user.RTUsername.Trim();
                                            });
                                        }
                                        catch { }
                                        await Task.Delay(1000);
                                        // Remove the event from the Users info and only store it in the roles param
                                        users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Event", "", users, null);
                                    }
                                }
                            }
                            break;
                        }
                    }
                    // Send a message saying they auth'd and that they accepted the TOS
                    if (found == true)
                    {
                        await SentDiscordCommands.DeleteLastMessage(context, "landing");
                        await message.Channel.SendMessageAsync(message.Author.Mention + " Your Discord user has now been authenticated as a Guardian and accepted the TOS");
                        // Update the user info in the Sheets DB
                        if (updateonly == false)
                        {
                            await Task.Delay(5000);
                            users = Database.UpdateUser(message.Content.Split()[1].ToLower(), "DiscordUsername", message.Author.Id.ToString().ToLower(), users);
                            await Task.Delay(1000);
                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Authenticated", "TRUE", users);
                            await Task.Delay(1000);
                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "AuthCode", "NULL", users);
                        }
                    }
                    // Kick back and error if they didn't auth correctly
                    else
                    {
                        await SentDiscordCommands.DeleteLastMessage(context, "landing");
                        await message.Channel.SendMessageAsync(message.Author.Mention + " Your RT Username is already authenticated as another user or you are not registered as a Guardian. Your auth code may also be incorrect. Contact a HG or an Admin if you think this is incorrect.");
                    }
                    return true;
                }
            }
            // Check if its a help message
            else if (splitmessage[0].ToLower() == "!help")
            {
                // If its in landing only show them !rt so they dont have too much going on
                if (message.Channel.Name == "landing")
                {
                    await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!rt USERNAME *AUTHCODE* - to log in to be authenticated and accept the TOS (Auth Code is only required if you have not registered before)");
                }
                // Else show them every command available
                else
                {
                    if (SMSDisabled == false)
                    {
                        await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!sms - To enable or disable SMS for your device (You will have to register your phone if you have not yet)" + Environment.NewLine + "!notify - Type it in the channel you want to receive SMS notifications for" + Environment.NewLine + "!registerphone 1234567890 \"United States\" \"Verizon Wireless\" - Use this command to register your phone as a device to get texts on. (Number, Country, Carrier)" + Environment.NewLine + "(Admin Only) !newevent EVENT YEAR - Generates a new guardian event and channels for RTX" + Environment.NewLine + "(Admin Only) !deleteevent EVENT YEAR - Deletes a previous set of RTX channels and roles, not including each bar" + Environment.NewLine + "(Admin Only) !readroles - Updates everyones team roles based off the DB sheet" + Environment.NewLine + "(Team Lead Only) !squadlead USER EVENT YEAR - Makes a user a squad lead for that users team");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "(Admin Only) !newevent EVENT YEAR - Generates a new guardian event and channels for RTX" + Environment.NewLine + "(Admin Only) !deleteevent EVENT YEAR - Deletes a previous set of RTX channels and roles, not including each bar" + Environment.NewLine + "(Admin Only) !readroles - Updates everyones team roles based off the DB sheet" + Environment.NewLine + "(Team Lead Only) !squadlead USER EVENT YEAR - Makes a user a squad lead for that users team");
                    }
                }
                return true;
            }
            return false;
        }
    }
}
