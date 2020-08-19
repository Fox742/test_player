using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    class ShedOrderException:System.Exception
    {
        public ShedOrderException(string inputString):base(inputString)
        {

        }

    }
}
