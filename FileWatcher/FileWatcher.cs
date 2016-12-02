using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Mark.FileWatcher
{
    public class FileWatcher: IDisposable
    {
        private Timer _timer;
        private const int TimeoutMillis = 500;

        private FileSystemWatcher _watcher;
        private WatcherChangeTypes _changeType;
        private string _changeFullPath;
        private event EventHandler<FileSystemEventArgs> _changed;
        private bool _immediate;

        public string Fullpath { get; private set; }

        public FileWatcher(string fullpath, bool immediate = false)
        {
            Fullpath = fullpath;
            _immediate = immediate;

            _watcher = new FileSystemWatcher();
            _watcher.Filter = Path.GetFileName(fullpath);
            _watcher.Path = Path.GetDirectoryName(fullpath);
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            _watcher.Changed += OnWatcherChanged;
            _watcher.Created += OnWatcherChanged;
            _watcher.Deleted += OnWatcherChanged;
            _watcher.Renamed += OnWatcherRenamed;
            _watcher.EnableRaisingEvents = true;
            _timer = new Timer(new TimerCallback(OnTimerCallback), null, Timeout.Infinite, Timeout.Infinite);
        }

        void OnWatcherRenamed(object sender, RenamedEventArgs e)
        {
            _changeType = e.ChangeType;
            _changeFullPath = e.FullPath;
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            _changeType = e.ChangeType;
            _changeFullPath = e.FullPath;
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnTimerCallback(object state)
        {
            if (_changed != null) {
                var args = new FileSystemEventArgs(_changeType, Path.GetDirectoryName(_changeFullPath), Path.GetFileName(_changeFullPath));
                _changed(this, args);
            }
        }

        public void Dispose()
        {
            _watcher.Changed -= OnWatcherChanged;
            _watcher.Created -= OnWatcherChanged;
            _watcher.Deleted -= OnWatcherChanged;
            _watcher.Renamed -= OnWatcherRenamed;
            _watcher.Dispose();
            _timer.Dispose();
        }

        public event EventHandler<FileSystemEventArgs> Changed
        {
            add
            {
                _changed += value;

                if (_immediate)
                {
                    _changeType = WatcherChangeTypes.All;
                    var args = new FileSystemEventArgs(_changeType, Path.GetDirectoryName(_changeFullPath), Path.GetFileName(_changeFullPath));
                    _changed(this, args);
                }
            }
            remove
            {
                _changed -= value;
            }
        }
    }
}
