using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VideoPlayerEngine
{
    /// <summary>
    /// Класс для хранения плейлиста. Хранит список видеофайлов и текущее видео+количество секунд, которые были продемонстрированы
    /// </summary>
    class PlayList
    {
        /// <summary>
        /// Путь+имя текущего видеофайла
        /// </summary>
        public string currentFile       {     get{return _filesQueue[_currentFileNumber];}    }

        /// <summary>
        /// Номер файла, который проигрывается либо был прерван событием Interrupt
        /// </summary>
        public int currentFileNumber    {     get{return _currentFileNumber;}                 }

        /// <summary>
        /// Возвращает имена файлов из плейлиста
        /// </summary>
        public List<string> filenamesList
        {
            get
            {
                List<string> result = new List<string>();

                foreach (string onePath in _filesQueue)
                {
                    result.Add( filenamesByPath[onePath] );
                }

                return result;
            }
        }

        /// <summary>
        /// Возвращает и хранит количество секунд, проигранных в текущем видео
        /// </summary>
        public double currentPosition
        {
            get {return _position;}
            set{ _position = value;}
        }

        private bool _ringPlayback;     // Кольцевой проигрыш плейлиста (нужно для background-плейлистов)
        private List<string> _filesQueue;   // Список путей+имён файлов, которые нужно проиграть
        private int _currentFileNumber = 0; // Номер текущего видео в _filesQueue
        private double _position = 0;
        private Dictionary<string, string> filenamesByPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderWithVideo">Директория с видеофайлами</param>
        /// <param name="ringPlayback">Начинать ли сначала при достижении последнего видеофайла</param>
        public PlayList(string folderWithVideo, bool ringPlayback = false)
        {
            _filesQueue = new List<string>();
            filenamesByPath = new Dictionary<string, string>();

            _ringPlayback = ringPlayback;
            _filesQueue = new List<string>(Directory.GetFiles(folderWithVideo));
            _filesQueue.Sort();
            foreach( string filepath in _filesQueue )
            {
                FileInfo FI = new FileInfo(filepath);
                filenamesByPath[filepath] = FI.Name;
            }

        }

        /// <summary>
        /// Удалить все файлы после текущего. Делается для того чтобы чтобы при изменении расписания проиграть только одно оставшееся видео во всех плейлистах
        /// </summary>
        public void reset()
        {
            _ringPlayback = false; // Если у нас бекграундный плейлист с _ringPlay=true мы тоже его обязаны остановить
            _filesQueue.RemoveRange(_currentFileNumber+1, _filesQueue.Count - _currentFileNumber-1);
        }

        public bool played()
        {
            return _currentFileNumber >= _filesQueue.Count;
        }

        /// <summary>
        /// Сделать активным следущее видео в плейлисте. Если поднят флаг _ringPlayBack - то по достижении конца плейлиста, текущим становится первое видео
        /// </summary>
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
