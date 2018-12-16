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

        public static string ParseCarrierName(string message)
        {
            List<SMS> providers = LoadSMSData();
            RegionInfo region = new RegionInfo(message);
            foreach (SMS carrier in providers)
            {
                if (region.EnglishName == carrier.Country)
                {
                    return region.EnglishName;
                }
            }
            return "";
        }
    }
}
