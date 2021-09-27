using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Synchro.Services;

namespace Synchro.Models
{
    /// <summary>
    /// unique props object assigned to each guild the bot is deployed to
    /// </summary>
    public class BotGuildProps
    {
        //the service that is used to centralize every action with audio
        private MusicService _musicService;
        
        //the guild object the props object is referenced to
        private IGuild _guild;

        
        
        public BotGuildProps(IGuild guild)
        {
            _musicService = new MusicService();
            _guild = guild;
            
        }
        
        /// <summary>
        /// Is connected is used so we can decide if the bot has to join a channel or not
        /// If it is already in a channel, it won't connect to another one
        /// Else, it joins a channel
        /// </summary>
        /// <returns>boolean used to check if the bot is already connected on a voice channel</returns>
        public bool IsConnected() => _musicService.IsConnected();
        
        /// <summary>
        /// AddMusic is used to search a video based on a info given by user
        /// AddMusic will call AddMusic from musicService
        /// </summary>
        /// <param name="info">the info which we will base the video research on</param>
        /// <returns>Info object about the video found</returns>
        /// <exception cref="ArgumentException">
        /// If the video found is greater than 30 minutes long
        /// we do not add it to streaming queue
        /// </exception>
        public QueuedItemInfo AddMusic(string info)
        {
            try
            {
                return _musicService.AddMusic(info);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Video is longer than " + BotProperties.MaxVideoDuration + " minutes!");
            }
            
        }
        
        
        /// <summary>
        /// Gets an embed of the queue of the guild where the command has been called
        /// </summary>
        /// <returns>an Embed of the current queue in the guild</returns>
        public Embed GetQueue() => _musicService.GetQueue(_guild);
        
        /// <summary>
        /// Method called when the bot has to leave a channel
        /// </summary>
        public void ClearMusicProvider() => _musicService.ClearMusicProvider();
        
        /// <summary>
        /// Method called when a user wants to clear its streaming queue
        /// </summary>
        public void ClearQueue() => _musicService.ClearQueue();
        
        /// <summary>
        /// Play Music will call PlayMusic method of music service.
        /// </summary>
        /// <param name="audioClient">The audioclient where we should copy the music stream into</param>
        /// <returns>An async task</returns>
        public async Task PlayMusic(IAudioClient audioClient) => await _musicService.PlayMusic(audioClient,_guild);
        
        /// <summary>
        /// SkipToNextMusic is used when the user wants to skip the music
        /// </summary>
        /// <returns></returns>
        public async Task SkipToNextMusic()
        {
            _musicService.SkipToNextMusic();
        }
    }
}