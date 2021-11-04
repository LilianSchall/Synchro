using System;
using System.Collections.Generic;
using System.Security;
using Discord;
using Synchro.Models;
using YoutubeExplode.Channels;

namespace Synchro.Services
{
    public static class BotProperties
    {
        //Dictionnary that references every guild props based on their id
        public static Dictionary<ulong, BotGuildProps> GuildPropsMap = new Dictionary<ulong,BotGuildProps>();
        
        //max duration of a video that can be streamed
        public static int MaxVideoDuration = 30; // in minutes
        
        //max duration without any voice Activity before disconnection from voice channel
        public static TimeSpan TimeoutTime = TimeSpan.FromMinutes(10);

        public static BotGuildProps UpdateProps(IGuild guild)
        {
            BotGuildProps props = new BotGuildProps(guild);
            BotProperties.GuildPropsMap.Add(guild.Id,props);
            return props;
        }
    }
}