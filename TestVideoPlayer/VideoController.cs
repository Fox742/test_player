using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;


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
        private MultiPlayList MPlaylist;
        private System.Timers.Timer _FEtimer;

        public VideoController()
        {
            MPlaylist = new MultiPlayList();
            _FEtimer = new System.Timers.Timer();
            _FEtimer.AutoReset = true;
            _FEtimer.Elapsed += OnFELEvent;
        }

        /// <summary>
        /// Загрузить новое расписание
        /// </summary>
        /// <param name="shedule"></param>
        public void LoadShedule(Shedule shedule)
        {
            // Создаём новый список будущих событий
            FutureEventsList newList = new FutureEventsList(shedule);

            // Для доступа к кандидату на список будущих событие - нужен мьютекс, так как к кандидату мы имеем доступ из многих потоков (события)
            candidateLock.WaitOne();
            changingShedule = true;
            MPlaylist.resetPlayLists();
            CandidateFEList = newList;
            candidateLock.ReleaseMutex();

            // Если плейлист ничего не проигрывает - вызываем функцию запуска расписания
            if (!MPlaylist.busy())
            {
                launchPlayback();
            }

        }

        private void launchPlayback()
        {
            candidateLock.WaitOne();
            _FEtimer.Enabled = false;

            CurrentFEList = CandidateFEList;
            changingShedule = false;
            candidateLock.ReleaseMutex();
            FEEventInternal(true);
        }

        /// <summary>
        /// Функция обработки наступления будущего события. Может запускаться из обработчика события по таймеру, но может вызываться, когда события нет - а нужно
        /// инициализировать первое событие из списка
        /// </summary>
        /// <param name="isFake">Нужно ли обрабатывать наступившее событие</param>
        private void FEEventInternal(bool isFake = false)
        {
            FutureEventsList.FutureEvent lastFE = null;
            candidateLock.WaitOne();
            if (!isFake)
            {
                // Нужно обработать событие
                FutureEventsList.FutureEvent fe = CurrentFEList.current();
                
                if ((changingShedule && fe.path == null) || (!changingShedule)) // (Если меняется расписание - обрабатываем события о завершении Background-проигрывания)
                {
                    MPlaylist.HandleEvent(fe);
                }
            }

            // Устанавливаем будущее событие
            FutureEventsList.FutureEvent nextEvent = CurrentFEList.next();
            _FEtimer.Interval = (nextEvent.eventTime - DateTime.Now).TotalMilliseconds;
            _FEtimer.Start();

            // Нужно обработать событие типа background, если оно было до DateTime.Now и не закончилось к моменту начала запуска расписания
            if (isFake)
            {
                if ((CurrentFEList.LastBackgroundEvent != null) && (CurrentFEList.LastBackgroundEvent.path != null))
                {
                    MPlaylist.HandleEvent(CurrentFEList.LastBackgroundEvent);
                }
            }

            candidateLock.ReleaseMutex();
        }

        private void OnFELEvent(object sender, ElapsedEventArgs e)
        {
            FEEventInternal();
        }

    }
}
