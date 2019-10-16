using Networking.HealthCare;
using Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Files
{
    public class FileHandler
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string filesFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Files/Users");

        public static HistoryData GetHistoryData(string bsn, string cryptoKey)
        {
            lock(bsn)
            {
                if (File.Exists(filesFolderPath + @"/" + bsn + ".json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/" + bsn + ".json"), cryptoKey);

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        try
                        {
                            HistoryData historyData = new HistoryData();

                            JObject historydataJson = JObject.Parse(fileContent);
                            JArray heartratesJson = historydataJson.GetValue("heartrates").ToObject<JArray>();
                            JArray distancesJson = historydataJson.GetValue("distances").ToObject<JArray>();
                            JArray speedsJson = historydataJson.GetValue("speeds").ToObject<JArray>();
                            JArray cycleRhythmsJson = historydataJson.GetValue("cyclerhythms").ToObject<JArray>();

                            foreach (JObject heartrateJson in heartratesJson)
                                historyData.HeartrateValues.Add((int.Parse(heartrateJson.GetValue("heartrate").ToString()), DateTime.Parse(heartrateJson.GetValue("time").ToString())));

                            foreach (JObject distanceJson in distancesJson)
                                historyData.DistanceValues.Add((int.Parse(distanceJson.GetValue("distance").ToString()), DateTime.Parse(distanceJson.GetValue("time").ToString())));

                            foreach (JObject speedJson in speedsJson)
                                historyData.SpeedValues.Add((int.Parse(speedJson.GetValue("speed").ToString()), DateTime.Parse(speedJson.GetValue("time").ToString())));

                            foreach (JObject cycleRhythmJson in cycleRhythmsJson)
                                historyData.CycleRhythmValues.Add((int.Parse(cycleRhythmJson.GetValue("cyclerhythm").ToString()), DateTime.Parse(cycleRhythmJson.GetValue("time").ToString())));

                            return historyData;
                        }
                        catch (Exception e) { }
                    }
                }
                return null;
            }
        }

        public static void SaveHistoryData(string bsn, HistoryData newData, string cryptoKey)
        {
            HistoryData historyData = GetHistoryData(bsn, cryptoKey);
            if (historyData != null)
            {
                historyData.HeartrateValues.AddRange(newData.HeartrateValues);
                historyData.DistanceValues.AddRange(newData.DistanceValues);
                historyData.SpeedValues.AddRange(newData.SpeedValues);
                historyData.CycleRhythmValues.AddRange(newData.CycleRhythmValues);
            }
            else
                historyData = newData;

            JArray heartratesJson = new JArray();
            JArray distancesJson = new JArray();
            JArray speedsJson = new JArray();
            JArray cycleRhythmsjson = new JArray();

            foreach((int heartrate, DateTime time)heartrateData in historyData.HeartrateValues)
            {
                JObject heartrateJson = new JObject();
                heartrateJson.Add("heartrate", heartrateData.heartrate);
                heartrateJson.Add("time", heartrateData.time.ToString());
                heartratesJson.Add(heartrateJson);
            }

            foreach ((int distance, DateTime time) distanceData in historyData.DistanceValues)
            {
                JObject distanceJson = new JObject();
                distanceJson.Add("distance", distanceData.distance);
                distanceJson.Add("time", distanceData.time.ToString());
                distancesJson.Add(distanceJson);
            }

            foreach ((int speed, DateTime time) speedData in historyData.SpeedValues)
            {
                JObject speedJson = new JObject();
                speedJson.Add("speed", speedData.speed);
                speedJson.Add("time", speedData.time.ToString());
                speedsJson.Add(speedJson);
            }

            foreach ((int cycleRhythm, DateTime time) cycleRhythmData in historyData.CycleRhythmValues)
            {
                JObject cycleRhythmJson = new JObject();
                cycleRhythmJson.Add("cyclerhythm", cycleRhythmData.cycleRhythm);
                cycleRhythmJson.Add("time", cycleRhythmData.time.ToString());
                cycleRhythmsjson.Add(cycleRhythmJson);
            }

            JObject historyJson = new JObject();
            historyJson.Add("heartrates", heartratesJson);
            historyJson.Add("distances", distancesJson);
            historyJson.Add("speeds", speedsJson);
            historyJson.Add("cyclerhythms", cycleRhythmsjson);

            File.WriteAllText(filesFolderPath + @"/" + bsn + ".json", DataEncryptor.Encrypt(historyJson.ToString(), cryptoKey));
        }

        public static List<string> GetAllClientBSNS()
        {
            List<string> bsns = new List<string>();

            string[] filePaths = Directory.GetFiles(filesFolderPath);

            foreach(string filePath in filePaths)
            {
                string[] split = filePath.Split('\\');
                string filename = split[split.Length - 1].Replace(".json", "");
                if (filename.ToLower() != "authentifications")
                    bsns.Add(filename);
            }

            return bsns;
        }
    }
}
