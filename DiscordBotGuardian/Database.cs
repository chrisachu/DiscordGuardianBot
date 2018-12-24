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
                    foreach (var row in values)
                    {
                        if (headers == true)
                        {
                            // ToDo: Change the way we read the DB so we read it by header name rather than required row order
                            if (users.ElementAtOrDefault(userrow) != null)
                            {
                                if (row[0].ToString() != "NULL")
                                {
                                    users[userrow].RTUsername = row[0].ToString();
                                }
                                if (row[1].ToString() != "NULL")
                                {
                                    users[userrow].Team = row[1].ToString();
                                }
                                if (row[2].ToString() != "NULL")
                                {
                                    users[userrow].PhoneEndpoint = row[2].ToString();
                                }
                                if (row[3].ToString() != "NULL")
                                {
                                    users[userrow].Authenticated = Convert.ToBoolean(row[3].ToString());
                                }
                                if (row[4].ToString() != "NULL")
                                {
                                    users[userrow].DiscordUsername = row[4].ToString();
                                }
                                if (row[5].ToString() != "NULL")
                                {
                                    users[userrow].Roles = row[5].ToString().Split(',').ToList();
                                }
                                if (row[6].ToString() != "NULL")
                                {
                                    users[userrow].SMS = Convert.ToBoolean(row[6].ToString());
                                }
                                if (row[7].ToString() != "NULL")
                                {
                                    users[userrow].Channels = row[7].ToString().Split(',').ToList();
                                }
                                if (row[8].ToString() != "NULL")
                                {
                                    users[userrow].Event = row[8].ToString().Split(',').ToList();
                                }
                            }
                            else
                            {
                                if (newuser.ElementAtOrDefault(newuserrow) == null)
                                {
                                    UserData tempnewuser = new UserData();
                                    newuser.Add(tempnewuser);
                                }
                                if (row[0].ToString() != "NULL")
                                {
                                    newuser[newuserrow].RTUsername = row[0].ToString();
                                }
                                if (row[1].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Team = row[1].ToString();
                                }
                                if (row[2].ToString() != "NULL")
                                {
                                    newuser[newuserrow].PhoneEndpoint = row[2].ToString();
                                }
                                if (row[3].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Authenticated = Convert.ToBoolean(row[3].ToString());
                                }
                                if (row[4].ToString() != "NULL")
                                {
                                    newuser[newuserrow].DiscordUsername = row[4].ToString();
                                }
                                if (row[5].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Roles = row[5].ToString().Split(',').ToList();
                                }
                                if (row[6].ToString() != "NULL")
                                {
                                    newuser[newuserrow].SMS = Convert.ToBoolean(row[6].ToString());
                                }
                                if (row[7].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Channels = row[7].ToString().Split(',').ToList();
                                }
                                if (row[8].ToString() != "NULL")
                                {
                                    newuser[newuserrow].Event = row[8].ToString().Split(',').ToList();
                                }
                                newuserrow++;
                            }
                            userrow++;
                        }
                        else
                        {
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
                        if (person.RTUsername == user)
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
                int rownumber = 1;
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (row[0].ToString() == user.RTUsername)
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
                    team = user.Team;
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
                var oblist = new List<object>() { user.RTUsername, team, phoneendpoint, user.Authenticated.ToString(), user.DiscordUsername, roles, user.SMS.ToString(), channels, events };
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
