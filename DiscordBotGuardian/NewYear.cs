using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DiscordBotGuardian
{
    public class NewYear
    {

        /// <summary>
        /// Used for generating a new set of roles
        /// </summary>
        public async Task GenerateRolesAsync(CommandContext Context, string Event, int Year)
        {
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
        }
        /// <summary>
        /// Used for generating a new set of Categories
        /// </summary>
        public async Task GenerateCategoriesAsync(CommandContext Context, string Event, int Year)
        {
            /// Create Global Perm set for all Guardians so we dont need to manually tack it on each time
            /// 
            // Generate list
            List<RolePermissions> GuardianEventsStandard = new List<RolePermissions>();
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name.Contains("Guardian-"))
                {
                    GuardianEventsStandard.Add(new RolePermissions { Role = existingrole.Name, ChannelPermType = RolePermissions.ChannelPermissions("standard") });
                }
            }
            // Add the event we are adding
            GuardianEventsStandard.Add(new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            // Add default roles
            GuardianEventsStandard.Add(new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") });
            GuardianEventsStandard.Add(new RolePermissions { Role = "Staff", ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            GuardianEventsStandard.Add(new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") });




            List<RolePermissions> GuardianEventsReadOnly = new List<RolePermissions>();
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name.Contains("Guardian-"))
                {
                    GuardianEventsReadOnly.Add(new RolePermissions { Role = existingrole.Name, ChannelPermType = RolePermissions.ChannelPermissions("readonly") });
                }
            }
            // Add the event we are adding
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") });
            // Add default roles
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") });
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Staff", ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") });

            /// Create Categories before we make channels
            /// 
            // Global Categories
            await CreateCategory(Context, "Administration", null, 1);
            await CreateCategory(Context, "Global", GuardianEventsStandard, 2);

            // Event Based Categories
            await CreateCategory(Context, Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Command-Center-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Staff", ChannelPermType = RolePermissions.ChannelPermissions("standard") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Team-Lead-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, Event + "-Commons-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Center-Stage-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Center-Stage-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Dispatch-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Dispatch-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Expo-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Expo-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Freelancer-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Freelancer-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "PA-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "PA-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Panels-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Panels-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Registration-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Registration-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Response-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Response-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Signatures-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Signatures-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Special-Rooms-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Special-Rooms-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Store-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Store-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Tech-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Tech-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });
            await CreateCategory(Context, "Happy-Hour-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Happy-Hour-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } });

        }

        /// <summary>
        /// Used for generating a new set of channels for a new year
        /// </summary>
        public async Task GenerateChannelsAsync(CommandContext Context, string Event, int Year)
        {
            /// Create Global Perm set for all Guardians so we dont need to manually tack it on each time
            /// 
            // Generate list
            List<RolePermissions> GuardianEventsStandard = new List<RolePermissions>();
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name.Contains("Guardian-"))
                {
                    GuardianEventsStandard.Add(new RolePermissions { Role = existingrole.Name, ChannelPermType = RolePermissions.ChannelPermissions("standard") });
                }
            }
            // Add the event we are adding
            GuardianEventsStandard.Add(new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            // Add default roles
            GuardianEventsStandard.Add(new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") });
            GuardianEventsStandard.Add(new RolePermissions { Role = "Staff", ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            GuardianEventsStandard.Add(new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") });




            List<RolePermissions> GuardianEventsReadOnly = new List<RolePermissions>();
            foreach (Discord.IRole existingrole in Context.Guild.Roles)
            {
                // Compare the list of roles in the discord with the Role
                if (existingrole.Name.Contains("Guardian-"))
                {
                    GuardianEventsReadOnly.Add(new RolePermissions { Role = existingrole.Name, ChannelPermType = RolePermissions.ChannelPermissions("readonly") });
                }
            }
            // Add the event we are adding
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") });
            // Add default roles
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") });
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Staff", ChannelPermType = RolePermissions.ChannelPermissions("standard") });
            GuardianEventsReadOnly.Add(new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") });


            /// Create Channels
            /// 
            // Administration Category Channels
            await CreateChannel(Context, "alerts", GuardianEventsReadOnly, "Announcements for everyone.", "Administration", 1);
            await CreateChannel(Context, "admins", new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") } }, "Admin Chat", "Administration", 2);
            await CreateChannel(Context, "mods", new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Mod", ChannelPermType = RolePermissions.ChannelPermissions("mod") } }, "Mod Chat", "Administration", 3);
            await CreateChannel(Context, "report-request-to-admins", GuardianEventsStandard, "Report any issues or requests here.", "Administration", 4);
            await CreateChannel(Context, "rulebook", new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Everyone", ChannelPermType = RolePermissions.ChannelPermissions("readonly") } }, "Announcements for everyone.", "Administration", 5);
            await CreateChannel(Context, "landing", null, "Limbo where new users wait for roles to be assigned.", "Administration", 6); // ToDo: No History everyone

            // Global Category Channels
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
            await CreateChannel(Context, "command-chat-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Team-Lead-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "HGs and TLs", "Command-Center-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "crisis-center-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Crisis-Management-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Direct line: HGs and Response TLs.", "Command-Center-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "announcements-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") } }, "Announcements for RTX", "Command-Center-" + Event + "-" + Year, 3);
            await CreateChannel(Context, "links-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") } }, "Important links you should probably bookmark.", "Command-Center-" + Event + "-" + Year, 4);


            // Commons
            await CreateChannel(Context, "guardian-bar-" + Event + "-" + Year, null, "There will always be a Guardian Bar", Event + "-Commons-" + Year, 1);
            await CreateChannel(Context, "ride-room-share-" + Event + "-" + Year, null, "Because no one uses the forum anymore.", Event + "-Commons-" + Year, 2);
            await CreateChannel(Context, "meetups-events-" + Event + "-" + Year, null, "It's like matchmaking, but in real life.", Event + "-Commons-" + Year, 3);
            await CreateChannel(Context, "town-hall-" + Event + "-" + Year, null, "Ask the HGs anything appropriate.", Event + "-Commons-" + Year, 4);
            await CreateVoiceChannel(Context, Event + "-vc-1", null, Event + "-Commons-" + Year, 5);
            await CreateVoiceChannel(Context, Event + "-vc-2", null, Event + "-Commons-" + Year, 6);
            await CreateVoiceChannel(Context, "town-hall-vc-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Head-Guardian", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Guardian-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") } }, Event + "-Commons-" + Year, 7);

            // Team Chats
            // Center-Stage
            await CreateChannel(Context, "leads-cs-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-CS-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Leads chat", "Center-Stage-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "general-cs-" + Event + "-" + Year, null, "Team Chat", "Center-Stage-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "ops-cs-" + Event + "-" + Year, null, "Team Operations", "Center-Stage-" + Event + "-" + Year, 3);
            await CreateVoiceChannel(Context, "leads-vc-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-CS-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Center-Stage-" + Event + "-" + Year, 4);
            await CreateVoiceChannel(Context, "general-cs-" + Event + "-" + Year, null, "Center-Stage-" + Event + "-" + Year, 5);

            // Dispatch
            await CreateChannel(Context, "general-dp-" + Event + "-" + Year, null, "Team Chat", "Dispatch-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-dp-" + Event + "-" + Year, null, "Team Operations", "Dispatch-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-dp-" + Event + "-" + Year, null, "Dispatch-" + Event + "-" + Year, 3);

            // Expo
            await CreateChannel(Context, "general-ex-" + Event + "-" + Year, null, "Team Chat", "Expo-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-ex-" + Event + "-" + Year, null, "Squad Chat", "Expo-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-ex-" + Event + "-" + Year, null, "Expo-" + Event + "-" + Year, 3);

            // Freelancer
            await CreateChannel(Context, "general-fl-" + Event + "-" + Year, null, "Team Chat", "Freelancer-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-fl-" + Event + "-" + Year, null, "Team Operations", "Freelancer-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-fl-" + Event + "-" + Year, null, "Freelancer-" + Event + "-" + Year, 3);

            // PA
            await CreateChannel(Context, "general-pa-" + Event + "-" + Year, null, "Team Chat", "PA-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-pa-" + Event + "-" + Year, null, "Team Operations", "PA-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "response-pa-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "PAL-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Direct line to Response Leads", "PA-" + Event + "-" + Year, 3);
            await CreateVoiceChannel(Context, "general-pa-" + Event + "-" + Year, null, "PA-" + Event + "-" + Year, 4);

            // Panels
            await CreateChannel(Context, "general-pl-" + Event + "-" + Year, null, "Team Chat", "Panels-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-pl-" + Event + "-" + Year, null, "Squad Chat", "Panels-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-pl-" + Event + "-" + Year, null, "Panels-" + Event + "-" + Year, 3);

            // Registration
            await CreateChannel(Context, "leads-rg-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-RG-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Leads chat", "Registration-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "general-rg-" + Event + "-" + Year, null, "Team Chat", "Registration-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "ops-rg-" + Event + "-" + Year, null, "Team Operations", "Registration-" + Event + "-" + Year, 3);
            await CreateVoiceChannel(Context, "leads-rg-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-RG-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Registration-" + Event + "-" + Year, 4);
            await CreateVoiceChannel(Context, "general-rg-" + Event + "-" + Year, null, "Registration-" + Event + "-" + Year, 5);

            // Response
            await CreateChannel(Context, "leads-rs-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-RS-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Leads chat", "Response-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "general-rs-" + Event + "-" + Year, null, "Team Chat", "Response-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "file-share-rs-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-RS-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") }, new RolePermissions { Role = "Reponse-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("readonly") } }, "File share for Response team.", "Response-" + Event + "-" + Year, 3);
            await CreateChannel(Context, "alpha-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 4);
            await CreateChannel(Context, "bravo-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 5);
            await CreateChannel(Context, "charlie-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 6);
            await CreateChannel(Context, "delta-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 7);
            await CreateChannel(Context, "echo-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 8);
            await CreateChannel(Context, "foxtrot-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 9);
            await CreateChannel(Context, "golf-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 10);
            await CreateChannel(Context, "hotel-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 11);
            await CreateChannel(Context, "india-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 12);
            await CreateChannel(Context, "juliet-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 13);
            await CreateChannel(Context, "kilo-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 14);
            await CreateChannel(Context, "lima-rs-" + Event + "-" + Year, null, "Squad Operations", "Response-" + Event + "-" + Year, 15);
            await CreateVoiceChannel(Context, "leads-rs-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-RS-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Response-" + Event + "-" + Year, 16);
            await CreateVoiceChannel(Context, "general-rs-" + Event + "-" + Year, null, "Response-" + Event + "-" + Year, 17);

            //Signatures
            await CreateChannel(Context, "leads-sg-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-sg-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Leads chat", "Signatures-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "general-sg-" + Event + "-" + Year, null, "Team Chat", "Signatures-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "alpha-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 3);
            await CreateChannel(Context, "bravo-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 4);
            await CreateChannel(Context, "charlie-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 5);
            await CreateChannel(Context, "delta-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 6);
            await CreateChannel(Context, "echo-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 7);
            await CreateChannel(Context, "foxtrot-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 8);
            await CreateChannel(Context, "golf-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 9);
            await CreateChannel(Context, "hotel-sg-" + Event + "-" + Year, null, "Squad Operations", "Signatures-" + Event + "-" + Year, 10);
            await CreateVoiceChannel(Context, "leads-sg-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-sg-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Signatures-" + Event + "-" + Year, 11);
            await CreateVoiceChannel(Context, "general-sg-" + Event + "-" + Year, null, "Signatures-" + Event + "-" + Year, 12);

            // Special Rooms
            await CreateChannel(Context, "general-sr-" + Event + "-" + Year, null, "Team Chat", "Special-Rooms-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-sr-" + Event + "-" + Year, null, "Squad Chat", "Special-Rooms-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-sr-" + Event + "-" + Year, null, "Special-Rooms-" + Event + "-" + Year, 3);

            // Store
            await CreateChannel(Context, "general-st-" + Event + "-" + Year, null, "Team Chat", "Store-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-st-" + Event + "-" + Year, null, "Squad Chat", "Store-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-st-" + Event + "-" + Year, null, "Store-" + Event + "-" + Year, 3);

            // Tech
            await CreateChannel(Context, "general-th-" + Event + "-" + Year, null, "Team Chat", "Tech-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "ops-th-" + Event + "-" + Year, null, "Squad Chat", "Tech-" + Event + "-" + Year, 2);
            await CreateVoiceChannel(Context, "general-th-" + Event + "-" + Year, null, "Tech-" + Event + "-" + Year, 3);

            // Happy Hour
            await CreateChannel(Context, "leads-hh-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-hh-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Leads chat", "Happy-Hour-" + Event + "-" + Year, 1);
            await CreateChannel(Context, "general-hh-" + Event + "-" + Year, null, "Team Chat", "Happy-Hour-" + Event + "-" + Year, 2);
            await CreateChannel(Context, "ops-hh-" + Event + "-" + Year, null, "Team Operations", "Happy-Hour-" + Event + "-" + Year, 3);
            await CreateVoiceChannel(Context, "leads-hh-" + Event + "-" + Year, new List<RolePermissions> { new RolePermissions { Role = "Admin", ChannelPermType = RolePermissions.ChannelPermissions("admin") }, new RolePermissions { Role = "Lead-hh-" + Event + "-" + Year, ChannelPermType = RolePermissions.ChannelPermissions("standard") } }, "Happy-Hour-" + Event + "-" + Year, 4);
            await CreateVoiceChannel(Context, "general-hh-" + Event + "-" + Year, null, "Happy-Hour-" + Event + "-" + Year, 5);

            /// Messages in the rulebook page
            // Clear out old TOS
            await DeleteLastMessage(Context, "rulebook");
            // Post new TOS
            await SendTOSMessage(Context, "rulebook");
        }

        /// <summary>
        /// Used for deleting the last sent message in a channel (Only used for Rulebook currently)
        /// </summary>
        private async Task DeleteLastMessage(CommandContext Context, string Channel)
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
        /// <summary>
        /// Send the TOS message to a channel
        /// </summary>
        private async Task SendTOSMessage(CommandContext Context, string Channel)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetChannelsAsync();
            // Check if the channel exists
            foreach (var channelname in channels)
            {
                // Make sure we are in the right channel
                if (channelname.Name.ToString().ToLower().Trim() == Channel.ToString().ToLower().Trim())
                {
                    var message = channelname as IMessageChannel;
                    // Send the TOS
                    await message.SendMessageAsync(@"Welcome to the official Guardian Discord! Here, you will find the means to communicate with your fellow Guardians. 
Please note the structure of this Discord as channels may have changed from previous years. 

Administration is where everyone starts. Here, we have our rulebook that everyone must follow and acknowledge to be a part of this super Discord. 

Global is where you can fly across the pond and chat with Guardians from other events (RTX London, RTX Austin, etc.). Most of the social channels live here as well. 

Region Locked are the channels for each specific RTX event. Each team will have their own version of the following:

Command-Center: Announcements and links for your RTX Event.

Commons: Your RTX Event’s shared channels.

Team Channels: Each team has their own set of channels that are specific to each team’s needs.

You’ll find voice channels in Global, Commons, and your Team category.
And now for The Rules:
1. The Rooster Teeth Code of Conduct applies before, during, and after RTX- in both real life and online. If you have not read it, please do so right now. It is the standard by which this community holds its members; it is how we expect Guardians to comport themselves. https://docs.google.com/document/d/e/2PACX-1vQbrMCVptCC5-c_LbdheGUitvSvXsKo1W0EFkCSvfoue7qlW_Xdm2Po1QpXRm4kO_Z85eL00PePgOyW/pub
2.  Follow all normal Guardian rules. Seriously. If you need a refresher on what Guardian rules are, here is the short version - BE NICE - Post like you would on RT.
3.  When in voice chat, please use push-to-talk.
4.  Do not share this Discord with non-Guardians. This is our private fun place.
5.  Some Teams have Squad chats. Please use your assigned squad chat only. Do not go into other squad chats.

Once you understand these rules, go back to #landing and type '!rt RT - Site - Username' to be given access to all of the fun. Thank you for being an important part of the RTX experience!
-------------------");
                }
            }
            return;
        }

        /// <summary>
        /// Create a Category using the channel using roles provided
        /// </summary>
        private async Task CreateCategory(CommandContext Context, string Category, [Optional]List<RolePermissions> Roles, [Optional]int? Position)
        {
            // Get the list of Categories
            IReadOnlyCollection<IGuildChannel> categories = await Context.Guild.GetCategoriesAsync();
            // Check if the Category exists
            bool exists = false;
            ICategoryChannel newcategory = null;
            foreach (var categoryname in categories)
            {
                if (categoryname.Name.ToString().ToLower().Trim() == Category.ToString().ToLower().Trim())
                {
                    // If the channel exists exit
                    newcategory = categoryname as ICategoryChannel;
                    exists = true;
                    break;
                }
            }
            // Create the Category
            if (exists == false)
            {
                newcategory = await Context.Guild.CreateCategoryAsync(Category);
            }

            // Wait for Category to Generate
            await Task.Delay(1000);
            if (newcategory != null)
            {
                // Check if we are passing roles
                if (Roles != null)
                {
                    // Parse in the roles to add them to the channel
                    foreach (RolePermissions role in Roles)
                    {
                        // Before we go any further let's see if the role already exists
                        // If the role exists exit the task
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower().Trim() == role.Role.ToLower().Trim())
                            {
                                // Add the selected roles to the channel using inhert as its base
                                OverwritePermissions inheret = new OverwritePermissions();
                                await newcategory.AddPermissionOverwriteAsync(existingrole, inheret);
                                break;
                            }
                        }
                    }
                    // Remove the everyone permission if it's not in the list
                    bool permfound = false;
                    foreach (RolePermissions perm in Roles)
                    {
                        if (perm.Role.ToLower().Contains("everyone") == true)
                        {
                            permfound = true;
                            break;
                        }
                    }
                    if (permfound == false)
                    {
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower() == "@everyone")
                            {
                                OverwritePermissions denypermissions = new OverwritePermissions(createInstantInvite: PermValue.Deny, manageChannel: PermValue.Deny, addReactions: PermValue.Deny, viewChannel: PermValue.Deny, sendMessages: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageMessages: PermValue.Deny, embedLinks: PermValue.Deny, attachFiles: PermValue.Deny, readMessageHistory: PermValue.Deny, mentionEveryone: PermValue.Deny, useExternalEmojis: PermValue.Deny, connect: PermValue.Deny, speak: PermValue.Deny, muteMembers: PermValue.Deny, deafenMembers: PermValue.Deny, moveMembers: PermValue.Deny, useVoiceActivation: PermValue.Deny, manageRoles: PermValue.Deny, manageWebhooks: PermValue.Deny);
                                // Remove Everyones permissions
                                await newcategory.AddPermissionOverwriteAsync(existingrole, denypermissions);
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
        }

        /// <summary>
        /// Create a Voice Channel using the name and roles provided
        /// </summary>
        private async Task CreateVoiceChannel(CommandContext Context, string VoChannel, [Optional]List<RolePermissions> Roles, [Optional]string Category, [Optional]int? Position)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetVoiceChannelsAsync();
            // Check if the channel exists
            bool exists = false;
            Discord.IVoiceChannel newchannel = null;
            foreach (var channelname in channels)
            {
                if (channelname.Name.ToString().ToLower().Trim() == VoChannel.ToString().ToLower().Trim())
                {
                    // If the channel exists exit
                    newchannel = channelname as IVoiceChannel;
                    exists = true;
                    break;
                }
            }
            if (exists == false)
            {
                newchannel = await Context.Guild.CreateVoiceChannelAsync(VoChannel);
            }

            // Wait for VO to Generate
            await Task.Delay(1000);
            if (newchannel != null)
            {
                // Check if roles were passed
                if (Roles != null)
                {
                    // Parse in the roles to add them to the channel
                    foreach (RolePermissions role in Roles)
                    {
                        // Before we go any further let's see if the role already exists
                        // If the role exists exit the task
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower().Trim() == role.Role.ToLower().Trim())
                            {
                                // Add the selected roles to the channel using inhert as its base
                                await newchannel.AddPermissionOverwriteAsync(existingrole, role.ChannelPermType);
                                break;
                            }
                        }
                    }
                    // Remove the everyone permission if it's not in the list
                    bool permfound = false;
                    foreach (RolePermissions perm in Roles)
                    {
                        if (perm.Role.ToLower().Contains("everyone") == true)
                        {
                            permfound = true;
                            break;
                        }
                    }
                    if (permfound == false)
                    {
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower() == "@everyone")
                            {
                                OverwritePermissions denypermissions = new OverwritePermissions(createInstantInvite: PermValue.Deny, manageChannel: PermValue.Deny, addReactions: PermValue.Deny, viewChannel: PermValue.Deny, sendMessages: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageMessages: PermValue.Deny, embedLinks: PermValue.Deny, attachFiles: PermValue.Deny, readMessageHistory: PermValue.Deny, mentionEveryone: PermValue.Deny, useExternalEmojis: PermValue.Deny, connect: PermValue.Deny, speak: PermValue.Deny, muteMembers: PermValue.Deny, deafenMembers: PermValue.Deny, moveMembers: PermValue.Deny, useVoiceActivation: PermValue.Deny, manageRoles: PermValue.Deny, manageWebhooks: PermValue.Deny);
                                // Remove Everyones permissions
                                await newchannel.AddPermissionOverwriteAsync(existingrole, denypermissions);
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
                        if (categoryname.Name.ToLower().Trim() == Category.ToLower().Trim())
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
                if (Roles == null)
                {
                    await newchannel.SyncPermissionsAsync();
                }
            }
        }

        /// <summary>
        /// Create a Text Channel using the name and roles provided
        /// </summary>
        private async Task CreateChannel(CommandContext Context, string Channel, [Optional]List<RolePermissions> Roles, [Optional]string Description, [Optional]string Category, [Optional]int? Position)
        {
            // Get the list of channels
            IReadOnlyCollection<IGuildChannel> channels = await Context.Guild.GetChannelsAsync();
            // Check if the channel exists
            bool exists = false;
            Discord.ITextChannel newchannel = null;
            foreach (var channelname in channels)
            {
                if (channelname.Name.ToString().ToLower().Trim() == Channel.ToString().ToLower().Trim())
                {
                    // If the channel exists exit
                    exists = true;
                    newchannel = channelname as ITextChannel;
                    break;
                }
            }
            if (exists == false)
            {
                newchannel = await Context.Guild.CreateTextChannelAsync(Channel);
            }

            // Wait for Channel to Generate
            await Task.Delay(1000);
            if (newchannel != null)
            {
                // Check if roles were passed
                if (Roles != null)
                {
                    // Parse in the roles to add them to the channel
                    foreach (RolePermissions role in Roles)
                    {
                        // Before we go any further let's see if the role already exists
                        // If the role exists exit the task
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower().Trim() == role.Role.ToLower().Trim())
                            {
                                // Add the selected roles to the channel using inhert as its base
                                await newchannel.AddPermissionOverwriteAsync(existingrole, role.ChannelPermType);
                                break;
                            }
                        }
                    }
                    // Remove the everyone permission if it's not in the list
                    bool permfound = false;
                    foreach (RolePermissions perm in Roles)
                    {
                        if (perm.Role.ToLower().Contains("everyone") == true)
                        {
                            permfound = true;
                            break;
                        }
                    }
                    if (permfound == false)
                    {
                        foreach (Discord.IRole existingrole in Context.Guild.Roles)
                        {
                            // Compare the list of roles in the discord with the Role
                            if (existingrole.Name.ToLower() == "@everyone")
                            {
                                OverwritePermissions denypermissions = new OverwritePermissions(createInstantInvite: PermValue.Deny, manageChannel: PermValue.Deny, addReactions: PermValue.Deny, viewChannel: PermValue.Deny, sendMessages: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageMessages: PermValue.Deny, embedLinks: PermValue.Deny, attachFiles: PermValue.Deny, readMessageHistory: PermValue.Deny, mentionEveryone: PermValue.Deny, useExternalEmojis: PermValue.Deny, connect: PermValue.Deny, speak: PermValue.Deny, muteMembers: PermValue.Deny, deafenMembers: PermValue.Deny, moveMembers: PermValue.Deny, useVoiceActivation: PermValue.Deny, manageRoles: PermValue.Deny, manageWebhooks: PermValue.Deny);
                                // Remove Everyones permissions
                                await newchannel.AddPermissionOverwriteAsync(existingrole, denypermissions);
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
                        if (categoryname.Name.ToLower().Trim() == Category.ToLower().Trim())
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
                if (Roles == null)
                {
                    await newchannel.SyncPermissionsAsync();
                }
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
                if (existingrole.Name.ToLower().Trim() == Role.ToLower().Trim())
                {
                    return;
                }
            }

            GuildPermissions roleperms = RolePermissions.GuildPermissions(Perms.ToLower().Trim());

            // ToDo: Set Role Position
            // Actually create the role using the provided settings
            await Context.Guild.CreateRoleAsync(Role, roleperms, RoleColor, DisplayedRole);

            // Pause after role creation
            await Task.Delay(200);
        }

    }
}
