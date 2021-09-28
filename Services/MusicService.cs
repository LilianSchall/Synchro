using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Synchro.Models;
using Synchro.Services;
using YoutubeExplode.Search;

namespace Synchro.Services
{
    /// <summary>
    /// Music Service
    /// Service that centralize every music functionnalities
    /// such as playing music, adding music to queue
    /// skip music, pause music...
    /// </summary>
    public class MusicService
    {
        
        //the current music queue
        private List<VideoSearchResult> _streamQueue;
        
        //Streaming service used to flush music into the discord stream
        private  readonly StreamingService _streamingService;
        
        //TokenGenerator used when the user wants to skip a music
        //This cancel the copyAsync method called in StreamingService
        private CancellationTokenSource _streamCancelToken;

        /*
         * States bool
         * isPlaying: bool that say whether or not the bot is playing music
         * isConnected: bool that say whether or not the bot is connected to a voice channel
         */
        private bool _isPlaying;
        private bool _isConnected;

        private Timer _disconnectTimer = null;
        
        //Audio-referenced states
        private IAudioClient _audioClient = null;
        private IVoiceChannel _currentChannel = null;

        private int infiniteWaitingTime = -1; //infinite
        public MusicService()
        {
            _streamQueue = new List<VideoSearchResult>();
            _streamingService = new StreamingService();
            _streamCancelToken = new CancellationTokenSource();
            _disconnectTimer = new Timer(OnTimerElapsed, null, infiniteWaitingTime, 
                (int)BotProperties.TimeoutTime.TotalMilliseconds);//this is ok because < Int32.MaxValue
            _isPlaying = false;
            _isConnected = false;
        }
        
        /// <summary>
        /// IsConnected Getter
        /// </summary>
        /// <returns>returns whether the bot is connected to a voice channel in the guild</returns>
        public bool IsConnected() => _isConnected;
        
        /// <summary>
        /// Check if there is any music in the streamqueue right now
        /// </summary>
        /// <returns>whether there is any music currently in the streaming queue</returns>
        public bool HasMusicInQueue() => _streamQueue.Count > 0 || _isPlaying;
        
        /// <summary>
        /// AddMusic: AddMusic is called when a user wnats to add a music to the streaming queue
        /// It will search the meta data of the content through _streamingService Search functionnality
        /// than it will check whether the video is longer than a constant defined in Bot static properties
        /// </summary>
        /// <param name="info">the info which we will base the video research on</param>
        /// <returns>Info object about the video found</returns>
        /// <exception cref="InvalidConstraintException">
        /// If the video found is greater than 30 minutes long
        /// we do not add it to streaming queue
        /// </exception>
        public QueuedItemInfo AddMusic(string info)
        {
            VideoSearchResult result = _streamingService.SearchContent(info);
            QueuedItemInfo queuedItemInfo = null;
            
            //if we have found something relevant
            if (result != null)
            {
                //we check its duration
                if (result.Duration != null && result.Duration > TimeSpan.FromMinutes(BotProperties.MaxVideoDuration))
                    throw new InvalidConstraintException(
                        "Video is longer than" + BotProperties.MaxVideoDuration + " minutes");
                
                //Here, we add the content to the streaming queue
                Console.WriteLine("Found music, title: " + result.Title);
                queuedItemInfo = AddMusicToQueue(result);
            }

            return queuedItemInfo;
        }
        
        /// <summary>
        /// Gets an embed of the queue of the guild where the command has been called
        /// </summary>
        /// <returns>an Embed of the current queue in the guild</returns>
        public Embed GetQueue(IGuild guild)
        {
            string queue = "";
            //we define a standard embed structure with title color and little description
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.DarkBlue,
                Title = "Queue",
                Description = guild.Name
            };
            //we get every information of the videos that are stored in the streaming queue
            for(int i = 0; i< _streamQueue.Count; i++)
            {
                queue += (i + 1) + " - **" + _streamQueue[i].Title + "**" + " by " + "**" + _streamQueue[i].Author +
                         "** " + " (Duration: " + _streamQueue[i].Duration + ")" + 
                         "\n";
            }
            embed.AddField("Currently in the queue:", queue)
                .WithCurrentTimestamp(); //we add the date

            return embed.Build(); //well, we build the embed
        }
        
