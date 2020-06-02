using System.Collections.Generic;
using System.Linq;

namespace sctm.services.discordBot.Models
{
    public class LeaderboardInsight
    {
        public string Leaderboard { get; set; }
        public List<LeaderboardMapInsight> Maps { get; set; }

        public string Champion { get
            {
                if (Maps == null || !Maps.Any()) return null;
                else
                {
                    var _players = new List<LeaderboardMapInsightPlayerEntry>();
                    foreach (var map in Maps)
                    {
                        foreach (var player in map.Players)
                        {
                            var _check = _players.Where(i => i.Name == player.Name).First();
                            if (_check == null) _players.Add(new LeaderboardMapInsightPlayerEntry { Name = player.Name, CurrentRank = player.CurrentRank });
                            else _check.CurrentRank += player.CurrentRank;
                        }
                    }

                    return _players.OrderByDescending(i => i.CurrentRank).Select(i => i.Name).FirstOrDefault();
                }
            }
        }
    }

    public class LeaderboardMapInsight
    {
        public string Map { get; set; }
        public (string Org, string Players) Urls { get; set; }
        public (int current, int previous) OrgPlacement { get; set; }
        public List<LeaderboardMapInsightPlayerEntry> Players { get; set; }
    }

    public class LeaderboardMapInsightPlayerEntry
    {
        public string Name { get; set; }
        public int CurrentRank { get; set; }
        public int PreviousRank { get; set; }
    }
}
