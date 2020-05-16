using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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


            #region send to SCTM for parsing

            var content = new MultipartFormDataContent();

            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value is FormFile)
                {
                    var file = value as FormFile;
                    content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = prop.Name, FileName = file.FileName };
                }
                else
                {
                    content.Add(new StringContent(JsonConvert.SerializeObject(value)), prop.Name);
                }
            }

            if (!string.IsNullOrWhiteSpace(token))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpClient.PostAsync(url, content);

            #endregion



            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "csvFile", "filename");

            using (var form = new MultipartFormDataContent())
            {
                using (var streamContent = new StreamContent(_streamImage))
                {
                    using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                    {
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                        // "file" parameter name should be the same as the server side input parameter name
                        form.Add(fileContent, "file", Path.GetFileName(filePath));
                        HttpResponseMessage response = await httpClient.PostAsync(url, form);
                    }
                }
            }




            HttpContent c = new StreamContent(memStreamImage);
            var _resParse = await _httpClient.PostAsync(_config["SCTM:Urls:UploadScreenshot:ToParse"], c);

            if (_resParse.IsSuccessStatusCode)
            {
                _logger.WriteEntry(new logging.Models.LogEntry
                {
                    Action = _logAction,
                    Level = Microsoft.Extensions.Logging.LogLevel.Information,
                    Message = $"Parsing completed successfully"
                });

                _ret = JsonConvert.DeserializeObject<SubmitScreenshotForParsing_Response>(await _resParse.Content.ReadAsStringAsync());
            } else
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
