using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mark.FileWatcher.Demo
{
    public partial class Form1 : Form
    {
        private FileWatcher watcher;

        public Form1()
        {
            InitializeComponent();

            //watcher = new FileWatcher(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "data.txt"));
            //watcher = new FileWatcher("d:/data.txt");
            //watcher.Changed += Watcher_Changed;

            FileWatcherManager.Watch("aaa", WatchHandler);
            FileWatcherManager.Watch("aaa", WatchHandler2);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
                Console.WriteLine(File.ReadAllText(e.FullPath));
        }

        private void WatchHandler(FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
                Console.WriteLine(File.ReadAllText(e.FullPath));
        }
        private void WatchHandler2(FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
                Console.WriteLine(File.ReadAllText(e.FullPath));
        }
    }
}
