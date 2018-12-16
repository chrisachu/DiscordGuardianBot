using Newtonsoft.Json;

namespace DiscordBotGuardian
{
    /// <summary>
    /// Used for loading in sms.json https://email2sms.info/
    /// </summary>
    public class SMS
    {
        /// <summary>
        /// Used for loading in the Country
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Used for loading in the Region
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Used for loading in the Carrier
        /// </summary>
        [JsonProperty("carrier")]
        public string Carrier { get; set; }

        /// <summary>
        /// Used for loading in the endpoint for SMS
        /// </summary>
        [JsonProperty("email-to-sms")]
        public string EmailToSms { get; set; }

        /// <summary>
        /// Used for loading in the endpoint for MMS
        /// </summary>
        [JsonProperty("email-to-mms")]
        public string EmailToMms { get; set; }

        /// <summary>
        /// Used for loading in any notes
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

}
