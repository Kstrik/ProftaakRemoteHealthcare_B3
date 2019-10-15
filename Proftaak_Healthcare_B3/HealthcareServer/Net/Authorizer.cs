using Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Net
{
    public class Authorizer
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string filesFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Files/Users");

        public static bool CheckDoctorAuthorization(string username, string password, string cryptoKey)
        {
            if (File.Exists(filesFolderPath + @"/Authentifications.json"))
            {
                string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/Authentifications.json"), cryptoKey);

                if (!String.IsNullOrEmpty(fileContent))
                {
                    JObject json = JObject.Parse(fileContent);
                    JArray authentifications = json.GetValue("authentifications").ToObject<JArray>();

                    foreach (JToken authToken in authentifications)
                    {
                        JObject authentification = authToken.ToObject<JObject>();

                        string usernameAuth = authentification.GetValue("username").ToString();
                        string passwordAuth = authentification.GetValue("password").ToString();

                        if (username == usernameAuth && password == passwordAuth)
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool AddNewDoctorAuthorization(string username, string password, string cryptoKey)
        {
            if (!Authorizer.CheckDoctorAuthorization(username, password, cryptoKey))
            {
                if (File.Exists(filesFolderPath + @"/Authentifications.json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/Authentifications.json"), cryptoKey);

                    JObject json = null;
                    JArray authentifications = null;

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        json = JObject.Parse(fileContent);
                        authentifications = json.GetValue("authentifications").ToObject<JArray>();
                        json.Remove("authentifications");
                    }
                    else
                    {
                        authentifications = new JArray();
                        json = new JObject();
                    }

                    JObject authentification = new JObject();
                    authentification.Add("username", username);
                    authentification.Add("password", password);

                    authentifications.Add(authentification);
                    json.Add("authentifications", authentifications);

                    File.WriteAllText(filesFolderPath + @"/Authentifications.json", DataEncryptor.Encrypt(json.ToString(), cryptoKey));

                    return true;
                }
            }
            return false;
        }

        public static bool ClientExists(string bsn)
        {
            return File.Exists(filesFolderPath + @"/" + bsn + ".json");
        }

        public static bool AddClient(string bsn)
        {
            if (!Authorizer.ClientExists(bsn))
            {
                File.WriteAllText(filesFolderPath + @"/" + bsn + ".json", "");
                return true;
            }
            return false;
        }
    }
}
