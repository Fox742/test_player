using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    public delegate void VideoPlaybackCompleted();

    /// <summary>
    /// Иерархический плейлист, который хранит информацию о всех видео должны ещё проиграться.
    /// На вход получает события из списка будущих событий. Управляет проигрываетелем в интерфейсе
    /// </summary>
    class MultiPlayList
    {

        /// <summary>
        /// Событие об окончании проигрывания всего расписания
        /// </summary>
        public event VideoPlaybackCompleted PlaybackCompleted;
        
        private PlayList _background = null;        // Плейлист бекграундных видео
        private List<PlayList> _interrupted;        // Список активных плейлистов interrupted
        private Mutex PlaylistLock = new Mutex();   // Мьютекс, защищающий плейлисты от одноворвменного доступа из обработчика события таймера и обработчика событий плеера

        public MultiPlayList()
        {
            _interrupted = new List<PlayList>();
            InterfaceWrapper.VideoEndEvent += OnVideoEnd; // Подписываемся на событие плеера о том, что текущее видео закончилось
        }

        /// <summary>
        /// Вывести в интерфейс информацию о текущем плейлисте
        /// </summary>
        private void printCurrentPlaylist()
        {
            int activeIndex = -1;
            List<string> Files = new List<string>();
            if (_interrupted.Count > 0)
            {
                activeIndex = _interrupted[_interrupted.Count - 1].currentFileNumber;
                Files = _interrupted[_interrupted.Count - 1].filenamesList;
            }
            else if (_background!=null)
            {
                activeIndex =   _background.currentFileNumber;
                Files =         _background.filenamesList;
            }
            InterfaceWrapper.printPlayList(Files,activeIndex);
        }

        /// <summary>
        /// Обработчик события завершения текущего видео
        /// </summary>
        private void OnVideoEnd()
        {
            PlaylistLock.WaitOne();

            // Так как видео закончилось - нам нужно перевинуть текущий плейлист на следующее видео (stepNext)
            if (_interrupted.Count>0)
            {
                _interrupted.Last().stepNext();
            }
            else
            {
                if (_background!=null)
                {
                    _background.stepNext();
                    if (_background.played())
                    {
                        _background = null;
                    }
                }
            }

            // Определяем и запускаем следующее видео
            changeVideo();

            // Печатаем текущий плейлист
            printCurrentPlaylist();

            PlaylistLock.ReleaseMutex();

            // Если никих плейлистов не осталось - это означает, что больше проигрывать нечего и сигнализируем об подписчикам
            if (_background==null && _interrupted.Count == 0)
            {
                PlaybackCompleted();
            }
        }

        /// <summary>
        /// Заресетить все плейлисты. В наших терминах "заресетить" - это удалить все видео, которые должны проигрываться после текущих.
        ///     Делается это в том случае, если происходит смена расписания на ходу, чтобы никакие видео кроме текущих не проигрывались больше.
        ///             (Текущие видео в данном случае - это видео, которое либо проигрывается, либо было прервано)
        /// </summary>
        public void resetPlayLists()
        {
            PlaylistLock.WaitOne();
            if (_background!=null)
            {
                _background.reset();
            }
            for (int i=0;i<_interrupted.Count;i++)
            {
                _interrupted[i].reset();
            }
            printCurrentPlaylist();
            PlaylistLock.ReleaseMutex();
        }

        /// <summary>
        /// Функция переключающая видео в плеере. Вызывается тогда, когда нужно либо запустить новый плейлист, либо по окончании воспроизведения видео (событие VideoEnd)
        /// </summary>
        /// <param name="up">Нужно ли запустить новый плейлист?</param>
        private void changeVideo(bool up = false)
        {
            if (up) // Нужно запустить только что открытый плейлист
            {
                if ( _interrupted.Count==0 ) // Нужно запустить бекграунд-видео (так как interrupted-плейлистов нет)
                {
                    InterfaceWrapper.startVideo(_background.currentFile,_background.currentPosition);
                }
                else if (_interrupted.Count == 1)   // Происходит прерывание воспроизведения бекграунд-плейлиста. Значит, надо остановить видео, 
                                                    //  сохранить позицию беграунд-плейлиста и запустить видео первого плейлиста из прерываний
                {
                    _background.currentPosition = InterfaceWrapper.getCurrentPosition();
                    InterfaceWrapper.stopVideo();
                    InterfaceWrapper.startVideo(_interrupted[0].currentFile, _interrupted[0].currentPosition);
                }
                else if (_interrupted.Count > 1)    // Происходит прерывания видеопотока прерывания :) Нужно сохранить позицию предыдущего потока прерывания и запустить новый
                {
                    _interrupted[_interrupted.Count-2].currentPosition = InterfaceWrapper.getCurrentPosition();
                    InterfaceWrapper.stopVideo();
                    InterfaceWrapper.startVideo(_interrupted.Last().currentFile, _interrupted.Last().currentPosition);
                }
            }
            else // Закончилось предыдущее видео - нужно запустить следующее видео (если оно есть)
            {
                if (_interrupted.Count>0) // Если есть интерраптед плейлисты
                {
                    while (_interrupted.Last().played()) // Выбрасываем interrupted-плейлисты, которые мы уже проиграли
                    {
                        _interrupted.RemoveAt(_interrupted.Count - 1);
                        if (_interrupted.Count == 0)
                            break;
                    }
                    // Если в interrupted остался хотя бы один плейлист из которого ещё что-то можно проиграть - запускаем
                    if (_interrupted.Count>0)
                    {
                        InterfaceWrapper.startVideo(_interrupted.Last().currentFile, _interrupted.Last().currentPosition);
                        return;
                    }
                }
                // Все interrupted плейлистов ничего не запустили - надо посмотреть можно ли что-то запустить из background
                if (_background != null)
                {
                    InterfaceWrapper.startVideo(_background.currentFile, _background.currentPosition);
                }
            }
        }

        /// <summary>
        /// Проигрывается ли что-то в плеере?
        /// </summary>
        /// <returns></returns>
        public bool busy()
        {
            return !(_background==null && _interrupted.Count == 0);
        }

        /// <summary>
        /// Обработать событие из списка будущих событий
        /// </summary>
        /// <param name="fe"></param>
        public void HandleEvent(FutureEventsList.FutureEvent fe)
        {
            PlaylistLock.WaitOne();
            if (fe.path == null) // Событие завершения проигрывания из Background
            {
                    // Если мы проигрываем Background - мы должны его остановить
                    if (_interrupted.Count == 0)
                    {
                        InterfaceWrapper.stopVideo();
                    }
                    _background = null;

            }
            else if (fe.ringPlayback) // Событие Background (для таких событие установлен флаг "кольцевое воспроизведение")
            {
                    _background = new PlayList(fe.path, fe.ringPlayback);
                    if (_interrupted.Count == 0)
                    {
                        // Должны запустить _Background-плейлист
                        changeVideo(true);
                    }

            }
            else // (fe.ringPlayback == false) - Событие interrupt
            {
                    _interrupted.Add(new PlayList(fe.path, fe.ringPlayback));
                    // Interrupted - запускаются всегда
                    changeVideo(true);
            }
            printCurrentPlaylist();
            PlaylistLock.ReleaseMutex();

        }
    }
}
