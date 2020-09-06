using sctm.connectors.sctmDB.Models.Experience;
using System;

namespace sctm.services.discordBot.Models
{
    public class Leaderboards_Result
    {
        public LeagueSeason Season { get; set; }
        public ExperienceResultSummary Results { get; set; }
    }

    public class LeagueSeason
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public LeagueSeason_Dates Dates { get; set; }
    }

    public class LeagueSeason_Dates
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
