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
            
            //we search the stream manifest through the id of the music we want to stream
            var streamManifest = await _ytclient.Videos.Streams.GetManifestAsync(music.Id);
            //we get the audio-only stream info used to download the music
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            
            //some informations about the stream...
            Console.WriteLine("Size of stream: " + streamInfo.Size.KiloBytes + "KB");
            Console.WriteLine("Bitrate: " + streamInfo.Bitrate);

            Progress<double> downloadProgress = new Progress<double>();
            downloadProgress.ProgressChanged += DownloadProgressChanged;

            //we download the content into a specified format we have got with the stream info
            await _ytclient.Videos.Streams.DownloadAsync(streamInfo, $"{guild.Id}.{streamInfo.Container}", progress: downloadProgress);
            Console.WriteLine("Finished download...");

            _player =  Task.Run(async () =>
            {
                CancellationToken ct = cts.Token;
                
                ct.ThrowIfCancellationRequested();

                //we create an audio stream through an ffmpeg process
                using var ffmpeg = CreateStream($"{guild.Id}.{streamInfo.Container}");
                Console.WriteLine("Created ffmpeg stream");
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
                }
                finally
                {
                    Console.WriteLine("Flushing discord buffer");
                    await discord.FlushAsync(ct);
                    Thread.Sleep(2000);
                    Console.WriteLine("Finished sleeping...");
                }
                Console.WriteLine("End of playing task");
            },cts.Token);

            try
            {
                await _player;
            }
            catch (OperationCanceledException)
            {
                
            }
            finally
            {
                cts.Dispose();
            }
        }

        

        public void SkipCurrentMusic(CancellationTokenSource cts) => cts.Cancel();


        /// <summary>
        /// Method used to create a ffmpeg process that will create a stream from the downloaded content
        /// </summary>
        /// <param name="path">the path to the downloaded content</param>
        /// <returns>the ffmpeg process</returns>
        private Process CreateStream(string path)
        {
            Process ffmpeg =  Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            Console.WriteLine("Created ffmpeg process");
            return ffmpeg;
        }

        private void DownloadProgressChanged(object sender, double value)
        {
            for (int i = 0; i < (int)(value*100)/10; i++)
                Console.Write("#");
            Console.WriteLine(" : " + (int)(value*100) + " %");
            
        }
    }
}