using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using static TeetoBot.Sources.Logger;
using System.Collections.Generic;

namespace TeetoBot.Sources {

    /// <summary>
    /// Bot class.
    /// 
    /// Provides the bots start method
    /// and initialization routine.
    /// </summary>
    public class _TeetoBot {

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        private static _TeetoBot INSTANCE;

        /// <summary>
        /// Gets the current bot instance.
        /// </summary>
        /// <returns>The static instance of the bot, or throws
        /// an exception if the bot is not yet initialized.</returns>
        public static _TeetoBot GetCurrentInstance() {
            if (INSTANCE == null)
                throw new InvalidOperationException("Bot not yet initialized");

            return INSTANCE;
        }

        /// <summary>
        /// Logger for the application.
        /// </summary>
        private readonly Logger logger = new Logger();

        /// <summary>
        /// Discord socket connection.
        /// </summary>
        private DiscordSocketClient _client;

        /// <summary>
        /// User command service.
        /// </summary>
        private CommandService _commands;

        /// <summary>
        /// Collection of services in use by the bot.
        /// </summary>
        private IServiceProvider _services;

        /// <summary>
        /// The defining properties of the bot. This includes:
        /// name, private token, version and command prefix.
        /// </summary>
        public BotDefinitions Definitions { get; }

        /// <summary>
        /// Creates a new bot instance.
        /// </summary>
        /// 
        /// <param name="jsonBotDefinitions">
        /// The json file name containing the defining bot configuration.
        /// See BotDefinitions.cs for more information.
        /// 
        /// This file must be located in a folder called "bot_cfg"
        /// in the root directory of the project.
        /// </param>
        public _TeetoBot(string jsonBotDefinitions) {
            Definitions = JsonConvert.DeserializeObject<BotDefinitions>(
                new StreamReader("..\\..\\Configuration\\" + jsonBotDefinitions).ReadToEnd()
            );
        }
        
        /// <summary>
        /// Starts the bot.
        /// </summary>
        /// 
        /// <returns>the async task.</returns>
        public async Task RunBotAsync() {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(new AudioService())
                .BuildServiceProvider();
            _client.Log += Log;

            INSTANCE = this;
            
            await RegisterCommandsAsync();
            await _client.LoginAsync(Discord.TokenType.Bot, Definitions.Token);
            await _client.StartAsync();
            GetLogger().Log(Logger.Level.INFO, "Bot is starting...");

            bool rdy = false;
            _client.Ready += async () => {
                rdy = true;
            };

            label: //Got to love this...
            if (!rdy) {
                await Task.Delay(1000);
                goto label;
            }

            foreach (IGuild guild in _client.Guilds) {
                GetLogger().Log(Level.INFO, "Found guild: " + guild.Name);
                if (guild.Name.Equals("For All The Rejects")) {
                    GetLogger().Log(Level.INFO, "Auto joining: " + guild.Name);
                    IGuildChannel channel = null;
                    IReadOnlyCollection<IGuildChannel> collection = await guild.GetVoiceChannelsAsync();
                    IMessageChannel messageChannel = guild.GetDefaultChannelAsync() as IMessageChannel;

                    foreach (IGuildChannel channelA in collection) {
                        if (channelA.Name.Equals("Hell")) {
                            channel = channelA;
                        }
                    }

                    if (channel == null) {
                        return;
                    }

                    await AudioService.GetInstance().LeaveAudioAsync(guild);
                    await AudioService.GetInstance().JoinAudioAsync(guild, channel as IVoiceChannel);
                    await AudioService.GetInstance().LoopAudioAsync(guild, messageChannel, "Nyan");
                }
            }
            
            await Task.Delay(-1);
        }

        /// <summary>
        /// </summary>
        /// <returns>The logger for the application.</returns>
        public Logger GetLogger() {
            return logger;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// 
        /// <param name="arg">The message.</param>
        /// <returns>The async task.</returns>
        private Task Log(LogMessage arg) {
            GetLogger().Log(Logger.Level.DISCORD, arg.ToString().Substring(9));
            //Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers the cammand listener.
        /// </summary>
        /// 
        /// <returns>The async task.</returns>
        private async Task RegisterCommandsAsync() {
            _client.MessageReceived += HandleMessageReceived;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Deciphers and handles commands from a user.
        /// </summary>
        /// 
        /// <param name="message">The message possibly containing a command.</param>
        /// 
        /// <returns>The async task.</returns>
        private async Task HandleMessageReceived(SocketMessage message) {
            var usrMessage = message as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (usrMessage.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                await ExecuteCommand(usrMessage, argPos);
            }

            foreach(string commandPrefix in Definitions.CommandPrefixes) {
                if(usrMessage.HasStringPrefix(commandPrefix, ref argPos)){
                    logger.Log(Level.INFO, "Command requested: \"" + usrMessage.ToString()
                        + "\" from: \"" + usrMessage.Author.Username + "\" in: \"" + usrMessage.Channel + "\"");

                    if(usrMessage.ToString().Replace(commandPrefix, "").Length == 0) {
                        await usrMessage.Channel.SendMessageAsync("Use " + commandPrefix + " help for a list of commands and how to use them.");
                    }
                    
                    await ExecuteCommand(usrMessage, argPos);
                }
            }
        }
        
        /// <summary>
        /// Executes a command within a message from a user.
        /// </summary>
        /// <param name="command">The command message.</param>
        /// <param name="argPos">The argument position.</param>
        /// <returns></returns>
        private async Task ExecuteCommand(SocketUserMessage command, int argPos) {
            var context = new SocketCommandContext(_client, command);
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess) {
                Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
