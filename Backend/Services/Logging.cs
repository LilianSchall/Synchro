using System;
using System.Threading.Tasks;
using Discord;

namespace Synchro.Services
{
    public class Logging
    {
        /// <summary>
        /// Log: display every Discord log on stdout.
        /// </summary>
        /// <param name="msg">a logging message to display</param>
        /// <returns></returns>
        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}