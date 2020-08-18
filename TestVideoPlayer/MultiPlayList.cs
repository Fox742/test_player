using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class MultiPlayList
    {

        public MultiPlayList()
        {
            InterfaceWrapper.VideoEndEvent += OnVideoEnd;
        }

        private void OnVideoEnd()
        {

        }

        public void resetPlayLists()
        {

        }

        public bool busy()
        {
            return false;
        }

        public void HandleEvent(FutureEventsList.FutureEvent fe)
        {

        }
    }
}
