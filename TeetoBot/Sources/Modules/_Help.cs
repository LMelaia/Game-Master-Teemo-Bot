using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TeetoBot.Sources.Modules {
    public class _Help : ModuleBase<SocketCommandContext> {

        /// <summary>
        /// Prints a hello world message to the user who issued the command.
        /// </summary>
        /// <returns></returns>
        [Command("help me")]
        public async Task Command() {
            await ReplyAsync("Fuck off");
        }
    }
}
