using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using sctm.services.discordBot.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Attachments
{
    public partial class AttachmentCommands
    {
        public async Task RunCommand_JpgAttachment(ulong itemId, MessageCreateEventArgs e)
        {
            await e.Channel.TriggerTypingAsync();

            if (_token == null || _tokenDate < DateTime.Now.AddMinutes(-15))
            {
                _token = await _services._GetSCTMToken();
                if (_token != null) _tokenDate = DateTime.Now;
            }

            if (_tokenDate != null && _tokenDate > DateTime.Now.AddMinutes(-15))
            {
                _sctmClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                MemoryStream memStream = null;
                #region get image to memory stream

                var _imageUrl = e.Message.Attachments.Where(i => i.Id == itemId).Select(i => i.Url).FirstOrDefault();
                if (_imageUrl == null)
                {
                    _logger.LogError("Unable to get url for image");
                    return;
                }

                var _imageReq = await _openClient.GetAsync(_imageUrl);
                var _stream = await _imageReq.Content.ReadAsStreamAsync();

                //create new MemoryStream object
                memStream = new MemoryStream();
                memStream.SetLength(_stream.Length);
                //read file to MemoryStream
                _stream.Read(memStream.GetBuffer(), 0, (int)_stream.Length);

                if (memStream == null)
                {
                    _logger.LogError("Unable to create memory stream for image");
                    return;
                }
                #endregion

                var _url = _config["SCTMUrls:ProcessLeaderboardImage"] + e.Author.Id;

                var form = new MultipartFormDataContent();
                var content = new StreamContent(memStream);
                form.Add(content, "file");
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = e.Message.Attachments.Where(i => i.Id == itemId).Select(i => i.FileName).FirstOrDefault()
                };
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "image/jpg");
                form.Add(content);


                string _stringResult = null;
                var response = await _sctmClient.PostAsync(_url, form);
                if (response.IsSuccessStatusCode)
                {
                    _stringResult = await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // 401 
                    // Authorization header has been set, but the server reports that it is missing.
                    // It was probably stripped out due to a redirect.

                    var finalRequestUri = response.RequestMessage.RequestUri; // contains the final location after following the redirect.

                    if (finalRequestUri != new Uri(_url)) // detect that a redirect actually did occur.
                    {
                        // If this is public facing, add tests here to determine if Url should be trusted
                        response = await _sctmClient.PostAsync(finalRequestUri, form);
                        if (response.IsSuccessStatusCode)
                        {
                            _stringResult = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            // give up
                            _stringResult = await response.Content.ReadAsStringAsync();
                            var f = "";
                        }
                    }
                }
                else
                {
                    var _errorContent = await response.Content.ReadAsStringAsync();
                    ApiError _errorResponse = null; try { _errorResponse = JsonConvert.DeserializeObject<ApiError>(_errorContent); }
                    catch (Exception ex)
                    {
                        var _m = ex.Message;
                    }
                    var f = "";
                }

                if (_stringResult == null)
                {
                    // no data - bail
                    var f = "";
                }
                else
                {
                    var _parsedResult = JsonConvert.DeserializeObject<ProcessScreenshotResult>(_stringResult); 
                    
                }


            }


        }

    }
}
