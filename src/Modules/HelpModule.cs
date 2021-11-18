using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Synchro.Services;

namespace Synchro.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder eb = new EmbedBuilder()
            {
                Title = "Help",
                Color = Color.Blue,
                Description = "Describe the commands the bot has. Every command begins with key ';' "
            }.AddField("play or p [url or title]","▶ ️Play or put a specified music in queue.")
                .AddField("queue","🔢 Display every music added in queue.")
                .AddField("skip or s", "⏩ Skip the current music played.")
                .AddField("leave","⤵️ ️Make the bot leave from its current channel.")
                .AddField("clear", "⏺ Clears the current queue.")
                .AddField("settings", "⚙️ display the settings command.")
                .AddField("help", "🤝 display this message.")
                .WithCurrentTimestamp();

            await ReplyAsync(embed: eb.Build());
        }
    }
}