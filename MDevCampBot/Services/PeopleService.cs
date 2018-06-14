using MDevCampBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MDevCampBot.Services
{
    public class PeopleService
    {
        static string _peopleListUrl = ConfigurationManager.AppSettings["PeopleListUrl"];

#if DEBUG
        const string PEOPLE_LIST_PATH = @"C:\tmp\people-list.json";
#else
        const string PEOPLE_LIST_PATH = @"D:\home\people-list.json"; // Azure App Service local storage
#endif


        /// <summary>
        /// Gets people list from local file system. If not present, downloads it from Azure Storage.
        /// </summary>
        public static async Task<PeopleCollection> GetPeopleLisAtAsync()
        {
            if (!File.Exists(PEOPLE_LIST_PATH))
            {
                return await DownloadPeopleListAsync();
            }
            else
            {
                var peopleRaw = File.ReadAllText(PEOPLE_LIST_PATH);
                return JsonConvert.DeserializeObject<PeopleCollection>(peopleRaw);
            }
        }

        /// <summary>
        /// Downloads people list from Azure Storage and stores it locally.
        /// </summary>
        public static async Task<PeopleCollection> DownloadPeopleListAsync()
        {
            PeopleCollection res = null;
            using (var hc = new HttpClient())
            {
                try
                {
                    res = PeopleCollection.FromCsv(await hc.GetStringAsync(_peopleListUrl));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to download people list from URL {_peopleListUrl}.");
                }
            }

            File.WriteAllText(PEOPLE_LIST_PATH, JsonConvert.SerializeObject(res));

            return res;
        }
    }
}