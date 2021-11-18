using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;

namespace Synchro.Services
{
    public class StreamingService
    {
        //Api client used to communicate with youtube content
        private readonly YoutubeClient _ytclient;

        private Task _player = null;

        public StreamingService()
        {
            _ytclient = new YoutubeClient();
        }
        
        /// <summary>
        /// Searches the 10 most correlated video content based on the info given
        /// </summary>
        /// <param name="info">the info we base our research on</param>
        /// <returns>the first content found</returns>
        public VideoSearchResult SearchContent(string info)
        {
            var content = _ytclient.Search.GetVideosAsync(info).CollectAsync(5).GetAwaiter().GetResult();
            
            //display the found research in stdout
            Console.WriteLine("--------------------");
            foreach (var searchResult in content)
            {
                Console.WriteLine("searchResult title: "+searchResult.Title);
            }
            Console.WriteLine("--------------------");

            return content.Count > 0 ? content[0]:null;
        }
        
        /// <summary>
        /// PlayMusic will copy a stream we create with an ffmpeg process from
        /// a downloaded file and flush into a discord stream
        /// </summary>
        /// <param name="audioClient">the audioclient of the guild we want to stream on</param>
        /// <param name="music">the info about the content we want to stream</param>
        /// <param name="cancellationToken">
        /// Token that is used to interrupt the ffmpeg process
        /// In case we want to skip the current music
        /// </param>
        /// <param name="guild">The guild where we stream the content</param>
        public async Task PlayMusic(IAudioClient audioClient, VideoSearchResult music,CancellationTokenSource cts,IGuild guild)
        {
            Console.WriteLine("Initializing streaming...");

            // we create a unique filename based on the guild that wants to play the music,
            // and the format of the music that we want to play
            string filename = $"{guild.Id}.mp3";
            
            DownloadMusic(filename,music.Url);
            
            Console.WriteLine("Finished download...");
            Console.WriteLine("Constructing player");
            _player =  Task.Run(async () =>
            {
                Console.WriteLine("Player constructed");

                Console.WriteLine("Creating ffmpeg process");
                //we create an audio stream through an ffmpeg process
                using var ffmpeg = CreateStream(filename).Result;
                Console.WriteLine("Created ffmpeg process !");
                await using var output = ffmpeg.StandardOutput.BaseStream;
                Console.WriteLine("Created output base stream");
                await using var discord = audioClient.CreatePCMStream(AudioApplication.Music);
                Console.WriteLine("Created discord pcm stream");
                try
                {
                    Console.WriteLine("Copying stream...");
                    //we copy the ffmpeg stream into the discord stream and base any interruption on a cancellation token
                    await output.CopyToAsync(discord, cts.Token);
                    Console.WriteLine("Finished to copy the output into discord stream");
                    Console.WriteLine("Flushing discord buffer normally");
                    await discord.FlushAsync();
                    output.Close();
                    ffmpeg.Close();
                    Console.WriteLine("Flushed !");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Flushing discord buffer");
                    await discord.FlushAsync();
                    output.Close();
                    ffmpeg.Close();
                    Console.WriteLine("Flushed discord buffer after Cancellation !");
                }
                Console.WriteLine("End of playing task");
            });

            await _player;
            cts.Dispose();
        }

        

        public void SkipCurrentMusic(CancellationTokenSource cts) => cts.Cancel();

        public void PauseMusic(IAudioClient audioClient)
        {
            _player.Start();
        }
        
        /// <summary>
        /// Method used to create a ffmpeg process that will create a stream from the downloaded content
        /// </summary>
        /// <param name="path">the path to the downloaded content</param>
        /// <returns>the ffmpeg process</returns>
        private async Task<Process> CreateStream(string path)
        {
            Task<Process> task = Task<Process>.Run(() =>
            {
                Process ffmpeg = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                });
                return ffmpeg;
            });

            return await task;
        }
        
        /// <summary>
        /// Method used to download the music based on its url and filename
        /// </summary>
        /// <param name="filename">the file name of the downloaded music</param>
        /// <param name="url">the url of the music we want to download</param>
        private void DownloadMusic(string filename, string url)
        {
            Process downloader = Process.Start( new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"downloader.py --filename={filename} --url={url}"
            });
            if (downloader == null)
                throw new ApplicationException("Synchro: No downloader.py found !");
            downloader.WaitForExit();
        }
        
    }
}