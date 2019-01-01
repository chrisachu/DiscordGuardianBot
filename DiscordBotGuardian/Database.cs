using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace DiscordBotGuardian
{
    class Database
    {
        // ToDo: Comments for The DB Class
        public static string range = "";
        public static string sheetname = "";
        public static string spreadsheetId = "";
        private static void LoadCreds()
        {
            string curDir = Directory.GetCurrentDirectory();
            string credsfile = File.ReadAllText(curDir + "/botsettings.json");
            Credentials creds = JsonConvert.DeserializeObject<Credentials>(credsfile);
            range = creds.SheetName + "!A:I";
            sheetname = creds.SheetName;
            spreadsheetId = creds.SpreadSheetID;
        }
        private static SheetsService Logintosheets()
        {
            LoadCreds();
            UserCredential credential;
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "GuardianBot-Discord";
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            SheetsService service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
        public static List<UserData> ReadDB(List<UserData> users)
        {
            lock (users)
            {
                SheetsService service = Logintosheets();
                List<UserData> newuser = new List<UserData>();
                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    int newuserrow = 0;
                    int userrow = 0;
                    bool headers = false;
                    int rtusername = 0;
                    int team = 0;
                    int phoneendpoint = 0;
                    int authenticated = 0;
                    int discordusername = 0;
                    int roles = 0;
                    int sms = 0;
                    int channels = 0;
                    int eventname = 0;
                    foreach (var row in values)
                    {
                        if (headers == true)
                        {
                            if (users.ElementAtOrDefault(userrow) != null)
                            {
                                if (row[rtusername].ToString() != "NULL")
                                {
                                    users[userrow].RTUsername = row[rtusername].ToString();
                                }
                                if (row[team].ToString() != "NULL")
                                {
                                    users[userrow].Team = row[team].ToString().Split(',').ToList();
                                }
                                if (row[phoneendpoint].ToString() != "NULL")
                                {
                                    users[userrow].PhoneEndpoint = row[phoneendpoint].ToString();
                                }
                                if (row[authenticated].ToString() != "NULL")
                                {
                                    users[userrow].Authenticated = Convert.ToBoolean(row[authenticated].ToString());
                                }
                                if (row[discordusername].ToString() != "NULL")
                                {
                                    users[userrow].DiscordUsername = row[discordusername].ToString();
                                }
                                if (row[roles].ToString() != "NULL")
                                {
                                    users[userrow].Roles = row[roles].ToString().Split(',').ToList();
                                }
                                if (row[sms].ToString() != "NULL")
                                {
                                    users[userrow].SMS = Convert.ToBoolean(row[sms].ToString());
                                }
                                if (row[channels].ToString() != "NULL")
                                {
                                    users[userrow].Channels = row[channels].ToString().Split(',').ToList();
                                }
                                if (row[eventname].ToString() != "NULL")
                                {
                                    users[userrow].Event = row[eventname].ToString().Split(',').ToList();
                                }
                            }
                            else
                            {
                                if (newuser.ElementAtOrDefault(newuserrow) == null)
                                {
                                    UserData tempnewuser = new UserData();
                                    newuser.Add(tempnewuser);
                                }
                                if (row[rtusername].ToString() != "NULL")
                                {
                                    newuser[newuserrow].RTUsername = row[rtusername].ToString();
                                }
                                if (row[team].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Team = row[team].ToString().Split(',').ToList();
                                }
                                if (row[phoneendpoint].ToString() != "NULL")
                                {
                                    newuser[newuserrow].PhoneEndpoint = row[phoneendpoint].ToString();
                                }
                                if (row[authenticated].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Authenticated = Convert.ToBoolean(row[authenticated].ToString());
                                }
                                if (row[discordusername].ToString() != "NULL")
                                {
                                    newuser[newuserrow].DiscordUsername = row[discordusername].ToString();
                                }
                                if (row[roles].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Roles = row[roles].ToString().Split(',').ToList();
                                }
                                if (row[sms].ToString() != "NULL")
                                {
                                    newuser[newuserrow].SMS = Convert.ToBoolean(row[sms].ToString());
                                }
                                if (row[channels].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Channels = row[channels].ToString().Split(',').ToList();
                                }
                                if (row[eventname].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Event = row[eventname].ToString().Split(',').ToList();
                                }
                                newuserrow++;
                            }
                            userrow++;
                        }
                        else
                        {
                            int counter = 0;
                            foreach(var headerrow in row)
                            { 
                                if (headerrow.ToString().ToLower().Trim() == "rtusername")
                                {
                                    rtusername = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "team")
                                {
                                    team = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "phoneendpoint")
                                {
                                    phoneendpoint = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "authenticated")
                                {
                                    authenticated = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "discordusername")
                                {
                                    discordusername = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "roles")
                                {
                                    roles = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "sms")
                                {
                                    sms = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "channels")
                                {
                                    channels = counter;
                                }
                                else if (headerrow.ToString().ToLower().Trim() == "event")
                                {
                                    eventname = counter;
                                }
                                counter++;
                            }                            
                            headers = true;
                        }
                    }
                }
                if (newuser.Count > 0)
                {
                    foreach (UserData user in newuser)
                    {
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public static List<UserData> UpdateUser(string user, string type, string value,List<UserData> users, [Optional] List<string> list)
        {
            bool updated = false;
            UserData userinfo = new UserData();
            foreach (UserData person in users)
            {
                try
                {
                    if (type == "DiscordUsername")
                    {
                        if (person.RTUsername.ToLower() == user.ToLower())
                        {
                            person.DiscordUsername = value;
                            break;
                        }
                    }
                    else
                    {
                        if (person.DiscordUsername != null)
                        {
                            if (person.DiscordUsername.ToLower() == user)
                            {
                                if (type == "PhoneEndpoint")
                                {
                                    person.PhoneEndpoint = value;
                                }
                                else if (type == "Authenticated")
                                {
                                    person.Authenticated = Convert.ToBoolean(value);
                                }
                                else if (type == "Roles")
                                {
                                    person.Roles = list;
                                }
                                else if (type == "SMS")
                                {
                                    person.SMS = Convert.ToBoolean(value);
                                }
                                else if (type == "Channels")
                                {
                                    person.Channels = list;
                                }
                                else if (type == "Event")
                                {
                                    person.Event = list;
                                }
                                else if (type == "Team")
                                {
                                    person.Team = list;
                                }
                                updated = true;
                                userinfo = person;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            if (updated == true)
            {
                WriteDB(userinfo);
            }
            return users;
        }

        public static bool WriteDB(UserData user)
        {
            try
            {
                SheetsService service = Logintosheets();
                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                int counter = 0;
                int rtusernamec = 0;
                int teamc = 0;
                int phoneendpointc = 0;
                int authenticatedc = 0;
                int discordusernamec = 0;
                int rolesc = 0;
                int smsc = 0;
                int channelsc = 0;
                int eventnamec = 0;

                int rownumber = 1;
 
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        foreach (var headerrow in row)
                        {
                            if (headerrow.ToString().ToLower().Trim() == "rtusername")
                            {
                                rtusernamec = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "team")
                            {
                                teamc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "phoneendpoint")
                            {
                                phoneendpointc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "authenticated")
                            {
                                authenticatedc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "discordusername")
                            {
                                discordusernamec = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "roles")
                            {
                                rolesc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "sms")
                            {
                                smsc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "channels")
                            {
                                channelsc = counter;
                            }
                            else if (headerrow.ToString().ToLower().Trim() == "event")
                            {
                                eventnamec = counter;
                            }
                            counter++;
                        }
                    }
                    foreach (var row in values)
                    {
                        if (row[rtusernamec].ToString() == user.RTUsername)
                        {
                            break;
                        }
                        else
                        {
                            rownumber++;
                        }
                    }
                }

                String range2 = sheetname + "!A" + rownumber.ToString();

                ValueRange requestBody = new ValueRange();

                SpreadsheetsResource.ValuesResource.UpdateRequest request2 = service.Spreadsheets.Values.Update(requestBody, spreadsheetId, range2);
                request2.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                string team;
                if (user.Team == null)
                {
                    team = "NULL";
                }
                else
                {
                    team = string.Join(",", user.Team);
                }
                string phoneendpoint;
                if (user.PhoneEndpoint == null)
                {
                    phoneendpoint = "NULL";
                }
                else
                {
                    phoneendpoint = user.PhoneEndpoint;
                }
                string roles;
                if (user.Roles == null)
                {
                    roles = "NULL";
                }
                else
                {
                    roles = string.Join(",", user.Roles);
                }
                string channels;
                if (user.Channels == null || user.Channels.Count == 0)
                {
                    channels = "NULL";
                }
                else
                {
                    channels = string.Join(",", user.Channels);
                }
                string events;
                if (user.Event == null || user.Event.Count == 0)
                {
                    events = "NULL";
                }
                else
                {
                    events = string.Join(",", user.Event);
                }

                Dictionary<int, string> array = new Dictionary<int, string>
                {
                    [rtusernamec] = user.RTUsername,
                    [teamc] = team,
                    [phoneendpointc] = phoneendpoint,
                    [authenticatedc] = user.Authenticated.ToString(),
                    [discordusernamec] = user.DiscordUsername,
                    [rolesc] = roles,
                    [smsc] = user.SMS.ToString(),
                    [channelsc] = channels,
                    [eventnamec] = events
                };
                var oblist = new List<object>() { array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7], array[8] };
                requestBody.Values = new List<IList<object>> { oblist };
                UpdateValuesResponse response2 = request2.Execute();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
