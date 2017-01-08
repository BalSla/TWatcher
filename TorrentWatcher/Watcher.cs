using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using CsQuery;
using System.Net;
using System.Web;
using System.IO;
using TorrentWatcher.Parsers;

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
			_console.Write ("Canceling work...");
			// stop _workers
			foreach (var item in _workers) {
				item.CancelAsync ();
			}
			_console.Write ("Work finished.");
		}

		void TorrentBackgroundWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			((TorrentBackgroundWorker)sender).DoPersonalWork ();
		}

		void TorrentBackgroundWorker_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			throw new NotImplementedException ();
		}

		TorrentBackgroundWorker AddWatch (TorrentTarget idleItem)
		{
			TorrentBackgroundWorker worker = new TorrentBackgroundWorker (idleItem, _console, _parserManager);
			worker.DoWork += new DoWorkEventHandler (TorrentBackgroundWorker_DoWork);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler (TorrentBackgroundWorker_WorkCompleted);
			worker.RunWorkerAsync ();
			return worker;
		}

		IParsersManager _parserManager;

		public void Start ()
		{
			_console.Write ("Starting TorrentWatcher...");
			_parserManager = new ParsersManager (new KrutorParser());
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

