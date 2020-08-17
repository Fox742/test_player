using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class BaseEvent
    {
        private DateTime _begin;
        private long _stringNumber;
        private string _rawString;
        private string _path;

        public DateTime Begin { get { return _begin; } }
        public long StringNumber { get { return _stringNumber; } }
        public string RawString { get { return _rawString; } }
        public string Path { get { return _path; } }

        public BaseEvent( DateTime begin, long stringNumber, string rawString, string path )
        {
            _begin = begin;
            _stringNumber = stringNumber;
            _rawString = rawString;
            _path = path;
        }

    }
}
