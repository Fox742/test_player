using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace VideoPlayerEngine
{
    class EventsParser
    {

        private readonly string backgroundStr = "Background";
        private readonly string interruptStr = "Interrupt";

        private string getStringMessage(long stringNumber, string rawString)
        {
            return string.Format("Ошибка в файле расписания в строке <<{0}>> номер: {1}", rawString, stringNumber);
        }

        public virtual BaseEvent Parse( long stringNumber, string fileString )
        {
            // Разрезаем строку на подстроки
            BaseEvent result = null;
            List<string> substrings = new List<string>(Regex.Split(fileString, @"\s+").Where(s => s != string.Empty));
            string preambula = getStringMessage(stringNumber, fileString);

            // Должно быть 3 или 4 аргумента
            if (substrings.Count!=3 && substrings.Count != 4)
            {
                throw new ShedParseException( preambula + " не хватает аргументов");
            }

            // Проверяем тип события - должно быть Background или Interrupt
            if (substrings[0]!= backgroundStr && substrings[0] != interruptStr)
            {
                throw new ShedParseException(preambula + " первым аргументом должен стоять тип Backround или Interrupt");
            }

            DateTime beginEvent;
            if (!DateTime.TryParseExact(substrings[1], "HH:mm", null, System.Globalization.DateTimeStyles.None, out beginEvent))
            {
                throw new ShedParseException(preambula + " не удалось разобрать время начала события");
            }

            string Path = "";
            if (substrings[0] == backgroundStr)
            {
                DateTime endEvent;
                if (!DateTime.TryParseExact(substrings[2], "HH:mm", null, System.Globalization.DateTimeStyles.None, out endEvent))
                {
                    throw new ShedParseException(preambula + " не удалось разобрать время конца события");
                }

                if (beginEvent > endEvent)
                {
                    throw new ShedParseException(preambula + " время начала события Background не может быть больше времени конца");
                }

                if (substrings.Count < 4)
                {
                    throw new ShedParseException(preambula + " не хватает последнего аргумента - пути к папке");
                }
                
                Path = substrings[3];
                result = new BackgroundEvent(beginEvent, stringNumber, fileString, Path, endEvent);
            }
            else
            {
                Path = substrings[2];
                result = new InterruptEvent(beginEvent, stringNumber, fileString, Path);
            }
            return result;
        }
    }
}
