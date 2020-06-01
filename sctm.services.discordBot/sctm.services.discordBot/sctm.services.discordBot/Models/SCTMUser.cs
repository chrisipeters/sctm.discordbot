namespace sctm.services.discordBot.Models
{
    public class SCTMUser
    {
        public string Id { get; set; }
        public SCTMUser_Discord Discord { get; set; }
        public SCTMUser_RSI RSI { get; set; }
    }

    public class SCTMUser_RSI
    {
        public int CitizenRecord { get; set; }
        public string Handle { get; set; }
    }
    public class SCTMUser_Discord
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public int Discriminator { get; set; }
    }
}
