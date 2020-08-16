namespace sctm.services.discordBot.Models
{
    public class TradingConsole_POST_Result
    {
        public ProcessImage_Result ImagaData { get; set; }
        public object DatabaseRecord { get; set; }
    }

    public class ProcessImage_Result
    {
        public ScreenShotTypes ScreenshotType { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
