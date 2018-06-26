﻿using Discord;
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
    class Program {

        static void Main(string[] args) => new GameMasterBot("bot_definitions.json").RunBotAsync().GetAwaiter().GetResult();

    }
}
