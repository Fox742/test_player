using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VideoPlayerEngine
{
    class PlayList
    {
        public string currentFile
        {
            get
            {
                return _filesQueue[_currentFileNumber];
            }
        }

        public double currentPosition
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        private bool _ringPlayback;
        private List<string> _filesQueue;
        private int _currentFileNumber = 0;
        private double _position = 0;

        public PlayList(string folderWithVideo, bool ringPlayback = false)
        {
            _filesQueue = new List<string>();

            _ringPlayback = ringPlayback;
            _filesQueue = new List<string>(Directory.GetFiles(folderWithVideo));
            _filesQueue.Sort();

        }

        /// <summary>
        /// Удалить все файлы после текущего. Делается для того чтобы чтобы при изменении расписания проиграть только одно оставшееся видео во всех плейлистах
        /// </summary>
        public void reset()
        {
            _ringPlayback = false; // Если у нас бекграундный плейлист с _ringPlay=true мы тоже его обязаны остановить
            _filesQueue.RemoveRange(_currentFileNumber, _filesQueue.Count - _currentFileNumber);
        }

        public bool played()
        {
            return _currentFileNumber >= _filesQueue.Count;
        }

        public void stepNext()
        {
            _position = 0;
            _currentFileNumber++;
            if (_ringPlayback && played())
            {
                _currentFileNumber = 0;
            }

        }

    }
}
