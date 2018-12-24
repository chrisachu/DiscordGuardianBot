using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class SentDiscordCommands
    {
        /// <summary>
        /// Used for updating a users role for reg
        /// </summary>
        public static async Task RoleTask(CommandContext Context, string role)
        {
            // Search for the roles in the server
            var roles = Context.Guild.Roles;
            ulong roleId = 000000;
            bool found = false;
            // See if the role you are sending matches one on the server
            foreach (var singlerole in roles)
            {
                if (singlerole.Name == role)
                {
                    roleId = singlerole.Id;
                    found = true;
                    break;
                }
            }
            // If we found the role update it for the user
            if (found == true)
            {
                IRole roleid = Context.Guild.GetRole(roleId);
                await ((SocketGuildUser)Context.User).AddRoleAsync(roleid);
            }
        }
        /// <summary>
        /// Used for updating a users role for Sqds
        /// </summary>
        public static async Task SquadTask(CommandContext Context, string role, ulong user)
        {
            // Search for the roles in the server
            var roles = Context.Guild.Roles;
            ulong roleId = 000000;
            bool found = false;
            // See if the role you are sending matches one on the server
            foreach (var singlerole in roles)
            {
                if (singlerole.Name.ToLower().Trim() == role.ToLower().Trim())
                {
                    roleId = singlerole.Id;
                    found = true;
                    break;
                }
            }
            // If we found the role update it for the user
            if (found == true)
            {
                IRole roleid = Context.Guild.GetRole(roleId);
                await (await Context.Guild.GetUserAsync(user)).AddRoleAsync(roleid);
            }
        }
        /// <summary>
        /// Used for deleting the last sent message in a channel (Only used for Rulebook currently)
        /// </summary>
        public static async Task DeleteLastMessage(CommandContext Context, string Channel)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetChannelsAsync();
            // Check if the channel exists
            foreach (var channelname in channels)
            {
                if (channelname.Name.ToString().ToLower().Trim() == Channel.ToString().ToLower().Trim())
                {
                    try
                    {
                        // Convert the found channel to an IMessageChannel
                        var message = channelname as IMessageChannel;
                        // Pull the last message and change it to a usable format
                        var messages = await message.GetMessagesAsync(1).FlattenAsync();
                        // Delete the message based of its ID
                        await message.DeleteMessageAsync(messages.ToList()[0].Id);
                    }
                    catch { }
                }
            }
            return;
        }

    }
}
