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
    public class SkipModule : ModuleBase<SocketCommandContext>
    {

        [Command("skip")]
        public async Task Skip()
        {
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null || channel != Context.Guild.CurrentUser.VoiceChannel)
            {
                await ReplyAsync("❌ **You are not connected in the right voice channel currently.**");
                return;
            }

            if (BotProperties.GuildPropsMap[Context.Guild.Id].HasMusicInQueue())
            {
                await ReplyAsync("⏩ **Skipping to next music...**");

                await BotProperties.GuildPropsMap[Context.Guild.Id].SkipToNextMusic();
            }
            else
            {
                await ReplyAsync("❌ **There isn't currently any music playing.**");
            }
            
        }

        [Command("s")]
        public async Task SkipS() => await Skip();
    }
}