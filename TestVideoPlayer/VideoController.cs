using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Класс, управляющий воспроизведением видео по расписанию. Не управляет напрямую самим проигрываетелем (ТОЛЬКО посредством InterfaceWrapper)
    /// </summary>
    class VideoController
    {
        public VideoController()
        {

        }

        public void LoadShedule(Shedule shedule)
        {
            InterfaceWrapper.VideoEndEvent += OnVideoEnd;
        }

        private void OnVideoEnd()
        {

        }

    }
}