        /// <summary>
        /// ClearMusicProvider will status the bot as not connected to any channel,
        /// not playing any music
        /// and with the streaming queue cleared
        /// </summary>
        public async Task ClearMusicProvider(bool disconnect)
        {
            _isConnected = false;
            _isPlaying = false;
            _streamQueue = new List<VideoSearchResult>();
            _audioClient = null;

            if (_currentChannel != null && disconnect)
                await _currentChannel.DisconnectAsync();
        }

        /// <summary>
        /// Method called when a user wants to clear its streaming queue
        /// </summary>
        public void ClearQueue() => _streamQueue.Clear();
        
        /// <summary>
        /// PlayMusic is going to EOS when the bot is not already playing music
        /// PlayMusic will call PlayNextMusic in order to tell the streaming service to
        /// flush a video into discord stream
        /// </summary>
        /// <param name="channel">The channel we are connected to</param>
        /// <param name="audioClient">The guild audioclient</param>
        /// <param name="guild">The guild where the bot has been requested to play music</param>
        public async Task PlayMusic(IVoiceChannel channel,IAudioClient audioClient, IGuild guild)
        {
            _isConnected = true;
            _currentChannel = channel;
            //as we said, we don't tell the streaming service to play a music if it is already playing one
            if(_isPlaying)
                return;
            //which means, if we already in a channel and we didn't get disconnected
            if(audioClient != null)
                _audioClient = audioClient;

            _disconnectTimer.Change((int) BotProperties.TimeoutTime.TotalMilliseconds,
                (int) BotProperties.TimeoutTime.TotalMilliseconds); //this is ok because < Int32.MaxValue
            while (_streamQueue.Count > 0)
            {
                _isPlaying = true;
                //this await call will stuck at this state as long as we are playing a music
                //and will play music as long as there is something in the streaming queue
                await PlayNextMusic(ConsumeMusic(guild),guild);
            }
            //reset states
            _isPlaying = false;
        }
        
        /// <summary>
        /// Method used to skip a playing music
        /// The method is telling a cancellation token to be marked as active so that
        /// CopyToAsync in the streaming service is interrupted
        /// </summary>
        public void SkipToNextMusic()
        {
            _streamCancelToken.Cancel();
            _streamCancelToken.Dispose(); //we free the ram storage of token generator
            _streamCancelToken = new CancellationTokenSource(); //we create a new one for the following music 
            
        }
        
        
        
        /// <summary>
        /// PlayNextMusic is private method called by PlayMusic in order to tell the
        /// streaming service to stream music to the discord stream
        /// </summary>
        /// <param name="audioClient">the guild audioclient</param>
        /// <param name="music">the content to stream to the discord stream</param>
        private async Task PlayNextMusic(VideoSearchResult music,IGuild guild)
        {
            await _streamingService.PlayMusic(_audioClient, music,_streamCancelToken.Token,guild);
        }
        
        /// <summary>
        /// Method called in order to add a new content in the streaming queue
        /// </summary>
        /// <param name="content">the content we want to add</param>
        /// <returns>the info about the content we added to the streaming queue</returns>
        private QueuedItemInfo AddMusicToQueue(VideoSearchResult content)
        {
            _streamQueue.Add(content);
            return new QueuedItemInfo(_streamQueue.Count, content);

        }
        
        /// <summary>
        /// ConsumeMusic will dequeue the first item of the streaming queue and return it
        /// </summary>
        /// <param name="guild">the guild where we want to play a music</param>
        /// <returns>The content we want to hear</returns>
        /// <exception cref="DataException">if the streaming queue is already clear, throw an exception</exception>
        private VideoSearchResult ConsumeMusic(IGuild guild)
        {
            if (_streamQueue.Count == 0)
                throw new DataException("streamingQueue for guild" + guild.Name +
                                        "is empty, but consume music has been called");

            VideoSearchResult video = _streamQueue[0];
            _streamQueue.RemoveAt(0);
            return video;
        }
        /// <summary>
        /// Method that is trigger each interval of _disconnectTimer
        /// Check if we are playing a music and if not, then the trigger of this method
        /// represent the fact that we didn't had any activity for a specified time
        /// So we clear and disconnect the bot from the voice channel
        /// </summary>
        /// <param name="state">null state required by Timer Constructor</param>
        private void OnTimerElapsed(object state)
        {
            if (!_isPlaying)
            {
                //we cannot make it await because timer that triggers OnTimerElasped doesn't support Async method
                #pragma warning disable 4014
                ClearMusicProvider(true); //async not awaited call
                #pragma warning restore 4014
                _disconnectTimer.Change(infiniteWaitingTime, infiniteWaitingTime);
            }
        }
        
        
        
    }
}