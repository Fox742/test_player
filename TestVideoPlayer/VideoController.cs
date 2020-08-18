using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Класс, управляющий воспроизведением видео по расписанию. Не управляет напрямую самим проигрываетелем (ТОЛЬКО посредством InterfaceWrapper)
    /// </summary>
    class VideoController
    {
        /// <summary>
        /// Флаг, который сигнализирует о смене расписания. При взведённом флаге контроллер должен работать немного по-другому нежели в "Штатном режиме"
        /// </summary>
        private bool changingShedule = false;

        private FutureEventsList CurrentFEList = null;
        private FutureEventsList CandidateFEList = null;
        private Mutex candidateLock = new Mutex();


        public VideoController()
        {
            InterfaceWrapper.VideoEndEvent += OnVideoEnd;
        }

        public void LoadShedule(Shedule shedule)
        {
            FutureEventsList newList = new FutureEventsList(shedule);


            candidateLock.WaitOne();
            changingShedule = true;
            CandidateFEList = newList;
            candidateLock.ReleaseMutex();

        }

        private void OnVideoEnd()
        {

        }

        private void OnFELEvent()
        {

        }

    }
}
