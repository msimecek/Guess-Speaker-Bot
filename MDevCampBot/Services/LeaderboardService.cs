using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MDevCampBot.Services
{
    public class LeaderboardService
    {
        public static async Task SaveScoreAsync(string playerName, double score)
        {
            using (var hc = new HttpClient())
            {
                var req = new
                {
                    PlayerName = playerName,
                    Score = score
                };

                await hc.PostAsJsonAsync(ConfigurationManager.AppSettings["ScoreApiUrl"], req);
            }
        }

        public static async Task<bool> IsNewPlayer(string playerName)
        {
            HttpResponseMessage resp;
            using (var hc = new HttpClient())
            {
                resp = await hc.GetAsync(string.Format(ConfigurationManager.AppSettings["CheckApiUrl"], playerName));
            }

            return (resp.StatusCode == System.Net.HttpStatusCode.Accepted);
        }
    }
}