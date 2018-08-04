using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TeetoBot.Sources.Modules {
    public class _Help : ModuleBase<SocketCommandContext> {

        /// <summary>
        /// Help Command. Display bot help message and commands.
        /// </summary>
        /// <returns></returns>
        [Command("help")]
        public async Task Command() {
            await ReplyAsync("Fuck off");
        }
    }
}
