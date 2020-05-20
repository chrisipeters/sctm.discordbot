using System.Net.Http;

namespace sctm.services.discordBot.Commands.Attachments
{
    class AttachmentCommands
    {
        private HttpClient _httpClient;

        public AttachmentCommands(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
