using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
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

                var _imageReq = await _sctmClient.GetAsync(_imageUrl);
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

                using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    var _parseResult = await _sctmClient.PostAsync(_url, content);
                    if (!_parseResult.IsSuccessStatusCode)
                    {
                        var _errorContent = await _parseResult.Content.ReadAsStringAsync();
                        ApiError _errorResponse = null; try { _errorResponse = JsonConvert.DeserializeObject<ApiError>(_errorContent); }
                        catch (Exception ex)
                        {
                            var _m = ex.Message;
                        }
                        var f = "";
                    }
                    else
                    {
                        var _successContent = await _parseResult.Content.ReadAsStringAsync();
                        var f = "";
                    }
                }

            }


        }
    }
}
