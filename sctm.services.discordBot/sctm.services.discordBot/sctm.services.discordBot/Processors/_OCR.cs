using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using sctm.connectors.sctmDB.Models.DBModels.Screenshots.OCR.TradeConsole;
using sctm.connectors.sctmDB.Models.Models.Screenshots;
using sctm.services.discordBot.Models;
using Serilog;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sctm.services.discordBot
{
    public partial class Processors
    {
        public async Task ProcessWithOCR(DiscordClient discord, MessageCreateEventArgs e)
        {
            var _logAction = "ProcessWithOCR";
            Log.Information($"{_logAction} -  command called");

            var _userId = e.Author.Id;
            var _teamId = e.Message.ChannelId;
            var _orgId = e.Message.Channel.GuildId;

            var _client = await _services.GetSCTMClient();
            if (_client == null) Log.Error("{logAction}: unable to get SCTM client", _logAction);

            // prepare image
            var form = new MultipartFormDataContent();
            if (_client != null)
            {
                Log.Information("{logAction}: getting image to process", _logAction);
                try
                {
                    MemoryStream memStream = null;
                    StreamContent content = null;
                    #region get image to memory stream

                    var _imageUrl = e.Message.Attachments[0].Url;
                    if (_imageUrl == null)
                    {
                        Log.Error("Unable to get url for image");
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                        return;
                    }

                    var _simpleClient = new HttpClient();

                    var _imageReq = await _simpleClient.GetAsync(_imageUrl);
                    var _stream = await _imageReq.Content.ReadAsStreamAsync();

                    //create new MemoryStream object
                    memStream = new MemoryStream();
                    memStream.SetLength(_stream.Length);
                    //read file to MemoryStream
                    _stream.Read(memStream.GetBuffer(), 0, (int)_stream.Length);

                    if (memStream == null)
                    {
                        Log.Error("Unable to create memory stream for image");
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

                    Log.Information("{logAction}: image ready", _logAction);

                    _simpleClient.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to create call content");
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName((DiscordClient)e.Client, ":cry:"));
                    return;
                }
            }


            // make the call
            TradingConsole_POST_Result _data = null;
            if (_client != null)
            {
                var _url = _config["SCTM:Urls:Uploads"].TrimEnd('/') + $"/images/tradeconsole?user={_userId}&team={_teamId}&organization={_orgId}";
                Log.Information("{logAction}: making call to: {url}", _logAction, _url);

                var _res = await Services.MakeHttpPostCall(_client, _url, form);

                var _content = await _res.Content.ReadAsStringAsync();

                Log.Information("{logAction}: UploadsAPI call resulted in: {result} with content of {@_content}", _logAction, _res.StatusCode);

                if (_res.IsSuccessStatusCode)
                {
                    
                    _data = JsonConvert.DeserializeObject<TradingConsole_POST_Result>(_content);
                } else
                {
                    // fail
                    try
                    {
                        if(_content.Contains("I'm sorry Dave, I cannot process this file for you again"))
                        {
                            await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":cry:"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "{logAction}: Exception encountered making call to uploads API", _logAction);
                    }
                }
                _client.Dispose();
            }

            // send embed
            DiscordEmbed _embed = null;
            if (_data != null)
            {
                Log.Information("{logAction}: Preparing embed for data: {@data}", _logAction, _data);


                if(_data.Image.Data.ScreenshotType == ScreenShotTypes.CreateScreen)
                {
                    // create screen
                    var _dbRecord = JsonConvert.DeserializeObject<CreateScreen>(_data.DatabaseRecord.ToString());
                    _embed = Embeds.CreateScreen(_dbRecord, e, e.Message.Attachments[0], _dbRecord.Id);

                } else if(_data.Image.Data.ScreenshotType == ScreenShotTypes.ConfirmScreen)
                {
                    // Confirm screen
                    var _dbRecord = JsonConvert.DeserializeObject<ConfirmScreen>(_data.DatabaseRecord.ToString());
                    _embed = Embeds.ConfirmScreen(_dbRecord, e, e.Message.Attachments[0], _dbRecord.Id);
                }
                
                Log.Information("{logAction}: sending embed for {screenshotType}", _logAction, _data.Image.Data.ScreenshotType.ToString());
                if (_embed != null) await e.Message.RespondAsync(null, false, _embed);
            }

        }
    }
}
