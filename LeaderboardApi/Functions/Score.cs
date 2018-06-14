using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace LeaderboardApi.Functions
{
    public static class Score
    {
        [FunctionName("Score")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, 
            [DocumentDB("mDevCampBot", "Score", ConnectionStringSetting = "CosmosDbConnection", CreateIfNotExists = true)] IAsyncCollector<ScoreRequest> scoreOutput,
            TraceWriter log)
        {
            var sreq = await req.Content.ReadAsAsync<ScoreRequest>();
            if (sreq == null)
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Score request invalid.");

            await scoreOutput.AddAsync(sreq);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }

    public class ScoreRequest
    {
        public string PlayerName { get; set; }
        public double Score { get; set; }
    }
}
