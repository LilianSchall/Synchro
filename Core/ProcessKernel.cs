using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Synchro.Handlers;
using Synchro.Services;

namespace Synchro.Core
{
    /// <summary>
    /// Process Kernel of the bot: centralize all handlers and set up the necessary
    /// sockets and services so the handlers can work fine
    /// </summary>
    public class ProcessKernel
    {
        //the discord client, that is attached to the bot in order to perform action with the discord api
        private DiscordSocketClient _client;
        
        //the service, that the Command Handler needs in order to aggregate each command module to it.
        private CommandService _commands;
        
        //the token of the bot
        private string _discordToken;

        public ProcessKernel()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            if (!Debugger.IsAttached)
                _discordToken = Environment.GetEnvironmentVariable("DISCORDTOKEN");
            else
                _discordToken = "ODkzMTc5MjgxMTA5MzE5NzEx.YVXsBw._dY8yW31qkyvof5_KgzDARvGbP8";
            Console.WriteLine("discord token: " + _discordToken);
            if (_discordToken == null)
            {
                throw new ArgumentNullException("$DISCORDTOKEN","Please use $DISCORDTOKEN in order to set the token of the discord bot.");
            }
        }
        
        public async Task Main()
        {
            //we attach to the logging event our own custom logging service
            _client.Log += Logging.Log;
            
            //we connnect to the discord api
            await _client.LoginAsync(TokenType.Bot, _discordToken);
            await _client.StartAsync();
            
            //information about the status of the bot displayed on stdout
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected !");
                return Task.CompletedTask;
            };
            
            //setting up  other handlers...
            CommandHandler cmdHandler = new CommandHandler(_client, _commands);
            VoiceStateHandler voiceHandler = new VoiceStateHandler(_client);
            
            await cmdHandler.InstallCommands();
            await voiceHandler.InstallDetector();

            //we want the process Kernel to be running ad vidam eternam for the moment
            await Task.Delay(-1);
        }
    }
}
