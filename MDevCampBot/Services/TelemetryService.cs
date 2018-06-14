using MDevCampBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MDevCampBot.Services
{
    public static class TelemetryService
    {
        //static HttpClient hc = new HttpClient();

        public static async Task SendTelemetry(TelemetryModel data)
        {
            using (var hc = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                await hc.PostAsync(ConfigurationManager.AppSettings["TelemetryApiUrl"], content);
            }
        }
    }
}