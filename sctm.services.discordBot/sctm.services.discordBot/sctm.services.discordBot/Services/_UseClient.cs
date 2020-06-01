using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Services
    {
        public async Task<HttpClient> GetSCTMClient()
        {
            if (await _GetSCTMToken() == null) return null;
            else
            {
                _sctmHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return _sctmHttpClient;
            }
        }
    }
}
