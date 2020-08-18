using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Иерархический плейлист, который хранит информацию о всех видео должны ещё проиграться. 
    /// На вход получает события из списка будущих событий. Управляет проигрываетелем в интерфейсе
    /// </summary>
    class MultiPlayList
    {

        private PlayList _background = null;
        private List<PlayList> _interrupted;
        private Mutex PlaylistLock = new Mutex();

        public MultiPlayList()
        {
            _interrupted = new List<PlayList>();
            InterfaceWrapper.VideoEndEvent += OnVideoEnd;
        }

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

        private void OnVideoEnd()
        {
            PlaylistLock.WaitOne();
            if (_interrupted.Count>0)
            {
                _interrupted[_interrupted.Count - 1].stepNext();
            }
            else
            {
                if (_background!=null)
                {
                    _background.stepNext();
                }
            }

            changeVideo();
            printCurrentPlaylist();
            PlaylistLock.ReleaseMutex();
        }

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
                if ( _interrupted.Count==0 ) // Нужно запустить бекграунд-видео
                {
                    InterfaceWrapper.startVideo(_background.currentFile,_background.currentPosition);
                }
                else if (_interrupted.Count == 1)   // Происходит прерывание воспроизведения бекграунд-файла. Значит, надо остановить видео, 
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
                    InterfaceWrapper.startVideo(_interrupted[_interrupted.Count-1].currentFile, _interrupted[_interrupted.Count - 1].currentPosition);
                }
            }
            else // Закончилось предыдущее видео - нужно запустить следующее видео (если оно есть)
            {
                if (_interrupted.Count>0) // Если есть интерраптед плейлисты
                {
                    while (_interrupted[_interrupted.Count-1].played()) // Выбрасываем interrupted-плейлисты, которые мы уже проиграли
                    {
                        _interrupted.RemoveAt(_interrupted.Count - 1);
                        if (_interrupted.Count == 0)
                            break;
                    }
                    // Если в interrupted остался хотя бы один плейлист из которого ещё что-то можно проиграть - запускаем
                    if (_interrupted.Count>0)
                    {
                        InterfaceWrapper.startVideo(_interrupted[_interrupted.Count - 1].currentFile, _interrupted[_interrupted.Count - 1].currentPosition);
                        return;
                    }
                }
                // Все interrupted плейлистов ничего не запустили - надо посмотреть можно ли что-то запустить из background
                //if ((_background != null)&&(_background.stepNext())) // Можно, если беграунд-плейлист не пустой и у него есть следующий элемент, который можно запустить
                if (_background != null)
                {
                    InterfaceWrapper.startVideo(_background.currentFile, _background.currentPosition);
                }
                else
                {
                    _background = null; // Увы, но нет, из него нельзя ничего запустить тоже - поэтому зануляем его.
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
            if (fe.path==null) // Событие завершения проигрывания из Background
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
                _background = new PlayList(fe.path,fe.ringPlayback);
                if (_interrupted.Count==0)
                {
                    // Должны запустить _Background-плейлист
                    changeVideo(true);
                }

            }
            else // (fe.ringPlayback == false) - Событие interrupt
            {
                _interrupted.Add(new PlayList(fe.path,fe.ringPlayback));
                // Interrupted - запускаются всегда
                changeVideo(true);
            }
            printCurrentPlaylist();
            PlaylistLock.ReleaseMutex();

        }
    }
}
