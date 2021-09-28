using System;
using System.Diagnostics;
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
        private YoutubeClient _ytclient;

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
            var content = _ytclient.Search.GetVideosAsync(info).CollectAsync(10).GetAwaiter().GetResult();
            
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
        public async Task PlayMusic(IAudioClient audioClient, VideoSearchResult music,CancellationToken cancellationToken,IGuild guild)
        {
            Console.WriteLine("Initializing streaming...");
            
            //we search the stream manifest through the id of the music we want to stream
            var streamManifest = await _ytclient.Videos.Streams.GetManifestAsync(music.Id);
            //we get the audio-only stream info used to download the music
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            
            //some informations about the stream...
            Console.WriteLine("Size of stream: " + streamInfo.Size.KiloBytes + "KB");
            Console.WriteLine("Bitrate: " + streamInfo.Bitrate);
            
            //we download the content into a specified format we have got with the stream info
            await _ytclient.Videos.Streams.DownloadAsync(streamInfo, $"{guild.Id}.{streamInfo.Container}");
            Console.WriteLine("Finished download...");
            
            //we create an audio stream through an ffmpeg process
            using (var ffmpeg = CreateStream($"{guild.Id}.{streamInfo.Container}"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = audioClient.CreatePCMStream(AudioApplication.Music))
            {
                Console.WriteLine("Flushing stream...");
                try
                {
                    //we copy the ffmpeg stream into the discord stream and base any interruption on a cancellation token
                    await output.CopyToAsync(discord, cancellationToken);
                    Console.WriteLine("Finished to copy the output into discord stream");
                }
                catch (TaskCanceledException tce)
                {
                    
                    Console.WriteLine("cancelling music..." +  tce.Source);
                }
                finally
                {
                    Console.WriteLine("Flushing discord buffer");
                    await discord.FlushAsync();
                }
            }

        }
        /// <summary>
        /// Method used to create a ffmpeg process that will create a stream from the downloaded content
        /// </summary>
        /// <param name="path">the path to the downloaded content</param>
        /// <returns>the ffmpeg process</returns>
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        
        
    }
}