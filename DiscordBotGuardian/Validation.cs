using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DiscordBotGuardian
{
    /// <summary>
    /// Methods commonly used for validation of data
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// Pass the string of a phone number and make sure its numbers only
        /// </summary>
        public static bool Isphonenumber(string message)
        {
            // Check each letter to see if its a number or not
            foreach (char c in message)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Load the SMS.json file with the info from sms2email.info
        /// </summary>
        public static List<SMS> LoadSMSData()
        {
            // Get the directory and load the text
            string curDir = Directory.GetCurrentDirectory();
            string json = File.ReadAllText(curDir + "/sms.json");
            // Deserialize the info and return the list
            List<SMS> providers = JsonConvert.DeserializeObject<List<SMS>>(json);
            return providers;
        }

        /// <summary>
        /// Passing the carrier name along with the provider data to see if its a valid provider
        /// </summary>
        public static bool Isvalidcarrier(string message)
        {
            List<SMS> providers = LoadSMSData();
            // Check each carrier in the providers
            foreach (SMS carrier in providers)
            {
                // If the message matches the carrier name return true
                if (message.ToLower() == carrier.Carrier.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Pass the region name along with the provider data to see if its valid
        /// </summary>
        public static bool Isvalidregion(string message)
        {
            List<SMS> providers = LoadSMSData();
            try
            {
                // if the message is less than 3 characters its short code
                if (message.Length <= 3)
                {
                    // Parse the region based off the short code message
                    RegionInfo region = new RegionInfo(message);
                    foreach (SMS carrier in providers)
                    {
                        if (region.EnglishName == carrier.Country)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            // If it was not short code, check it against a full list of country names to see if it matches
            foreach (SMS carrier in providers)
            {
                if (message.ToLower() == carrier.Country.ToLower())
                {
                    return true;
                }

            }
            return false;
        }
        /// <summary>
        /// Check if user has been authenticated yet
        /// </summary>
        public static bool IsUserAuthenticated(string user, List<UserData> users)
        {
            foreach (UserData person in users)
            {
                if (person.DiscordUsername != null)
                {
                    if (user.ToLower() == person.DiscordUsername.ToLower() && person.Authenticated == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Parse the carriers name to its full english name
        /// </summary>
        public static string ParseCarrierName(string message)
        {
            // Load the Provider DB
            List<SMS> providers = LoadSMSData();
            // Read the message into RegionInfo
            RegionInfo region = new RegionInfo(message);
            foreach (SMS carrier in providers)
            {
                // Check if the Name matches the Carrier Country
                if (region.EnglishName == carrier.Country)
                {
                    // Return the correct name
                    return region.EnglishName;
                }
            }
            return "";
        }
        /// <summary>
        /// Check against the list of roles in the Server if the user is a HG or Admin
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> IsHGorAdminAsync(CommandContext Context, SocketMessage User)
        {
            try
            {
                // Get the users info
                var userinfo = await Context.Guild.GetUserAsync(User.Author.Id) as IGuildUser;
                // Check all the roles in the discord
                foreach (var role in Context.Guild.Roles)
                {
                    // If the role matches either option
                    if (role.Name.ToLower().Trim() == "head-guardian" || role.Name.ToLower().Trim() == "admin")
                    {
                        // Check the role against all of the ID's the user has
                        foreach (var userrole in userinfo.RoleIds)
                        {
                            // If it matches return true
                            if (role.Id == userrole)
                            {
                                return true;
                            }
                        }
                    }
                }
                // Else just return false
                return false;
            }
            catch { return false; }
        }
    }
}
