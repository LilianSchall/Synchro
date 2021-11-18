using System;
using System.Data;
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
    public class UnblacklistModule : SettingsBase
    {
        [Command("unblacklist")]
        public async Task Unblacklist([Remainder] SocketTextChannel channel)
        {
            BotGuildProps props = BotProperties.GuildPropsMap.ContainsKey(Context.Guild.Id) ? 
                BotProperties.GuildPropsMap[Context.Guild.Id]:
                BotProperties.UpdateProps(Context.Guild);
            if (!props.IsChannelInBlacklist(channel.Id))
            {
                await ReplyAsync("❌ **This channel is not in blacklist.**");
                return;
            }
            props.RemoveChannelFromBlacklist(channel.Id);
            await ReplyAsync("✅ **Removed** `" + channel.Name + "` **from blacklist.**");
        }
    }
}