namespace DiscordBotGuardian
{
    /// <summary>
    ///  Used for loading in creds from the credentials file
    /// </summary>
    internal class Credentials
    {

        /// <summary>
        /// The spreadsheet ID (In the URL)
        /// </summary>
        public string SpreadSheetID { get; set; }

        /// <summary>
        /// The spreadsheet page name (The Tab)
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// The email server you are hitting for sending texts
        /// </summary>
        public string SMTPEndpoint { get; set; }

        /// <summary>
        /// The username you are using to log into the email server
        /// </summary>
        public string SMTPUsername { get; set; }
    
        /// <summary>
        /// The password you are using to log into the email server
        /// </summary>
        public string SMTPPassword { get; set; }

        /// <summary>
        /// The actual email account you are using for the account (Likely the same as Username)
        /// </summary>
        public string SMTPEmail { get; set; }

        /// <summary>
        /// The Discord bot token you get https://discordapp.com/developers/applications/
        /// </summary>
        public string BotToken { get; set; }

    }

}
