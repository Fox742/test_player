using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestVideoPlayer
{
    class PlayList
    {
        private bool _ringPlayback;

        public PlayList(string folderWithVideo, bool ringPlayback = false)
        {
            _ringPlayback = ringPlayback;
            
            
        }
    }
}
