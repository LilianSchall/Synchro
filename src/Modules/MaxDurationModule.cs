using System;
using System.Data;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.Commands;
using Synchro.Models;
using Synchro.Services;
namespace Synchro.Modules
{

    public class MaxDurationModule : SettingsBase
    {
        [Command("maxduration")]
        public async Task SetMaxDuration([Remainder] int duration)
        {
            if (duration < 1)
            {
                await ReplyAsync("❌ **Invalid duration.**");
                return;
            }
            BotProperties.MaxVideoDuration = duration;
            await ReplyAsync("🔁 **Max duration set to** `" + duration + "` **minutes.**");
        }
    }
}