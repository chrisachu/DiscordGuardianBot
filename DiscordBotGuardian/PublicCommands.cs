using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotGuardian
{
    public class PublicCommands
    {
        /// <summary>
        /// Task for commands that everyone can run, not just authenticated users
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> ParsePublicCommandsAsync(SocketMessage message, List<UserData> users, CommandContext context)
        {
            // Split the message per word so we can parse it
            List<string> splitmessage = message.Content.Split().ToList();
            // Check if the command is to register using rt and they are in the channel landing
            if (splitmessage[0].ToLower() == "!rt" && message.Channel.Name == "landing")
            {
                // Check that the amount of words is correct
                if (splitmessage.Count == 2)
                {
                    bool found = false;
                    // Check every user to see if the DiscordUsername matches the author ID
                    foreach (UserData user in users)
                    {
                        // Keep looping on false if it dosent match the author
                        if (user.DiscordUsername == message.Author.Id.ToString() && user.RTUsername != message.Content.Split()[1].ToLower())
                        {
                            found = false;
                        }
                        // Find that the user has not been authenticated yet
                        else if (user.RTUsername == message.Content.Split()[1].ToLower() && user.Authenticated == false)
                        {
                            // Update the user info in the Sheets DB
                            users = Database.UpdateUser(message.Content.Split()[1].ToLower(), "DiscordUsername", message.Author.Id.ToString().ToLower(), users);
                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Authenticated", "TRUE", users);
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
                                    }
                                }
                            }
                            // ToDo: Assign correct role not hardcoded
                            if (roles.Contains("Guardian-US-2018") == false)
                            {
                                roles.Add("Guardian-US-2018");
                            }
                            // Update the users DB to contain the new role
                            users = Database.UpdateUser(message.Author.Id.ToString().ToLower(), "Roles", "", users, roles);
                            // Actually assign the new role
                            await SentDiscordCommands.RoleTask(context, "Guardian-US-2018");
                            found = true;
                            break;
                        }
                    }
                    // Send a message saying they auth'd and that they accepted the TOS
                    if (found == true)
                    {
                        await message.Channel.SendMessageAsync(message.Author.Mention + " Your Discord user has now been authenticated as a Guardian and accepted the TOS");
                    }
                    // Kick back and error if they didn't auth correctly
                    else
                    {
                        await message.Channel.SendMessageAsync(message.Author.Mention + " Your RT Username is already authenticated as another user or you are not registered as a Guardian. Contact a HG or an Admin if you think this is incorrect.");
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
                    await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!rt USERNAME - to log in to be authenticated and accept the TOS");
                }
                // Else show them every command available
                else
                {
                    await message.Channel.SendMessageAsync("The commands available are, " + Environment.NewLine + "!sms - To enable or disable SMS for your device (You will have to register your phone if you have not yet)" + Environment.NewLine + "!notify - Type it in the channel you want to receive SMS notifications for" + Environment.NewLine + "!registerphone 1234567890 \"United States\" \"Verizon Wireless\" - Use this command to register your phone as a device to get texts on. (Number, Country, Carrier)");
                }
                return true;
            }
            return false;
        }
    }
}
