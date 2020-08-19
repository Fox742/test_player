using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Класс для хранения списка будущих событий
    /// </summary>
    class FutureEventsList
    {
        /// <summary>
        /// Класс для элемента списка будущих событий
        /// </summary>
        public class FutureEvent
        {
            public readonly DateTime eventTime;
            public readonly bool ringPlayback;
            public readonly string path;

            public FutureEvent(DateTime EventTime, string Path = null ,bool RingPlayback = false)
            {
                eventTime = EventTime;
                path = Path;
                ringPlayback = RingPlayback;
            }
        }

        private List<FutureEvent> _futureEvents;

        private FutureEvent _current = null;

        private FutureEvent _lastBackgroundEvent;

        public FutureEvent LastBackgroundEvent
        {
            get
            {
                return _lastBackgroundEvent;
            }
        }

        /// <summary>
        /// Создаём список будущих событий по расписанию
        /// 
        ///     В списке будущих событий помимо событий Interrupt и Background будет третий тип событий - событие окончания background. 
        ///     Время наступления этого событие - это конечное время из события Background
        /// </summary>
        /// <param name="shedule"></param>
        public FutureEventsList(Shedule shedule)
        {
            _futureEvents = new List<FutureEvent>();

            bool isBackgroundPeriod = false;
            DateTime backgroundEnd = new DateTime(0);

            // Идём по расписанию
            for (int i=0;i<shedule.Length;i++)
            {
                // Проверим - а не нужно ли перед событием типа Interrupt добавить событие окончания Background
                if (isBackgroundPeriod && (backgroundEnd < shedule[i].Begin))
                {
                    _futureEvents.Add(new FutureEvent(backgroundEnd));
                    isBackgroundPeriod = false;
                }
                // Событие в расписании может быть двух типов
                if (Shedule.isBackroundEvent(shedule[i]))
                {
                    BackgroundEvent element = (BackgroundEvent)shedule[i];

                    // Возводим флаг, что у нас есть событие типа Background и запоминаем время его окончания (так как мы тоже будет доджны его лобавить в список будущих событий)
                    isBackgroundPeriod = true;
                    backgroundEnd = element.End;

                    _futureEvents.Add( new FutureEvent(element.Begin,element.Path,true) );

                }
                else
                {
                    InterruptEvent element = (InterruptEvent)shedule[i];
                    _futureEvents.Add(new FutureEvent( element.Begin,element.Path ));

                }
            }
            // Если началось событие background и не закончилось
            if (isBackgroundPeriod)
            {
                _futureEvents.Add(new FutureEvent(backgroundEnd));
            }

        }

        public FutureEvent next()
        {
            DateTime currentDT = DateTime.Now;
            FutureEvent result = null;

            foreach (FutureEvent fe in _futureEvents)
            {
                if (fe.eventTime > currentDT)
                {
                    result = fe;
                    break;
                }

                // Запоминаем последнее событие типа background, которое либо начинает background, либо - заканчивает
                if (fe.ringPlayback || fe.path == null)
                {
                    _lastBackgroundEvent = fe;
                }
            }
            _current = result;
            return result;
        }
        
        public FutureEvent current()
        {
            return _current;
        }

    }
}
