using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mark.FileWatcher
{
    public class FileWatcherManager
    {
        private static Dictionary<string, List<Action<FileSystemEventArgs>>> _watcher = new Dictionary<string, List<Action<FileSystemEventArgs>>>();

        public static void Watch(string fullpath, Action<FileSystemEventArgs> action)
        {
            fullpath = Path.GetFullPath(fullpath);
            if (!_watcher.ContainsKey(fullpath))
            {
                FileWatcher watcher = new FileWatcher(fullpath);
                watcher.Changed += Watcher_Changed;
                _watcher[fullpath] = new List<Action<FileSystemEventArgs>>();
            }

            if (!_watcher[fullpath].Contains(action))
            {
                _watcher[fullpath].Add(action);
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_watcher.ContainsKey(e.FullPath))
            {
                var actions = _watcher[e.FullPath];
                actions.ForEach(x =>
                {
                    x(e);
                });
            }
        }
    }
}
