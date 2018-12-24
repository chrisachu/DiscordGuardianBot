using System.Collections.Generic;

namespace DiscordBotGuardian
{
    /// <summary>
    /// Used for storing the data retrived out of the Google Doc
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Setting the Team that the user is on
        /// </summary>
        public List<string> Team { get; set; }

        /// <summary>
        /// Setting their RT Username
        /// </summary>
        public string RTUsername { get; set; }

        /// <summary>
        /// Setting their phone endpoint (sms.json)
        /// </summary>
        public string PhoneEndpoint { get; set; }

        /// <summary>
        /// Determines if they have authenticated and accepted the TOS
        /// </summary>
        public bool Authenticated { get; set; }

        /// <summary>
        /// Used for assigning their discord username
        /// </summary>
        public string DiscordUsername { get; set; }

        /// <summary>
        /// Used for holding the list of roles the user has assigned
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// Bool to check if they have SMS disabled or enabled
        /// </summary>
        public bool SMS { get; set; }

        /// <summary>
        /// List of channels they have set for 
        /// </summary>
        public List<string> Channels { get; set; }

        public List<string> Event { get; set; }
    }

}
