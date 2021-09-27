using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.Commands;
using Synchro.Models;
using Synchro.Services;

namespace Synchro.Modules
{
    public class PlayModule : ModuleBase<SocketCommandContext>
    {
        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string message)
        {
            Console.WriteLine("User " + Context.User + "is trying to play music.");
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("❌ **You are not connected to any voice channel currently.**");
                return;
            }
            BotGuildProps props = BotProperties.GuildPropsMap.ContainsKey(channel.Guild.Id) ? 
                BotProperties.GuildPropsMap[channel.Guild.Id]:
                BotProperties.UpdateProps(channel.Guild);
            

            
            if (!props.IsConnected())
            {
                Console.WriteLine("Connecting to channel " + channel.Name);
                IAudioClient audioClient = await channel.ConnectAsync(selfDeaf:true);
                if (audioClient == null)
                {
                    Console.WriteLine("audioclient isn't defined.");
                    return;
                }
                Console.WriteLine("Connected !");
                await ReplyAsync("👍 **Joined**" + "`"+channel.Name + "`" + "**!**");
                await ReplyAsync("🎶 **Searching for** `" + message + "`");
                QueuedItemInfo info = props.AddMusic(message);
                await ReplyAsync("**Playing** `" + info.Result.Title + "` **- Now !**");
                await props.PlayMusic(audioClient);
            }
            else
            {
                Console.WriteLine("Already connected to channel: " + channel.Name);
                await ReplyAsync("🎶 **Searching for** `" + message + "`");
                QueuedItemInfo info = null;
                try
                {
                    info = props.AddMusic(message);
                }
                catch (Exception e)
                {
                    await ReplyAsync("❌**Sorry, video is longer than " + BotProperties.MaxVideoDuration + " minutes.**");
                    return;
                }
                

                EmbedBuilder eb = new EmbedBuilder()
                {
                    Title = "Added to queue",
                    Description = "["+ info.Result.Title +"](" + info.Result.Url + ")",
                    ThumbnailUrl = info.Result.Thumbnails[0].Url
                };
                eb.AddField("Channel", info.Result.Author)
                    .AddField("Position in queue", info.Position);

                await ReplyAsync(embed: eb.Build());
            }
        }

        [Command("p", RunMode = RunMode.Async)]
        public async Task PlayP([Remainder] string message) => await Play(message);
        
    }
}