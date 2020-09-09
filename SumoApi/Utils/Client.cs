using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deployment.Utils
{
    public class Client
    {
        private HttpClient httpClient;
        public Client(byte[] authToken)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(authToken));

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<string> PostRequest(string baseAddress, Dictionary<string, string> contentToSend)
        {
            var content = string.Empty;
            
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddress)))
            {
                var json = JsonConvert.SerializeObject(contentToSend);
                using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                using (var response = await httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    content = await response.Content.ReadAsStringAsync();
                }
            }
            return content;
        }

        public async Task<string> GetRequest(string baseAddress)
        {
            var response = await httpClient.GetAsync(baseAddress);

            string content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
