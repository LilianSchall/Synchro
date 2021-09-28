using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Discord.WebSocket;
using Synchro.Services;

namespace Synchro.Handlers
{
    public class VoiceStateHandler
    {
        //the discord client, that is attached to the bot in order to perform action with the discord api
        private readonly DiscordSocketClient _client;

        public VoiceStateHandler(DiscordSocketClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// attach the Handle Detector that will be triggered each time the bot detectsa connection to a voice channel
        /// in a guild he is connected to
        /// </summary>
        public Task InstallDetector()
        {
            // use this to detect if we have been disconnected from a voice channel
            _client.UserVoiceStateUpdated += HandleDetector;
            return Task.CompletedTask;
        }

        /// <summary>
        /// The Detector detects if the bot has been disconnected from an admin user and clears its working states
        /// </summary>
        /// <param name="user">the user that has (dis)connected from/to a channel</param>
        /// <param name="oldState">the old state of the voice channel before the detector has been triggered</param>
        /// <param name="newState">the new state of the voice channel</param>
        private async Task HandleDetector(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            //we do nothing if the voice channel (dis)connexion isn't related with the bot
            if (user != _client.CurrentUser)
                return;
            
            //if we just got disconnected from a voice channel 
            if (oldState.VoiceChannel.Users.Contains(_client.CurrentUser as SocketUser) && 
                !newState.VoiceChannel.Users.Contains(_client.CurrentUser as SocketUser))
                await BotProperties.GuildPropsMap[newState.VoiceChannel.Guild.Id].ClearMusicProvider(disconnect:false);
            //we just clear the bot from it's worker status
        }
    }
}