using YoutubeExplode.Search;

namespace Synchro.Models
{
    public class QueuedItemInfo
    {
        //the position in the queue
        public int Position { get; set; }
        
        //General info about the video (Title author duration url...)
        public VideoSearchResult Result { get; set; }

        public QueuedItemInfo(int position, VideoSearchResult result)
        {
            Position = position;
            Result = result;
        }
    }
}