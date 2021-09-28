using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Synchro.Models;
using Synchro.Services;

namespace Synchro.Modules
{
    public class VoiceModule : ModuleBase<SocketCommandContext>
    {
        [Command("leave")]
        public async Task Leave()
        {
            IVoiceChannel channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("❌ **You are not connected to any voice channel currently.**");
                return;
            }
            if (channel != Context.Guild.CurrentUser.VoiceChannel)
            {
                await ReplyAsync(
                    "❌ **Cannot leave voice channel, you are not in the channel **" + Context.Guild.CurrentUser.VoiceChannel);
                return;
            }

            await ReplyAsync("✅ **Disconnected from channel: **" + channel.Name);
            await BotProperties.GuildPropsMap[channel.GuildId].ClearMusicProvider();
        }
        
    }
}