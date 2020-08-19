using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class ShedParseException:System.Exception
    {
        public ShedParseException(string inputString):base(inputString)
        {

        }
    }
}
