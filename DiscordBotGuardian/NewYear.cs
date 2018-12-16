using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class NewYear
    {
        public async Task CreateNewYearAsync(CommandContext Context, string Event, string Year)
        {
            // If the variables are empty return
            if(Event == string.Empty || Year == string.Empty) {return;}

            /// Create Roles
            // Global Roles
            await CreateRole(Context, "Admin", "Admin", Color.Purple, true);
            await CreateRole(Context, "Head-Guardian", "Admin", Color.Red, true);
            await CreateRole(Context, "Staff", "Mod", Color.Red, true);
            await CreateRole(Context, "Team-Lead", "Mod", Color.Blue, true);
            await CreateRole(Context, "Mod", "Mod", Color.LightOrange, true);

            // Team Related Roles
            await CreateRole(Context, "Team-Lead-" + Event, "Display", Color.Default, false);
            await CreateRole(Context, "Crisis-Management-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Center-Stage-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-CS-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Dispatch-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Expo-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Freelancer-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "PA-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "PAL-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Panels-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Registration-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-RG-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Reponse-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-RS-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Signatures-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-SG-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Special-Rooms-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-SR-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Store-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Tech-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Happy-Hour-" + Event + "-" + Year, "Standard", Color.Default, false);
            await CreateRole(Context, "Lead-HH-" + Event + "-" + Year, "Standard", Color.Default, false);

            // Actual Event Role
            await CreateRole(Context, "Guardian-" + Event + "-" + Year, "Standard", Color.Green, true);



            /// Create Channels
            // TODO: Skipping global channels for now in favor of doing that later with checking if they exist or not




            //CreateCategory(Context, "command-chat-" + Event + "-" + Year)


        }

        /// Create a Category using the channel using roles provided
        private async Task CreateCategory(CommandContext Context, string category, [Optional]List<string> roles)
        {
            // TODO: Parse in Roles so when a category is created we use the roles provided
            await Context.Guild.CreateCategoryAsync(category);
        }

        /// Create a Text Channel using the name and roles provided
        private async Task CreateChannel(CommandContext Context, string channel, [Optional]List<string> roles)
        {
            // TODO: Parse in the roles to add them to the channel
            await Context.Guild.CreateTextChannelAsync(channel);
        }

        /// After you create a channel we can add it to a category
        /// Unfortunately we cannot do both in the same task or it creates a new category each time
        private async Task AddChannelToCategory(CommandContext Context, string channel, string category)
        {
            // Get the current list of channels in the discord
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetChannelsAsync();
            ulong channelId = 000000;
            // Check if the channel exists and return the ID
            foreach (var channelname in channels)
            {
                if (channelname.Name == channel)
                {
                    channelId = channelname.Id;
                    break;
                }
            }

            // Get the list of categories
            IReadOnlyCollection<IGuildChannel> categories = await Context.Guild.GetCategoriesAsync();
            ulong categoryId = 000000;
            // Check if the category name matches the id if it does return the ID
            foreach (var categoryname in categories)
            {
                if (categoryname.Name == category)
                {
                    categoryId = categoryname.Id;
                    break;
                }
            }

            // Get the text channels info specificially so we can modify it
            var channelinfo = await Context.Guild.GetTextChannelAsync(channelId);

            // Add it to the correct category
            await channelinfo.ModifyAsync(x => x.CategoryId = categoryId);
        }

        /// Used for creating a discord role in the current guild
        /// <summary> Allowed Perms are Admin, Mod, Standard and Display </summary>
        private async Task CreateRole(CommandContext Context, string Role, string Perms, Color RoleColor, bool DisplayedRole)
        {
            // Before we go any further let's see if the role already exists
            // If the role exists exit the task
            foreach(Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if(existingrole.Name == Role)
                {
                    return;
                }
            }

            // Set up the base permissions flags for creating a new role rather than manually generating it each time
            GuildPermissions adminpermissions = new GuildPermissions(createInstantInvite: true, kickMembers: true, banMembers: true, administrator: true, manageChannels: true, manageGuild: true, addReactions: true, viewAuditLog: true, readMessages: true, sendMessages: true, sendTTSMessages: true, manageMessages: true, embedLinks: true, attachFiles: true, readMessageHistory: true, mentionEveryone: true, useExternalEmojis: true, connect: true, speak: true, muteMembers: true, deafenMembers: true, moveMembers: true, useVoiceActivation: true, changeNickname: true, manageNicknames: true, manageRoles: true, manageWebhooks: true, manageEmojis: true);
            GuildPermissions modpermissions = new GuildPermissions(createInstantInvite: false, kickMembers: false, banMembers: false, administrator: false, manageChannels: false, manageGuild: false, addReactions: true, viewAuditLog: false, readMessages: true, sendMessages: true, sendTTSMessages: false, manageMessages: true, embedLinks: true, attachFiles: true, readMessageHistory: true, mentionEveryone: false, useExternalEmojis: true, connect: true, speak: true, muteMembers: true, deafenMembers: true, moveMembers: false, useVoiceActivation: true, changeNickname: true, manageNicknames: true, manageRoles: false, manageWebhooks: false, manageEmojis: false);
            GuildPermissions standardpermissions = new GuildPermissions(createInstantInvite: false, kickMembers: false, banMembers: false, administrator: false, manageChannels: false, manageGuild: false, addReactions: true, viewAuditLog: false, readMessages: true, sendMessages: true, sendTTSMessages: false, manageMessages: false, embedLinks: true, attachFiles: true, readMessageHistory: true, mentionEveryone: false, useExternalEmojis: true, connect: true, speak: true, muteMembers: false, deafenMembers: false, moveMembers: false, useVoiceActivation: true, changeNickname: true, manageNicknames: false, manageRoles: false, manageWebhooks: false, manageEmojis: false);
            GuildPermissions displayonlypermissions = new GuildPermissions(createInstantInvite: false, kickMembers: false, banMembers: false, administrator: false, manageChannels: false, manageGuild: false, addReactions: false, viewAuditLog: false, readMessages: false, sendMessages: false, sendTTSMessages: false, manageMessages: false, embedLinks: false, attachFiles: false, readMessageHistory: false, mentionEveryone: false, useExternalEmojis: false, connect: false, speak: false, muteMembers: false, deafenMembers: false, moveMembers: false, useVoiceActivation: false, changeNickname: false, manageNicknames: false, manageRoles: false, manageWebhooks: false, manageEmojis: false);

            GuildPermissions roleperms;
            // Check what flag was passed because C# dosen't have a dynamic way to only give a specific list of options
            if (Perms.ToLower() == "admin")
            {
                // Create the Role using the passed flags
                roleperms = adminpermissions;
            }
            else if(Perms.ToLower() == "mod")
            {
                roleperms = modpermissions;
            }
            else if(Perms.ToLower() == "standard")
            {
                roleperms = standardpermissions;
            }
            else
            {
                // If it dosen't have the right passed flag just make it a display role
                roleperms = displayonlypermissions;
            }

            // Actually create the role using the provided settings
            await Context.Guild.CreateRoleAsync(Role, roleperms, RoleColor, DisplayedRole);
        }

    }
}
