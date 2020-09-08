using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Deployment.Models
{
    public class SumoQueryService : ISumoQueryService
    {

        private HttpClient client;
        private readonly Uri baseAddress;
        private readonly IConfiguration Configuration;

        public SumoQueryService(IConfiguration configuration)
        {

            Configuration = configuration;
            baseAddress = new Uri("https://api.au.sumologic.com/api/v1/search/jobs/");

            var sumoAuth = new SumoAuth();
            Configuration.GetSection(SumoAuth.SumoAuthSection).Bind(sumoAuth);


            var authToken = Encoding.ASCII.GetBytes($"{sumoAuth.AccessID}:{sumoAuth.AccessKey}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task<ulong> GetListDeployments()
        {
            var values = new Dictionary<string, string>
            {
                { "query", "_sourceCategory=\"/aws/release-prod\" and sme-web |" +
                " json field=_raw \"detail.env\" as env | json field=_raw \"detail.commit_id\" as commitId | json field=_raw \"time\"" +
                "| count by commitId, env, time" },
                { "from", "2020-09-01T12:00:00" },
                { "to", "2020-09-01T19:05:00" },
                { "timeZone", "IST" }
            };

            string id = string.Empty;
            var baseAddresspost = "https://api.au.sumologic.com/api/v1/search/jobs/";
            //string sumoAuth = config.GetSection("SumoAuth").Value;
            // use Ioptions


            client.DefaultRequestHeaders.Add("Accept", "application/json");


            //var response = await client.PostAsync(baseAddresspost, values);
            //
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddresspost)))
            {
                var json = JsonConvert.SerializeObject(values);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic stuff = JsonConvert.DeserializeObject(content);
                        id = stuff.id;

                    }
                }
            }



            return (1);
        }
    }
}
