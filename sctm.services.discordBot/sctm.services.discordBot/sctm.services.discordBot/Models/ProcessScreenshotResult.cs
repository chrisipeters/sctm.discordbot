using System;
using System.Collections.Generic;
using System.Text;

namespace sctm.services.discordBot.Models
{
    public class ProcessScreenshotResult
    {
        public string RecordId { get; set; }
        public ProcessScreenshotResult_ScreenshotDTO Screenshot { get; set; }
        public ProcessScreenshotResult_UploadResultDTO UploadResult { get; set; }
    }

    public class ProcessScreenshotResult_ScreenshotDTO
    {
        public string ImagePath { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
        public string UserIdentifier { get; set; }
        public DateTime AcceptedDate { get; set; }
    }

    public class ProcessScreenshotResult_UploadResultDTO
    {
        public string Type { get; set; }
        public Object Results { get; set; }
        public List<string> Messages { get; set; }
    }
}
