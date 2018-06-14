using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace LeaderboardApi.Functions
{
    public static class Telemetry
    {
        [FunctionName("Telemetry")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, 
            [DocumentDB("mDevCampBot", "Telemetry", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDbConnection")] IAsyncCollector<dynamic> outTelemetry,
            TraceWriter log)
        {
            log.Info("Received telemetry");

            dynamic data = await req.Content.ReadAsAsync<object>();
            outTelemetry.AddAsync(data);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
