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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot.Commands.Attachments
{
    public partial class AttachmentCommands
    {
        public async Task RunCommand_JpgAttachment(DiscordClient discord, ulong itemId, MessageCreateEventArgs e)
        {
            var _logAction = "RunCommand_JpgAttachment";
            _logger.LogInformation($"{_logAction} - RunCommand_JpgAttachment Called");


            #region Check User
            _logger.LogInformation($"{_logAction} - Checking User");
            var _user = await _services.GetUser(e.Message.Author.Id);
            if (_user == null)
            {
                _logger.LogInformation($"{_logAction} - Unknown User");
                var _registerEmbed = Embeds.Register(e);
                var _dm = await discord.CreateDmAsync(e.Author);
                await _dm.SendMessageAsync(null, false, _registerEmbed);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":bust_in_silhouette:"));
                return;
            }
            #endregion

            #region Get client
            _logger.LogInformation($"{_logAction} - Getting SCTM Api client");

            if (_services.GetSCTMClient() == null)
            {
                _logger.LogError("Unable to get SCTM Client");
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
            }

            #endregion

            var form = new MultipartFormDataContent();
            #region getContent

            _logger.LogInformation($"{_logAction} - Getting attachment");

            try
            {
                MemoryStream memStream = null;
                StreamContent content = null;
                #region get image to memory stream

                var _imageUrl = e.Message.Attachments.Where(i => i.Id == itemId).Select(i => i.Url).FirstOrDefault();
                if (_imageUrl == null)
                {
                    _logger.LogError("Unable to get url for image");
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                    return;
                }

                var _imageReq = await _services.HttpClient.GetAsync(_imageUrl);
                var _stream = await _imageReq.Content.ReadAsStreamAsync();

                //create new MemoryStream object
                memStream = new MemoryStream();
                memStream.SetLength(_stream.Length);
                //read file to MemoryStream
                _stream.Read(memStream.GetBuffer(), 0, (int)_stream.Length);

                if (memStream == null)
                {
                    _logger.LogError("Unable to create memory stream for image");
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                    return;
                }
                #endregion

                content = new StreamContent(memStream);

                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = e.Message.Attachments.Where(i => i.Id == itemId).Select(i => i.FileName).FirstOrDefault()
                };
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "image/jpg");
                
                form = new MultipartFormDataContent();
                form.Add(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create call content");
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                return;
            }


            #endregion

            await e.Channel.TriggerTypingAsync();

            String _resultString = null;
            #region Make call
            _logger.LogInformation($"{_logAction} - Calling SCTM Api");

            var _url = _config["SCTM:Urls:ProcessLeaderboardImage"] + e.Author.Id;

            var response = await MakeCall(await _services.GetSCTMClient(), _url, form);
            if (!response.IsSuccessStatusCode)
            {
                var _errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed calling leaderboard API\n: " + _errorContent);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                var _dm = await discord.CreateDmAsync(e.Author);
                await _dm.SendMessageAsync(null,false,Embeds.LeaderboardError(e,e.Message.Attachments.Where(i => i.Id == itemId).First(), _errorContent));
                return;
            } else
            {
                _resultString = await response.Content.ReadAsStringAsync();
            }

            #endregion

            ProcessScreenshotResult result = null;
            #region Parse result
            _logger.LogInformation($"{_logAction} - Parsing result");

            try
            {
                result = JsonConvert.DeserializeObject<ProcessScreenshotResult>(_resultString);
                await SendSuccess(e.Message.Attachments.Where(i => i.Id == itemId).First(), e, result);

                if (result == null) throw new Exception("Unable to parse result");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Api Resul: " + ex.Message);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
            }
            
            #endregion
        }

        private async Task<HttpResponseMessage> MakeCall(HttpClient client, string url, MultipartFormDataContent content)
        {
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode) return response;
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // 401 
                // Authorization header has been set, but the server reports that it is missing.
                // It was probably stripped out due to a redirect.

                var finalRequestUri = response.RequestMessage.RequestUri;

                if (finalRequestUri != new Uri(url)) return await MakeCall(client, finalRequestUri.ToString(), content);
                else return response;
            }
            else return response;
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
    }
}
