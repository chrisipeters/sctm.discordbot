using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using sctm.connectors.azureComputerVision.models.Terminals.Refinery;
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
                string _stringError = null;
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
                            _stringError = await response.Content.ReadAsStringAsync();
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

                if(_stringError != null){
                    await e.Channel.SendMessageAsync(_stringError);
                }
                else
                {
                    var _parsedResult = JsonConvert.DeserializeObject<ProcessScreenshotResult>(_stringResult);
                    await SendSuccess(e.Message.Attachments.Where(i => i.Id == itemId).First(),e,_parsedResult);
                }


            }


        }


        private async Task SendSuccess(DiscordAttachment image, MessageCreateEventArgs e, ProcessScreenshotResult result)
        {
            string _json = null;
            switch (result.UploadResult.Type)
            {
                case "Unknown":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":question:"));
                    break;
                case "TradeConsole_BUY":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":arrows_counterclockwise:"));
                    break;
                case "TradeConsole_SELL":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":arrows_counterclockwise:"));
                    break;
                case "TradeConsole_BUYConfirm":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":arrows_counterclockwise:"));
                    break;
                case "TradeConsole_SELLConfirm":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":arrows_counterclockwise:"));
                    break;
                case "FleetManager":
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":arrows_counterclockwise:"));
                    break;
                case "RefineryTerminal_SELL":
                    _json = result.UploadResult.Results.ToString().Replace("{{", "{").Replace("}}", "}");
                    Sell _RefineryTerminal_SELL = JsonConvert.DeserializeObject<Sell>(_json);

                    var _embed_rts = Embeds.RefinerySellEmbed(_RefineryTerminal_SELL, e, image, result.RecordId);
                    await e.Channel.SendMessageAsync(null, false, _embed_rts);
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":pick:"));
                    break;
                case "RefineryTerminal_SELLConfirm":
                    _json = result.UploadResult.Results.ToString().Replace("{{", "{").Replace("}}", "}");
                    Confirm _RefineryTerminal_SELLConfirm = JsonConvert.DeserializeObject<Confirm>(_json);

                    var _embed_rtsc = Embeds.RefineryConfirm(_RefineryTerminal_SELLConfirm, e, image, result.RecordId);
                    await e.Channel.SendMessageAsync(null, false, _embed_rtsc);
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":pick:"));
                    break;
                default:
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":question:"));
                    break;
            }
        }

        private async Task SendFail(MessageCreateEventArgs e, ProcessScreenshotResult result)
        {

        }
    }
}
