using sctm.connectors.azureComputerVision.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace sctm.services.discordBot.Models
{
    public class ProcessScreenshotResult
    {
        public string S3Id { get; set; }
        public string ImageUrl { get; set; }


        public string JsonUrl { get; set; }


        public ScreenShotTypes ScreenshotType { get; set; }
        public object Data { get; set; }

    }
}
