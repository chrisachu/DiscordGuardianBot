using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class SentDiscordCommands
    {
        public static async Task RoleTask(CommandContext Context, string role)
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

    }
}
