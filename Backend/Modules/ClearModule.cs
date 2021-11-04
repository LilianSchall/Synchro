using System.Threading.Tasks;
using Discord.Commands;
using Synchro.Services;

namespace Synchro.Modules
{
    public class ClearModule : ModuleBase<SocketCommandContext>
    {
        [Command("clear")]
        public async Task Clear()
        {
            BotProperties.GuildPropsMap[Context.Guild.Id].ClearQueue();
            await ReplyAsync("⏺ **Queue cleared ! **" + Context.User.Mention);
        }
    }
}