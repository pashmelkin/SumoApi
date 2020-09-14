using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Deployment.Utils;
using Newtonsoft.Json;

namespace Deployment.Models
{
    public class SumoQueryService : ISumoQueryService
    {

       
        private readonly string baseAddress;
        private readonly IHttpClientFactory _httpClientFactory;
        public readonly Client client;
        private List<DeploymentDetails> Deployments;

        public SumoQueryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            baseAddress = "https://api.au.sumologic.com/api/v1/search/jobs/";
            var sumoClient = _httpClientFactory.CreateClient("SumoClient");
            client = new Client(sumoClient);

            Deployments = new List<DeploymentDetails>();
        }

        public async Task<bool> WaitJobIsReady(string searchJobId)
        {
            var address = baseAddress + searchJobId;
           
            var res = await client.GetRequest(address);

            return true;
        }

        public async Task<List<DeploymentDetails>> GetAllDeployments(string searchJobId)
        {
            var address = baseAddress + searchJobId + "/records?offset=0&limit=100";

            var res = await client.GetRequest(address);
            dynamic result = JsonConvert.DeserializeObject(res);
            var records = result.records;
            foreach (var record in records) {
                var elem = record.map;
                Deployments.Add(new DeploymentDetails() {
                   commitSha = Convert.ToString(elem.commitid),
                   environment = Convert.ToString(elem.env),
                   date = Convert.ToDateTime(elem.time)
               });
            }

            return Deployments;
        }

        public async Task<string> SearchForDeployments()
        {
            var sumoQuery = new Dictionary<string, string>
            {
                { "query", "_sourceCategory=\"/aws/release-prod\" and sme-web |" +
                " json field=_raw \"detail.env\" as env | json field=_raw \"detail.commit_id\" as commitId | json field=_raw \"time\"" +
                "| count by commitId, env, time" },
                { "from", "2020-09-08T12:00:00" },
                { "to", "2020-09-11T19:05:00" },
                { "timeZone", "IST" }
            };

            var content = await client.PostRequest(baseAddress, sumoQuery);
            dynamic result = JsonConvert.DeserializeObject(content);

            return result.id;
        }
    }
}
