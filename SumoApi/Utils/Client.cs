using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deployment.Utils
{
    public class Client
    {
        private readonly HttpClient _client;
        public Client(HttpClient client)
        {
            _client = client;

        }

        public async Task<string> PostRequest(string baseAddress, Dictionary<string, string> contentToSend)
        {
            var content = string.Empty;

            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddress)))
            {
                var json = JsonConvert.SerializeObject(contentToSend);
                using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                using (var response = await _client
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
            var response = await _client.GetAsync(baseAddress);

            string content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
