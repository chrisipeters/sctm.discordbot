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
        public Task<HttpResponseMessage> PostFormDataAsync(this HttpClient httpClient, string url, string token, T data)
        {
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
        }
    }

}
