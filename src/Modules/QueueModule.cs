using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Synchro.Models;
using Synchro.Services;


namespace Synchro.Modules
{
    public class QueueModule : ModuleBase<SocketCommandContext>
    {
        [Command("queue")]
        public async Task GetQueue()
        {
            BotGuildProps props = BotProperties.GuildPropsMap.ContainsKey(Context.Guild.Id) ? 
                BotProperties.GuildPropsMap[Context.Guild.Id]:
                BotProperties.UpdateProps(Context.Guild);

            if (props.IsChannelInBlacklist(((SocketTextChannel)Context.Channel).Id))
            {
                await ReplyAsync("❌ **This channel is blacklisted.**");
                return;
            }
            
            Console.WriteLine("User " + Context.User + "is trying to play music.");
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null || channel != Context.Guild.CurrentUser.VoiceChannel)
            {
                await ReplyAsync("❌ **You are not connected in the right voice channel currently.**");
                return;
            }

            if (BotProperties.GuildPropsMap[channel.Guild.Id].HasMusicInQueue())
            {
                Embed embed = BotProperties.GuildPropsMap[Context.Guild.Id].GetQueue();
                await ReplyAsync(embed: embed);
            }
            else
            {
                await ReplyAsync("⤵️ **You have currently nothing in queue.** ");
            }
            
        }
        
    }
}