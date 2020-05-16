using Microsoft.Extensions.Configuration;
using sctm.logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace sctm.discordbot
{
    public partial class Services
    {
        private IConfiguration _config;
        private HttpClient _httpClient;
        private FileLogger _logger;
        private string _token = null;
        private DateTime _tokenDate = DateTime.MinValue;

        public Services(IConfiguration config, HttpClient httpClient, FileLogger logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger = logger;
        }
    }
}
