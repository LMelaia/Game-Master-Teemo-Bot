using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Master_Teemo_Bot.Modules {

    public class Ping : ModuleBase<SocketCommandContext> {
        
        [Command("ping")]
        public async Task PingAsync() {
            await ReplyAsync("!play https://www.youtube.com/watch?v=EErY75MXYXI");
        }
    }

}
