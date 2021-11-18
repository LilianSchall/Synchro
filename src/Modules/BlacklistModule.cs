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
    public class BlacklistModule : SettingsBase
    {
        [Command("blacklist")]
        public async Task Blacklist([Remainder] SocketTextChannel channel)
        {
            BotGuildProps props = BotProperties.GuildPropsMap.ContainsKey(Context.Guild.Id) ? 
                BotProperties.GuildPropsMap[Context.Guild.Id]:
                BotProperties.UpdateProps(Context.Guild);
            if (props.IsChannelInBlacklist(channel.Id))
            {
                await ReplyAsync("❌ **This channel is already in blacklist.**");
                return;
            }
            props.AddChannelToBlacklist(channel.Id);
            await ReplyAsync("✅ **Added** `" + channel.Name + "` **to blacklist.**");
        }
    }
}