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
            await ReplyAsync("⏩ **Skipping to next music...**");
            Console.WriteLine("Skip module called");
            await BotProperties.GuildPropsMap[Context.Guild.Id].SkipToNextMusic();
            Console.WriteLine("Skip module finished");
        }

        [Command("s")]
        public async Task SkipS() => await Skip();
    }
}