using MDevCampBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MDevCampBot.Controllers
{
    public class ServiceController : ApiController
    {
        [HttpPost]
        [Route("api/service/update")]
        public async Task<HttpResponseMessage> Update()
        {
            var people = await PeopleService.DownloadPeopleListAsync();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
