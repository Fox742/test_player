using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VideoPlayerEngine
{
    class Shedule
    {
        private List<BaseEvent> _events;

        /// <summary>
        /// Определение типа события
        /// </summary>
        /// <param name="eventToCheck"></param>
        /// <returns></returns>
        public static bool isBackroundEvent(BaseEvent eventToCheck)
        {
            if (eventToCheck is BackgroundEvent)
                return true;
            return false;
        }

        /// <summary>
        /// Найти следующее событие типа Background в списке, начиная с позиции startIndex
        /// </summary>
        /// <param name="eventsToCheck"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private int nextBackground(List<BaseEvent> eventsToCheck, int startIndex = 0)
        {
            int result = -1;
            int current = startIndex;
            while (current<eventsToCheck.Count)
            {
                if (isBackroundEvent(eventsToCheck[current]))
                    break;
                current++;
            }

            if (current < eventsToCheck.Count)
                result = current;

            return result;
        }

        /// <summary>
        /// Проверка не налезают ли события Background друг на друга
        ///     Поскольку у нас события отсортированы в порядке их начала, то нам нужно пробежатся по списку событий и проверить не залезает ли конец предыдущего события на начало следующего
        ///     
        /// </summary>
        /// <param name="eventsToCheck"></param>
        private void checkShedule(List<BaseEvent> eventsToCheck)
        {
            // Вычисляем первое событие с типом Background
            int firstPosition = nextBackground(eventsToCheck);
            if (firstPosition >= 0)
            {
                // Вычисляем следующее событие с типом Background
                int secondPosition = nextBackground(eventsToCheck, firstPosition + 1);
                while (secondPosition >= 0)
                {
                    // Выбираем по индексу первое и второе событие из списка
                    BackgroundEvent firstEvent = (BackgroundEvent)eventsToCheck[firstPosition];
                    BackgroundEvent secondEvent = (BackgroundEvent)eventsToCheck[secondPosition];

                    // Сравниваем, что если время конца предыдущего события больше времени начала следущего - надо выбросить исключение
                    if (firstEvent.End>secondEvent.Begin)
                    {
                        throw new ShedOrderException(string.Format("Ошибка в файле расписания. События, продекларированные в строке:\n{0} (номер {1})\nи строке:\n{2} (номер {3})\nнакладываются друг на друга",
                            firstEvent.RawString,firstEvent.StringNumber,secondEvent.RawString,secondEvent.StringNumber
                            ));

                    }

                    // Перемещаемся к следующей паре событий типа Background
                    firstPosition = secondPosition;
                    secondPosition = nextBackground(eventsToCheck,secondPosition+1);

                }
            }
        }

        /// <summary>
        /// Конструктор расписание. Принимает путь к файлу из которого нужно прочитать расписание
        /// </summary>
        /// <param name="Filename"></param>
        public Shedule(string Filename)
        {
            List<BaseEvent> events = new List<BaseEvent>();

            // Прочитаем файл с расписанием
            List<string>rawStrings = new List<string>(File.ReadAllLines(Filename));
            EventsParser parser = new EventsParser();

            
            // Проходимся по строкам файла с расписанием и по каждой из строк - создаём событие
            for (int i=0; i<rawStrings.Count;i++)
            {
                string currentString = rawStrings[i].Trim();
                if (!string.IsNullOrEmpty(currentString))
                events.Add(parser.Parse(i, currentString));
            }

            // Сортируем список events по времени начала событий в независимости от его типа
            events.Sort((event1, event2) => ( event1.Begin.CompareTo(event2.Begin) ));

            // Надо проверить, что события Background не залезают друг на друга
            if (events.Count != 0)
            {
                checkShedule(events);
            }
            _events = events;
        }
        
        public long Length { get { return _events.Count; } }

        public BaseEvent this[int index]
        {
            get
            {
                return _events[index];
            }
        }

    }
}
