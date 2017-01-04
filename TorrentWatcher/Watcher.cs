using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TorrentWatcher
{
	public class Watcher
	{
		private List<BackgroundWorker> _workers = new List<BackgroundWorker> ();
		private BackgroundWorker _consoleReader = new BackgroundWorker();
		private bool _completed=false;

		void ConsoleReader_DoWork (object sender, DoWorkEventArgs e)
		{
			while (true) {
				ConsoleKeyInfo key = Console.ReadKey ();
				if (key.Key==ConsoleKey.C )
				{ 
					return;
				}
			}
		}

		void ConsoleReader_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			_console.Write ("Work has been canceled.");
		}

		BackgroundWorker AddWatch (TorrentTarget idleItem)
		{
			throw new NotImplementedException ();
		}

		public void Start ()
		{
			_console.Debug ("Watcher started");
			_consoleReader.WorkerSupportsCancellation = true;
			_consoleReader.DoWork += new DoWorkEventHandler (ConsoleReader_DoWork);
			_consoleReader.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ConsoleReader_WorkCompleted);
			_consoleReader.RunWorkerAsync ();

			while (_consoleReader.IsBusy) 
			{
				_reader.ProcessQueue ();
				// create and start watcher for each item
				foreach (TorrentTarget idleItem in _reader.IdleItems()) {
					_workers.Add(AddWatch (idleItem));
				}
				Thread.Sleep (1000);
			}
			_reader.SaveIncompleted ();
		}

		private MyConsole _console;
		private ITargetReader _reader;
		public Watcher (MyConsole console, ITargetReader reader)
		{
			_console = console;
			_reader = reader;
			_console.DebugOn = true;
			_console.Write ("Torrent trecker started.");
			_console.Debug ("Debug mode is on.");
		}
	}
}

