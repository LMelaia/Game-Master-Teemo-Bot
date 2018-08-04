using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Game_Master_Teemo_Bot {

    /// <summary>
    /// Entry point class.
    /// 
    /// This class simply starts the bot.
    /// </summary>
    class Program {

        /// <summary>
        /// Starts the bot.
        /// </summary>
        /// 
        /// <param name="args"></param>
        static void Main(string[] args) => new TeetoBot.Sources._TeetoBot("bot_definitions.json").RunBotAsync().GetAwaiter().GetResult();

    }
}
//
