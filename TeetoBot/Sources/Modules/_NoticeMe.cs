using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeetoBot.Sources.Modules {

    /// <summary>
    /// Hello world command.
    /// </summary>
    public class NoticeMe : ModuleBase<SocketCommandContext> {
        
        /// <summary>
        /// Prints a hello world message to the user who issued the command.
        /// </summary>
        /// <returns></returns>
        [Command("notice me")]
        public async Task Command() {
            await ReplyAsync("Your senpai notices you!");
        }
    }

}
