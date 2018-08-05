using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.Audio;

namespace TeetoBot.Sources {

    /// <summary>
    /// Provides audio capabilities of the bot.
    /// 
    /// WORK IN PROGRESS. BUGGY. WARNING.
    /// </summary>
    public class AudioService {

        /// <summary>
        /// Path to the audio files.
        /// </summary>
        private static readonly string AudioFiles = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Audio\"));

        /// <summary>
        /// List of audio channels the bot is connected to.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        /// <summary>
        /// Connects the bot to the specified audio channel in the specified guild.
        /// </summary>
        /// <param name="guild">The guild containing the audio channel.</param>
        /// <param name="target">The audio channel.</param>
        /// <returns></returns>
        public async Task JoinAudio(IGuild guild, IVoiceChannel target) {
            _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Joining audio channel in guild: " + guild.Name);

            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client)) {
                return;
            }
            if (target.Guild.Id != guild.Id) {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient)) {
            }
        }

        /// <summary>
        /// Disconnects the bot from the specified audio channel in the specified guild.
        /// </summary>
        /// <param name="guild">The guild containing the audio channel.</param>
        /// <returns></returns>
        public async Task LeaveAudio(IGuild guild) {
            _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Leaving audio guild: " + guild.Name);

            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client)) {
                await client.StopAsync();
            }
        }

        /// <summary>
        /// Plays a song in the specified guild. If a song is already playing, the song is queued.
        /// </summary>
        /// <param name="guild">The guild to play the song in.</param>
        /// <param name="channel">The message channel the command was called from.</param>
        /// <param name="name">The name of the song.</param>
        /// <param name="handler">EventHandler fired when the song stops playing.</param>
        /// <returns></returns>
        public async Task PlayAudioAsync(IGuild guild, IMessageChannel channel, string name, EventHandler handler) {
            _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Playing song: " + name);

            name = AudioFiles + name + ".mp3";

            if (!File.Exists(name)) {
                _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Audio file not found: " + name);
                return;
            }

            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client)) {
                using (var ffmpeg = CreateProcess(name))
                using (var stream = client.CreatePCMStream(AudioApplication.Music)) {
                    ffmpeg.EnableRaisingEvents = true;
                    ffmpeg.Exited += handler;
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); } finally { await stream.FlushAsync(); }

                }
            }
        }
        
        /// <summary>
        /// Infinitly loops over a song in the specified guild.
        /// </summary>
        /// <param name="guild">The guild to play the song in.</param>
        /// <param name="channel">The message channel the command was called from.</param>
        /// <param name="name">The name of the song.</param>
        /// <returns></returns>
        public async Task LoopAudioAsync(IGuild guild, IMessageChannel channel, string name) {
            _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Looping song: " + name);

            await PlayAudioAsync(guild, channel, name, async (x, y) => {
                await LoopAudioAsync(guild, channel, name);
            });
        }

        /// <summary>
        /// Creates the external audio streaming process.
        /// </summary>
        /// <param name="path">The full path to the song.</param>
        /// <returns>The audio process.</returns>
        private Process CreateProcess(string path) {
            _TeetoBot.GetCurrentInstance().getLogger().Log(Logger.Level.INFO, "Starting new audio process: " + path);

            return Process.Start(new ProcessStartInfo {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
