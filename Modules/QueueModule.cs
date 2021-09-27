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
            Console.WriteLine("User " + Context.User + "is trying to play music.");
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null || channel != Context.Guild.CurrentUser.VoiceChannel)
            {
                await ReplyAsync("❌ **You are not connected in the right voice channel currently.**");
                return;
            }
            IGuild guild = (Context.User as IGuildUser)?.Guild;

            Embed embed = BotProperties.GuildPropsMap[Context.Guild.Id].GetQueue();
            
            
            
            await ReplyAsync(embed: embed);
        }
        
    }
}