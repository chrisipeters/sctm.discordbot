using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sctm.connectors.rsi.models.Leaderboards;
using sctm.services.discordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sctm.services.discordBot
{
    public partial class Embeds
    {
        public static DiscordEmbed RSILeaderboard(DiscordGuild guild, DiscordUser orgChampion, DiscordUser bot, LeaderboardInsight leaderboard, Leaderboard prevLeaderboard, DateTime dataDate)
        {
            /*
            {
  "content": "this `supports` __a__ **subset** *of* ~~markdown~~ 😃 ```js\nfunction foo(bar) {\n  console.log(bar);\n}\n\nfoo(1);```",
  "embed": {
    "title": ":checkered_flag: Racing results: 12 June 10am",
    "description": "Here at The Corporation over the past hour:\n- Greatest Improvement: **OldVandeval** Up 2 places\n- Largest dropoff: **RikkordMemorialRaceway** down 6 spots.\n\n**ChrispyKoala** has been our organization champion for **3 hours!**",
    "color": 13632027,
    "footer": {
      "icon_url": "https://images-ext-1.discordapp.net/external/dfJOA4gfNhMY1LZ45H1Dr-Mik599WVnbm1xuJD-uVk8/https/cdn.discordapp.com/icons/475101141193981953/8c077f3a43b75214e40b6b8b1d3689b8.jpg",
      "text": "Star Citizen Tools by SC TradeMasters"
    },
    "thumbnail": {
      "url": "https://images-ext-1.discordapp.net/external/jF88yjqX6Grv-weNThuUtWk6a9p34nqS_5XMevsttm8/%3Fsize%3D1024/https/cdn.discordapp.com/avatars/238837208029724674/ff3b7c03247d5def0460aa282d421af6.png?width=665&height=665"
    },
    "image": {
      "url": "https://robertsspaceindustries.com/media/18235dq8cydhur/store_small/Misc-Razer-Murray-Ringz-Paint-V5.jpg"
    },
    "author": {
      "name": "ChrispyKangaroo",
      "url": "https://discordapp.com",
      "icon_url": "https://images-ext-1.discordapp.net/external/dfJOA4gfNhMY1LZ45H1Dr-Mik599WVnbm1xuJD-uVk8/https/cdn.discordapp.com/icons/475101141193981953/8c077f3a43b75214e40b6b8b1d3689b8.jpg"
    },
    "fields": [
      {
        "name": "OLD VANDERVAL // :arrow_up: Corp: #13",
        "value": "- *ChrispyKoala*   #7\n- *Chippy_X*   #12"
      },
      {
        "name": "RIKKORD MEMORIAL RACEWAY // :arrow_double_down: Corp: #43",
        "value": "- *ChrispyKoala*   #7\n- *Chippy_X*   #12"
      },
      {
        "name": "DEFFORD LINK // Corp: #10",
        "value": "- *ChrispyKoala*   #7\n- *Chippy_X*   #12"
      }
    ]
  }
}
            */
            return null;
        }
    }
}
