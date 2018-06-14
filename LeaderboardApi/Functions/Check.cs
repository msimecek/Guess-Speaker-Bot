using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace LeaderboardApi.Functions
{
    public static class Check
    {
        [FunctionName("Check")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "delete", Route = "check/{userName}")]HttpRequestMessage req,
            string userName,
            [DocumentDB("mDevCampBot", "Score", SqlQuery = "SELECT * FROM c WHERE c.PlayerName = {userName}", ConnectionStringSetting = "CosmosDbConnection")] IEnumerable<dynamic> user,
            [DocumentDB("mDevCampBot", "Score", ConnectionStringSetting = "CosmosDbConnection")] DocumentClient client,
            TraceWriter log)
        {
            if (req.Method == HttpMethod.Delete)
            {
                if (user.Count() > 0)
                {
                    var u = user.FirstOrDefault();
                    client.DeleteDocumentAsync(u._self.Value);

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            else
                return (user.Count() > 0) ? new HttpResponseMessage(HttpStatusCode.Forbidden) : new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
