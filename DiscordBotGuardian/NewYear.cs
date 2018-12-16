using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class NewYear
    {
        /// <summary>
        /// Used for generating a new set of roles and channels for a new year
        /// </summary>
        public async Task CreateNewYearAsync(CommandContext Context, string Event, string Year)
        {
            // If the variables are empty return
            if (Event == string.Empty || Year == string.Empty) { return; }

            /// Create Roles
            /// 
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

            /// Generate list of event roles currently existing for global use
            /// 
            // Pull current events
            List<string> GuardianEvents = new List<string>();
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name.Contains("Guardian-"))
                {
                    GuardianEvents.Add(existingrole.Name);
                }
            }
            // Add the event we are adding
            GuardianEvents.Add("Guardian-" + Event + "-" + Year);

            /// Create Categories before we make channels
            /// 
            // Global Categories
            // ToDo: allow for permission updates for previous existing categories
            // ToDo: Implicit vs explict deny of channels
            await CreateCategory(Context, "Administration", null, 1);
            await CreateCategory(Context, "Global", GuardianEvents, 2);

            // Event Based Categories
            await CreateCategory(Context, Event + "-" + Year, new List<string> { "Guardian-" + Event + "-" + Year });
            await CreateCategory(Context, "Command-Center-" + Event + "-" + Year, new List<string> { "Admin", "Staff", "Head-Guardian", "Team-Lead-" + Event + "-" + Year });
            await CreateCategory(Context, Event + "-Commons-" + Year, new List<string> { "Guardian-" + Event + "-" + Year });
            await CreateCategory(Context, "Center-Stage-" + Event + "-" + Year, new List<string> { "Center-Stage-" + Event + "-" + Year });
            await CreateCategory(Context, "Dispatch-" + Event + "-" + Year, new List<string> { "Dispatch-" + Event + "-" + Year });
            await CreateCategory(Context, "Expo-" + Event + "-" + Year, new List<string> { "Expo-" + Event + "-" + Year });
            await CreateCategory(Context, "Freelancer-" + Event + "-" + Year, new List<string> { "Freelancer-" + Event + "-" + Year });
            await CreateCategory(Context, "PA-" + Event + "-" + Year, new List<string> { "PA-" + Event + "-" + Year });
            await CreateCategory(Context, "Panels-" + Event + "-" + Year, new List<string> { "Panels-" + Event + "-" + Year });
            await CreateCategory(Context, "Registration-" + Event + "-" + Year, new List<string> { "Registration-" + Event + "-" + Year });
            await CreateCategory(Context, "Response-" + Event + "-" + Year, new List<string> { "Response-" + Event + "-" + Year });
            await CreateCategory(Context, "Signatures-" + Event + "-" + Year, new List<string> { "Signatures-" + Event + "-" + Year });
            await CreateCategory(Context, "Special-Rooms-" + Event + "-" + Year, new List<string> { "Special-Rooms-" + Event + "-" + Year });
            await CreateCategory(Context, "Store-" + Event + "-" + Year, new List<string> { "Store-" + Event + "-" + Year });
            await CreateCategory(Context, "Tech-" + Event + "-" + Year, new List<string> { "Tech-" + Event + "-" + Year });
            await CreateCategory(Context, "Happy-Hour-" + Event + "-" + Year, new List<string> { "Happy-Hour-" + Event + "-" + Year });
            //ToDo: Voice Lines?

            /// Create Channels
            /// 
            // Administration Category Channels
            await CreateChannel(Context, "alerts", GuardianEvents, "Announcements for everyone.", "Administration", 1);
            await CreateChannel(Context, "admins", new List<string> { "Admin" }, "Admin Chat", "Administration", 2);
            await CreateChannel(Context, "mods", new List<string> { "Admin", "Mod" }, "Mod Chat", "Administration", 3);
            // ToDo: Create report-request-to-admins
            // ToDo: Create rulebook, requires explicit deny for all to write to
            // ToDo: Create landing, requires explicit rule changes for the channel

            // Global Category Channels
            // ToDo: Test if passing null is allowing for inherit
            await CreateChannel(Context, "guardian-lounge", null, "Our Patron Saint, Dame Angela Lansbury", "Global", 1);
            await CreateChannel(Context, "movies-shows", null, "Talking about movies, tv shows, and rabb.it", "Global", 2);
            await CreateChannel(Context, "music", null, "What's on your playlist?", "Global", 3);
            await CreateChannel(Context, "games", null, "Party up.", "Global", 4);
            await CreateChannel(Context, "pic-dump", null, "Cute things and memes go here.", "Global", 5);
            await CreateChannel(Context, "self-promotion", null, "Share your beautiful things for all to see!", "Global", 6);
            await CreateVoiceChannel(Context, "global-vc-1", null, "Global", 7);
            await CreateVoiceChannel(Context, "global-vc-2", null, "Global", 8);

            // Event Category Channels
            // Command Center
            await CreateChannel(Context, "command-chat-" + Event + "-" + Year, new List<string> { "Admin", "Head-Guardian", "Team-Lead-" + Event + "-" + Year }, "HGs and TLs", "Command-Center-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "crisis-center-" + Event + "-" + Year, new List<string> { "Admin", "Head-Guardian", "Crisis-Management-" + Event + "-" + Year }, "Direct line: HGs and Response TLs.", "Command-Center-" + Event + "-" + Year, 2);
            // ToDo: Create Announcements with explicit settings
            // ToDo: Create Links with explicit settings

            // Commons
            await CreateChannel(Context, "guardian-bar-" + Event + "-" + Year, null, "There will always be a Guardian Bar", Event + "-Commons-" + Year, 1);
            await CreateChannel(Context, "ride-room-share-" + Event + "-" + Year, null, "Because no one uses the forum anymore.", Event + "-Commons-" + Year, 2);
            await CreateChannel(Context, "meetups-events-" + Event + "-" + Year, null, "It's like matchmaking, but in real life.", Event + "-Commons-" + Year, 3);
            await CreateChannel(Context, "town-hall-" + Event + "-" + Year, null, "Ask the HGs anything appropriate.", Event + "-Commons-" + Year, 4);
            await CreateVoiceChannel(Context, Event + "-vc-1", null, Event + "-Commons-" + Year, 5);
            await CreateVoiceChannel(Context, Event + "-vc-2", null, Event + "-Commons-" + Year, 6);
            // ToDo: Add in Town Hall VC Requires explicit deny

            // Team Chats
            // ToDo: Add all the team chats
        }

        /// <summary>
        /// Create a Category using the channel using roles provided
        /// </summary>
        private async Task CreateCategory(CommandContext Context, string Category, [Optional]List<string> Roles, [Optional]int? Position)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> categories = await Context.Guild.GetCategoriesAsync();
            // Check if the channel exists
            foreach (var categoryname in categories)
            {
                if (categoryname.Name == Category)
                {
                    // If the channel exists exit
                    return;
                }
            }

            // Create the Category
            var newcategory = await Context.Guild.CreateCategoryAsync(Category);

            // Check if we are passing roles
            if (Roles != null)
            {
                // Parse in the roles to add them to the channel
                foreach (string role in Roles)
                {
                    // Before we go any further let's see if the role already exists
                    // If the role exists exit the task
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name == role)
                        {
                            // Add the selected roles to the channel using inhert as its base
                            OverwritePermissions inheret = new OverwritePermissions();
                            await newcategory.AddPermissionOverwriteAsync(existingrole, inheret);
                            break;
                        }
                    }
                }
                // Remove the everyone permission if it's not in the list
                if (Roles.Contains("Everyone") == false)
                {
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name.ToLower() == "everyone")
                        {
                            // Remove Everyones permissions
                            await newcategory.RemovePermissionOverwriteAsync(existingrole);
                            break;
                        }
                    }
                }
            }
            // Check if a position was provided
            if (Position != null)
            {
                // Update its position
                await newcategory.ModifyAsync(x =>
                {
                    x.Position = Position.Value;
                });
            }
        }

        /// <summary>
        /// Create a Voice Channel using the name and roles provided
        /// </summary>
        private async Task CreateVoiceChannel(CommandContext Context, string VoChannel, [Optional]List<string> Roles, [Optional]string Category, [Optional]int? Position)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetVoiceChannelsAsync();
            // Check if the channel exists
            foreach (var channelname in channels)
            {
                if (channelname.Name == VoChannel)
                {
                    // If the channel exists exit
                    return;
                }
            }
            Discord.IVoiceChannel newchannel = await Context.Guild.CreateVoiceChannelAsync(VoChannel);
            // Check if roles were passed
            if (Roles != null)
            {
                // Parse in the roles to add them to the channel
                foreach (string role in Roles)
                {
                    // Before we go any further let's see if the role already exists
                    // If the role exists exit the task
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name == role)
                        {
                            // Add the selected roles to the channel using inhert as its base
                            // ToDo: Allow for read only
                            OverwritePermissions inheret = new OverwritePermissions();
                            await newchannel.AddPermissionOverwriteAsync(existingrole, inheret);
                            break;
                        }
                    }
                }
                // ToDo: Determine if removing the everyone permission is possible
                // Remove the everyone permission if it's not in the list
                if (Roles.Contains("Everyone") == false)
                {
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name.ToLower() == "everyone")
                        {
                            // Remove Everyones permissions
                            await newchannel.RemovePermissionOverwriteAsync(existingrole);
                            break;
                        }
                    }
                }
            }
            // Check if a category was passed
            if (Category != null)
            {
                // Get the list of categories
                IReadOnlyCollection<IGuildChannel> categories = await Context.Guild.GetCategoriesAsync();
                ulong categoryId = 000000;
                // Check if the category name matches the id if it does return the ID
                foreach (var categoryname in categories)
                {
                    if (categoryname.Name == Category)
                    {
                        categoryId = categoryname.Id;
                        break;
                    }
                }

                // Add it to the category
                await newchannel.ModifyAsync(x =>
                {
                    x.CategoryId = categoryId;
                });
            }
            // Check if a position was provided
            if (Position != null)
            {
                // Update its position
                await newchannel.ModifyAsync(x =>
                {
                    x.Position = Position.Value;
                });
            }
        }

        /// <summary>
        /// Create a Text Channel using the name and roles provided
        /// </summary>
        private async Task CreateChannel(CommandContext Context, string Channel, [Optional]List<string> Roles, [Optional]string Description, [Optional]string Category, [Optional]int? Position)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetChannelsAsync();
            // Check if the channel exists
            foreach (var channelname in channels)
            {
                if (channelname.Name == Channel)
                {
                    // If the channel exists exit
                    return;
                }
            }
            Discord.ITextChannel newchannel = await Context.Guild.CreateTextChannelAsync(Channel);
            // Check if roles were passed
            if (Roles != null)
            {
                // Parse in the roles to add them to the channel
                foreach (string role in Roles)
                {
                    // Before we go any further let's see if the role already exists
                    // If the role exists exit the task
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name == role)
                        {
                            // Add the selected roles to the channel using inhert as its base
                            // ToDo: Allow for read only
                            OverwritePermissions inheret = new OverwritePermissions();
                            await newchannel.AddPermissionOverwriteAsync(existingrole, inheret);
                            break;
                        }
                    }
                }
                // Remove the everyone permission if it's not in the list
                if (Roles.Contains("Everyone") == false)
                {
                    foreach (Discord.IRole existingrole in Context.Guild.Roles)
                    {
                        // Compare the list of roles in the discord with the Role
                        if (existingrole.Name.ToLower() == "everyone")
                        {
                            // Remove Everyones permissions
                            await newchannel.RemovePermissionOverwriteAsync(existingrole);
                            break;
                        }
                    }
                }
            }
            // Check if a description was passed, if it was update the description
            if (Description != null)
            {
                // Modify the new channel created description
                await newchannel.ModifyAsync(x =>
                {
                    x.Topic = Description;
                });
            }
            // Check if a category was passed
            if (Category != null)
            {
                // Get the list of categories
                IReadOnlyCollection<IGuildChannel> categories = await Context.Guild.GetCategoriesAsync();
                ulong categoryId = 000000;
                // Check if the category name matches the id if it does return the ID
                foreach (var categoryname in categories)
                {
                    if (categoryname.Name == Category)
                    {
                        categoryId = categoryname.Id;
                        break;
                    }
                }

                // Add it to the category
                await newchannel.ModifyAsync(x =>
                {
                    x.CategoryId = categoryId;
                });
            }
            // Check if a position was provided
            if (Position != null)
            {
                // Update its position
                await newchannel.ModifyAsync(x =>
                {
                    x.Position = Position.Value;
                });
            }
        }

        /// <summary>
        /// Used for creating a discord role in the current guild
        /// Allowed Perms are Admin, Mod, Standard and Display
        /// </summary>
        private async Task CreateRole(CommandContext Context, string Role, string Perms, Color RoleColor, bool DisplayedRole)
        {
            // Before we go any further let's see if the role already exists
            // If the role exists exit the task
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name == Role)
                {
                    return;
                }
            }

            // ToDo: Set users Nickname as their site name and remove the users ability to change their nickname

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
            else if (Perms.ToLower() == "mod")
            {
                roleperms = modpermissions;
            }
            else if (Perms.ToLower() == "standard")
            {
                roleperms = standardpermissions;
            }
            else
            {
                // If it dosen't have the right passed flag just make it a display role
                roleperms = displayonlypermissions;
            }
            // ToDo: Set Role Position
            // Actually create the role using the provided settings
            await Context.Guild.CreateRoleAsync(Role, roleperms, RoleColor, DisplayedRole);
        }

    }
}
