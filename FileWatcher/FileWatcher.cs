using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Mark.FileWatcher
{
    public class FileWatcher
    {
        private Timer _timer;
        private const int TimeoutMillis = 500;

        private FileSystemWatcher watcher;
        private WatcherChangeTypes changeType;
        private string changeFullPath;

        public FileWatcher(string fullpath)
        {
            watcher = new FileSystemWatcher();
            watcher.Filter = Path.GetFileName(fullpath);
            watcher.Path = Path.GetDirectoryName(fullpath);
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Changed += OnWatcherChanged;
            watcher.Created += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            watcher.Renamed += OnWatcherRenamed;
            watcher.EnableRaisingEvents = true;
            _timer = new Timer(new TimerCallback(OnTimerCallback), null, Timeout.Infinite, Timeout.Infinite);
        }

        void OnWatcherRenamed(object sender, RenamedEventArgs e)
        {
            changeType = e.ChangeType;
            changeFullPath = e.FullPath;
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            changeType = e.ChangeType;
            changeFullPath = e.FullPath;
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnTimerCallback(object state)
        {
            if (Changed != null) {
                var args = new FileSystemEventArgs(changeType, Path.GetDirectoryName(changeFullPath), Path.GetFileName(changeFullPath));
                Changed(this, args);
            }
        }

        public event EventHandler<FileSystemEventArgs> Changed;
    }
}
