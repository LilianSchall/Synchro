using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Synchro.Modules
{
    public class SayModule : SettingsBase
    {
        /// <summary>
        /// Simple ping pong command that is used so we can know if the bot is online
        /// </summary>
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync($"🏓 Pong {Context.User.Mention}!");
        }
    }
}