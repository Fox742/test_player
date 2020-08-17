using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class BackgroundEvent:BaseEvent
    {
        private DateTime _end;
        public DateTime End { get { return _end; } }

        public BackgroundEvent(DateTime begin, long stringNumber, string rawString, string path, DateTime end):base(begin,stringNumber,rawString,path)
        {
            _end = end;
        }

    }
}
