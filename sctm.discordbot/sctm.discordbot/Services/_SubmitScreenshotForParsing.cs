using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.discordbot
{
    public partial class Services
    {
        public async Task<SubmitScreenshotForParsing_Response> SubmitScreenshotForParsing(string imageUrl)
        {
            var _logAction = "SubmitScreenshotForParsing";

            SubmitScreenshotForParsing_Response _ret = null;

            _logger.WriteEntry(new logging.Models.LogEntry
            {
                Action = _logAction,
                Level = Microsoft.Extensions.Logging.LogLevel.Information,
                Message = $"Submitting image for parsing"
            });

            var _resImage = await _httpClient.GetAsync(imageUrl);

            var _streamImage = await _resImage.Content.ReadAsStreamAsync();

            MemoryStream memStreamImage = new MemoryStream();
            memStreamImage.SetLength(_streamImage.Length);
            _streamImage.Read(memStreamImage.GetBuffer(), 0, (int)_streamImage.Length);

            var _token = await GetToken();

            if (_token == null)
            {
                _logger.WriteEntry(new logging.Models.LogEntry
                {
                    Action = _logAction,
                    Level = Microsoft.Extensions.Logging.LogLevel.Error,
                    Message = $"Cannot continue without JWT"
                });
                return null;
            }

            HttpResponseMessage response = null;
            #region send to SCTM for parsing

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            using (var form = new MultipartFormDataContent())
            {
                using (var fileContent = new ByteArrayContent(await _resImage.Content.ReadAsByteArrayAsync()))
                {
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                    // "file" parameter name should be the same as the server side input parameter name
                    form.Add(fileContent, "file", imageUrl.Split('/').Last());
                    response = await _httpClient.PostAsync(_config["SCTM:Urls:UploadScreenshot:ToParse"], form);
                }
            }

            #endregion

            if (response.IsSuccessStatusCode)
            {
                _logger.WriteEntry(new logging.Models.LogEntry
                {
                    Action = _logAction,
                    Level = Microsoft.Extensions.Logging.LogLevel.Information,
                    Message = $"Parsing completed successfully"
                });

                _ret = JsonConvert.DeserializeObject<SubmitScreenshotForParsing_Response>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.WriteEntry(new logging.Models.LogEntry
                {
                    Action = _logAction,
                    Level = Microsoft.Extensions.Logging.LogLevel.Error,
                    Message = $"Error parsing image"
                });

                _ret = null;
            }

            return _ret;
        }


    }



    public class SubmitScreenshotForParsing_Response
    {
        public string RecordId { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
    }
}
