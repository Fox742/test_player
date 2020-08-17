using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class InterruptEvent:BaseEvent
    {
        public InterruptEvent(DateTime begin, long stringNumber, string rawString, string path):base(begin,stringNumber,rawString,path)
        {

        }
    }
}
