using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Synchro.Modules
{
    [Group("settings")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class SettingsBase : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Default action, displays every subcommand of settings section
        /// </summary>
        [Command]
        public async Task DefaultAction()
        {
            Embed embed = new EmbedBuilder()
            {
                Title = "Settings",
                Color = Color.DarkBlue,
                Description =
                    "🏓 Ping: \t Connection test ! Try it !. \n\n" +
                    "⬛️ blacklist: \t Add a channel to blacklist. \n" +
                    "⬜️ unblacklist: \t Remove a channel from blacklist. \n\n"+
                    "🔁 maxduration: \t Set max duration of a video. \n\n"

            }.Build();
            await ReplyAsync(embed: embed);
        }
    }
}