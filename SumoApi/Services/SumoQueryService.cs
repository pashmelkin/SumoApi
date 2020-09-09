using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Deployment.Utils;
using Microsoft.Extensions.Configuration;

namespace Deployment.Models
{
    public class SumoQueryService : ISumoQueryService
    {

       
        private readonly string baseAddress;
        private readonly IConfiguration Configuration;
        private readonly Client client;

        public SumoQueryService(IConfiguration configuration)
        {

            Configuration = configuration;
            baseAddress = "https://api.au.sumologic.com/api/v1/search/jobs/";

            var sumoAuth = new SumoAuth();
            Configuration.GetSection(SumoAuth.SumoAuthSection).Bind(sumoAuth);


            var authToken = Encoding.ASCII.GetBytes($"{sumoAuth.AccessID}:{sumoAuth.AccessKey}");
            client = new Client(authToken); 
            
        }

        public async Task<DateTime> GetAllDeployments(string searchJobId)
        {
            var address = baseAddress + searchJobId + "/records?offset=0&limit=100";

            var res = await client.GetRequest(address);

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

            var jobId = await client.PostRequest(baseAddress, sumoQuery);
           
            return jobId;
        }
    }
}
