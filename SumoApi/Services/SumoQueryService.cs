using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Deployment.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Deployment.Models
{
    public class SumoQueryService : ISumoQueryService
    {

       
        private readonly string baseAddress;
        private readonly IConfiguration Configuration;
        private readonly Client client;
        private List<Tuple<object, object, object>> Deployments;

        public SumoQueryService(IConfiguration configuration)
        {

            Configuration = configuration;
            baseAddress = "https://api.au.sumologic.com/api/v1/search/jobs/";

            var sumoAuth = new SumoAuth();
            Configuration.GetSection(SumoAuth.SumoAuthSection).Bind(sumoAuth);


            var authToken = Encoding.ASCII.GetBytes($"{sumoAuth.AccessID}:{sumoAuth.AccessKey}");
            client = new Client(authToken);

            Deployments = new List<Tuple<object, object, object>>();
        }

        public async Task<DateTime> GetAllDeployments(string searchJobId)
        {
            var address = baseAddress + searchJobId + "/records?offset=0&limit=100";

            var res = await client.GetRequest(address);
            dynamic result = JsonConvert.DeserializeObject(res);
            var records = result.records;
            foreach (var record in records) {
                var elem = record.map;
                Deployments.Add(new Tuple<object, object, object>(elem.commitid, elem.env, elem.time));
            }

            return new DateTime();
        }

        public async Task<string> SearchForDeployments()
        {
            var sumoQuery = new Dictionary<string, string>
            {
                { "query", "_sourceCategory=\"/aws/release-prod\" and sme-web |" +
                " json field=_raw \"detail.env\" as env | json field=_raw \"detail.commit_id\" as commitId | json field=_raw \"time\"" +
                "| count by commitId, env, time" },
                { "from", "2020-09-01T12:00:00" },
                { "to", "2020-09-01T19:05:00" },
                { "timeZone", "IST" }
            };

            var content = await client.PostRequest(baseAddress, sumoQuery);
            dynamic result = JsonConvert.DeserializeObject(content);

            return result.id;
        }
    }
}
