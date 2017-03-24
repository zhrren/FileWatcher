using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Mark.FileWatcher
{
    public class FileWatcherManager
    {
        private static Dictionary<FileWatcher, List<Action<FileSystemEventArgs>>>
            _watcher = new Dictionary<FileWatcher, List<Action<FileSystemEventArgs>>>();

        public static void Watch(string fullpath, Action<FileSystemEventArgs> callback, bool immediate = false)
        {
            fullpath = Path.GetFullPath(fullpath);
            FileWatcher watcher;
            if (!_watcher.Keys.Select(x => x.Fullpath).Contains(fullpath))
            {
                watcher = new FileWatcher(fullpath);
                watcher.Changed += Watcher_Changed;
                _watcher[watcher] = new List<Action<FileSystemEventArgs>>();
            }

            watcher = _watcher.Keys.FirstOrDefault(x => x.Fullpath == fullpath);
            var watcherActions = _watcher[watcher];

            if (!watcherActions.Contains(callback))
            {
                _watcher[watcher].Add(callback);
            }

            if (immediate)
                callback(new FileSystemEventArgs(WatcherChangeTypes.All,
                    Path.GetDirectoryName(fullpath),
                    Path.GetFileName(fullpath)));
        }

        private static void Watcher_Changed(FileSystemEventArgs e)
        {
            var watcher = _watcher.Keys.FirstOrDefault(x => x.Fullpath == e.FullPath);
            if (watcher != null)
            {
                var actions = _watcher[watcher];
                actions.ToList().ForEach(x =>
                {
                    x(e);
                });
            }
        }

        public static void Dispose()
        {
            foreach(var watcherEntry in _watcher.ToList())
            {
                watcherEntry.Key.Changed -= Watcher_Changed;
                watcherEntry.Key.Dispose();
            }
            _watcher.Clear();
        }
    }
}
