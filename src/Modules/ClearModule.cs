using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Synchro.Models;
using Synchro.Services;

namespace Synchro.Modules
{
    public class ClearModule : ModuleBase<SocketCommandContext>
    {
        [Command("clear")]
        public async Task Clear()
        {
            BotGuildProps props = BotProperties.GuildPropsMap.ContainsKey(Context.Guild.Id)
                ? BotProperties.GuildPropsMap[Context.Guild.Id]
                : BotProperties.UpdateProps(Context.Guild);

            if (props.IsChannelInBlacklist(((SocketTextChannel)Context.Channel).Id))
            {
                await ReplyAsync("❌ **This channel is blacklisted.**");
                return;
            }

            props.ClearQueue();
            await ReplyAsync("⏺ **Queue cleared ! **" + Context.User.Mention);
        }
    }
}