using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Обёртка интерфейса, через которую выводит информацию движок
    /// </summary>
    static class InterfaceWrapper
    {

        public static event BaseInterfaceWrapper.VideoEndEventHandler VideoEndEvent;

        private static BaseInterfaceWrapper _uiWrapper = null;
        public static void initInterface(BaseInterfaceWrapper uiWrapper)
        {
            _uiWrapper = uiWrapper;
            _uiWrapper.VideoEndEvent += OnVideoEnd;
        }


        /// <summary>
        /// Вывести сообщение на экран
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public static void showMessage(string message, string caption = "")
        {
            if (_uiWrapper!=null)
            {
                _uiWrapper.PrintMessage(message,caption);
            }
        }

        private static void OnVideoEnd()
        {
            VideoEndEvent();
        }

        /// <summary>
        /// Запустить видео
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        public static void startVideo(string path, double position = 0.0)
        {
            if (_uiWrapper != null)
            {
                _uiWrapper.startVideo(path, position);
            }
        }

        internal static void stopVideo()
        {
            if (_uiWrapper != null)
            {
                _uiWrapper.stopVideo();
            }
        }

        /// <summary>
        /// Получить текущую позицию в видеопроигрывателе
        /// </summary>
        /// <returns></returns>
        public static double getCurrentPosition()
        {
            double result = 0;
            if (_uiWrapper != null)
            {
                return _uiWrapper.getPosition();
            }
            return result;
        }

        public static void printPlayList(List<string>items, int selectIndex)
        {
            if (_uiWrapper != null)
            {
                _uiWrapper.printPlayList(items,selectIndex);
            }
        }

        public static void refreshShedulePath(string shedulePath)
        {
            if (_uiWrapper != null)
            {
                _uiWrapper.refreshShedulePath(shedulePath);
            }
        }

    }
}
