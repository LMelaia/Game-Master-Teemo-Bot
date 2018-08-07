using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TeetoBot.Sources.Modules {
    
    /// <summary>
    /// All commands related to audio.
    /// </summary>
    public class _Audio : ModuleBase<ICommandContext> {
        
        /// <summary>
        /// Audio API.
        /// </summary>
        private readonly AudioService _service;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service">Audio API.</param>
        public _Audio(AudioService service) {
            _service = service;
        }
        
        /// <summary>
        /// Takes a user to a channel called hell.
        /// The bot is then connected to the channel and begins looping over a song.
        /// </summary>
        /// <returns></returns>
        [Command("take me to hell", RunMode = RunMode.Async)]
        public async Task TakeMeToHellCommand() {
            await _service.LeaveAudioAsync(Context.Guild);

            IGuildChannel channel= null;
            IReadOnlyCollection<IGuildChannel> collection = await Context.Guild.GetVoiceChannelsAsync();
                
            foreach (IGuildChannel channelA in collection) {
                if (channelA.Name.Equals("Hell")) {
                    channel = channelA;
                }
            }
            
            if(channel == null) {
                await Context.Channel.SendMessageAsync("I cannot drag you down to a place that does not exist.");
                return;
            }
            
            if((Context.User as IGuildUser).VoiceChannel == null) {
                await Context.Channel.SendMessageAsync("I cannot drag you down to hell when I cannot talk to you in a channel.");
                return;
            }
            

            await (Context.User as IGuildUser)?.ModifyAsync(x => {
                x.Channel = new Discord.Optional<IVoiceChannel>(channel as IVoiceChannel);
            });
            await Context.Channel.SendMessageAsync("I have taken " + (Context.User as IGuildUser).Username + " to hell, where they shall be tortured in an infinite loop for the cancer they have mained.");
            await Context.User.SendMessageAsync("You died");

            await _service.JoinAudioAsync(Context.Guild, channel as IVoiceChannel);
            await _service.LoopAudioAsync(Context.Guild, Context.Channel, "Nyan");
        }

        /// <summary>
        /// Connects the bot to a channel called hell and begins looping
        /// over a song. 
        /// </summary>
        /// <returns></returns>
        [Command("go to hell", RunMode = RunMode.Async)]
        public async Task GoToHellCommand() {
            IGuildChannel channel = null;
            IReadOnlyCollection<IGuildChannel> collection = await Context.Guild.GetVoiceChannelsAsync();

            foreach (IGuildChannel channelA in collection) {
                if (channelA.Name.Equals("Hell")) {
                    channel = channelA;
                }
            }

            if (channel == null) {
                await Context.Channel.SendMessageAsync("I cannot drag you down to a place that does not exist.");
                return;
            }

            await _service.JoinAudioAsync(Context.Guild, channel as IVoiceChannel);
            await Context.Channel.SendMessageAsync("We'll meet again... soon");
            await _service.LoopAudioAsync(Context.Guild, Context.Channel, "Nyan");
        }
        
        /// <summary>
        /// Disconnects the bot from the audio channel in the guild the command was called from.
        /// </summary>
        /// <returns></returns>
        [Command("fuck off", RunMode = RunMode.Async)]
        public async Task LeaveCmd() {
            await _service.LeaveAudioAsync(Context.Guild);
            await Context.Channel.SendMessageAsync("You have not seen the last of me...");
        }

        /// <summary>
        /// Plays a song in the guild the command was called from.
        /// </summary>
        /// <param name="song">The song to play.</param>
        /// <returns></returns>
        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song) {
            await _service.PlayAudioAsync(Context.Guild, Context.Channel, song, null);
        }
    }
}
