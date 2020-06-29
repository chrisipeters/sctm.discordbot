using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public  async static Task<HttpResponseMessage> MakeHttpPostCall(HttpClient client, string url, MultipartFormDataContent content)
        {
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode) return response;
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // 401 
                // Authorization header has been set, but the server reports that it is missing.
                // It was probably stripped out due to a redirect.

                var finalRequestUri = response.RequestMessage.RequestUri;

                if (finalRequestUri != new Uri(url)) return await MakeHttpPostCall(client, finalRequestUri.ToString(), content);
                else return response;
            }
            else return response;
        }
    }
}
