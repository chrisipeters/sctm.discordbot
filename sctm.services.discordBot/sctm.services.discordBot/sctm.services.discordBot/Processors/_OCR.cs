using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sctm.services.discordBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Processors
    {
        public async Task ProcessWithOCR(DiscordClient discord, MessageCreateEventArgs e)
        {
            var _logAction = "ProcessWithOCR";
            _logger.LogInformation($"{_logAction} -  command called");


            // Only process if there is 1 and only 1 attachment
            var _dmAuthor = await discord.CreateDmAsync(e.Author);
            try
            {
                if (e.Message == null || e.Message.Attachments == null || e.Message.Attachments.Count == 0)
                {
                    await _dmAuthor.SendMessageAsync("sorry, I can't find any image on that message to process");
                    return;
                }
                else if (e.Message.Attachments.Count > 1)
                {
                    await _dmAuthor.SendMessageAsync("sorry, I can only process messages with a single image on them. Please submit your images one at a time.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending dm to notify of invalid number of attachmentgs: " + ex.Message);
                return;
            }


            #region Check User
            _logger.LogInformation($"{_logAction} - Checking User");
            try
            {
                var _user = await _services.GetUser(e.Message.Author.Id);
                if (_user == null)
                {
                    _logger.LogInformation($"{_logAction} - Unknown User");
                    var _registerEmbed = Embeds.Register(e.Message.Channel.Guild.Name, e.Message.Channel.Guild.IconUrl, e.Message.Channel.Guild.Id, discord.CurrentUser.AvatarUrl);
                    var _dm = await discord.CreateDmAsync(e.Message.Author);
                    await _dm.SendMessageAsync(null, false, _registerEmbed);
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":bust_in_silhouette:"));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for valid user: " + ex.Message);
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
            #region Get content

            _logger.LogInformation($"{_logAction} - Getting attachment");

            try
            {
                MemoryStream memStream = null;
                StreamContent content = null;
                #region get image to memory stream

                var _imageUrl = e.Message.Attachments[0].Url;
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
                    FileName = e.Message.Attachments[0].FileName
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

            #region Make HTTP Call

            var _url = _config["SCTM:Urls:Uploads"].TrimEnd('/') + $"/images/tradingconsole?teamId={e.Message.ChannelId}&discordUser={e.Message.Author.Id}";
            if (_url == null)
            {
                _logger.LogError("Unable to get SCTM Uploads Url from config");
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                return;
            }

            var _res = await Services.MakeHttpPostCall(await _services.GetSCTMClient(), _url, form);
            var _content = await _res.Content.ReadAsStringAsync();
            if (_res.IsSuccessStatusCode)
            {
                
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":white_check_mark:"));
                var _parsed = JsonConvert.DeserializeObject<ProcessScreenshotResult>(_content);
                DiscordEmbed _embed = null;
                switch (_parsed.ScreenshotType)
                {
                    case connectors.azureComputerVision.models.ScreenShotTypes.Unknown:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_BUY:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_SELL:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_BUYConfirm:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.TradeConsole_SELLConfirm:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.FleetManager:
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELL:
                        try
                        {
                            _embed = Embeds.RefinerySellEmbed(JsonConvert.DeserializeObject<connectors.azureComputerVision.models.Terminals.Refinery.Sell>(_parsed.Data.ToString()), e, e.Message.Attachments[0], "abcde");
                            await e.Message.RespondAsync(null, false, _embed);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unable to parse data results: " + ex.Message);
                        }
                        
                        break;
                    case connectors.azureComputerVision.models.ScreenShotTypes.RefineryTerminal_SELLConfirm:
                        try
                        {
                            _embed = Embeds.RefineryConfirm(JsonConvert.DeserializeObject<connectors.azureComputerVision.models.Terminals.Refinery.Confirm>(_parsed.Data.ToString()), e, e.Message.Attachments[0], "abcde");
                            await e.Message.RespondAsync(null, false, _embed);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unable to parse data results: " + ex.Message);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var f = "";
            }
            #endregion
        }
    }
}
