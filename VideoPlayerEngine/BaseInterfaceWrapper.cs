using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    public abstract class BaseInterfaceWrapper
    {
        /// <summary>
        /// Делегат события "завершилось проигрывание видео"
        /// </summary>
        public delegate void VideoEndEventHandler();

        /// <summary>
        /// Событие завершилось проигрывание очередного видеофайлы
        /// </summary>
        public event VideoEndEventHandler VideoEndEvent;

        /// <summary>
        /// Поднять событие о завершении проигрывания очередного видеофайлы
        /// </summary>
        protected void RiseVideoEndEvent()
        {
            VideoEndEvent();
        }

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Caption"></param>
        public abstract void PrintMessage(string message, string Caption);

        /// <summary>
        /// Запустить проигрывание видео
        /// </summary>
        /// <param name="path">Путь к видеофайлу</param>
        /// <param name="position">С какой по счёту секунды начинаем проигрывать видео?</param>
        public abstract void startVideo(string path, double position);

        /// <summary>
        /// Остановить видео
        /// </summary>
        public abstract void stopVideo();

        /// <summary>
        /// Получить текущую позицию в видео, которое сейчас проигрывается
        /// </summary>
        /// <returns></returns>
        public abstract double getPosition();

        /// <summary>
        /// Распечатать плейлист в интерфейсе
        /// </summary>
        /// <param name="items"></param>
        /// <param name="selectIndex"></param>
        public abstract void printPlayList(List<string> items, int selectIndex);

        /// <summary>
        /// Вывести в label путь к вфбранному файлу расписания
        /// </summary>
        /// <param name="shecdulePath"></param>
        public abstract void refreshShedulePath(string shecdulePath);
    }
}
