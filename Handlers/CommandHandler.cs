using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Synchro.Handlers
{
    /// <summary>
    /// The handler for the command functionnality: operate on every channel if the message is prefixed
    /// with the right character. This handler triggers then the right module to perform any defined action
    /// </summary>
    public class CommandHandler
    {

        
        //the discord client, that is attached to the bot in order to perform action with the discord api
        private readonly DiscordSocketClient _client;
        
        //the service, that the Command Handler needs in order to aggregate each command module to it.
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }
        
        /// <summary>
        /// attach the Handle Command that will be triggered each time the bot receives a message in a guild he is connected to
        /// and add every command module to the commandservice
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            
            //we abort the command procedure if we wrongly parsed the message received
            if (message == null)
                return;

            int argPos = 0;
            
            //condition to prevent any trigger by a bot message, or a message that isn't prefixed by the right character
            if (!(message.HasCharPrefix(';', ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot)
                return;
            
            //we create a context of the message based on the guild where the bot has received the message
            //This context will centralize every information we can collect about the whereabout of 
            //the command message
            SocketCommandContext context = new SocketCommandContext(_client, message);
            
            //we execute the right module based on the command messages context
            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}