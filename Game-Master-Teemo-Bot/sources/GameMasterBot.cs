using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;

namespace Game_Master_Teemo_Bot {
    class GameMasterBot {

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private BotDefinitions definitions;

        public GameMasterBot(string jsonBotDefinitions) {
            definitions = JsonConvert.DeserializeObject<BotDefinitions>(
                new StreamReader("..\\..\\bot_properties\\" + jsonBotDefinitions).ReadToEnd()
            );
        }
        
        public async Task RunBotAsync() {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = definitions.Token;

            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg) {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync() {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg) {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix(definitions.CommandPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
