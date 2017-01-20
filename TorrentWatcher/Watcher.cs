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
using System.Diagnostics;

namespace TorrentWatcher
{
	public class Watcher
	{
		public void Add (string item, string category)
		{
			File.WriteAllText (Path.GetRandomFileName()+".txt" ,item);
			_console.Write ("Created ticket for [{0}] ({1}).", item, category);
		}

		private List<TorrentBackgroundWorker> _workers = new List<TorrentBackgroundWorker> ();
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
			_console.Debug ("Console reader state(0) is busy={0}", _consoleReader.IsBusy);
			// stop _workers
			foreach (var item in _workers) {
				_console.Debug ("Stopping [{0}]...", item.Name);
				item.CancelAsync ();
			}
			bool allStopped = false;
			while (!allStopped) {
				allStopped = true;
				foreach (var item in _workers) {
					if (item.IsBusy) {
						_console.Debug ("[{0}] still active...", item.Name);
						allStopped=false;
					}
				}
				_console.Debug ("Console reader state(1) is busy={0}", _consoleReader.IsBusy);
				_console.Debug ("allStopped={0}", allStopped);
				Thread.Sleep (6000);
			}
			_reader.SaveIncompleted ();
			_console.Write ("Work finished.");
			//TODO: Publish statisctics
			_completed = true;
		}

		void TorrentBackgroundWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			((TorrentBackgroundWorker)sender).DoPersonalWork ();
		}

		void TorrentBackgroundWorker_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			//TODO: Publish new found links
			_console.Debug ("Torrent watcher [{0}] has completed work.", ((TorrentBackgroundWorker)sender).Name);
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
			IParser krutor = new KrutorParser ();
			_parserManager = new ParsersManager (krutor);
			//TODO: implement Kinozal.tv parser
			//TODO: implement lostfilm.tv parser
			_console.Debug ("Watcher started");
			_consoleReader.WorkerSupportsCancellation = true;
			_consoleReader.DoWork += new DoWorkEventHandler (ConsoleReader_DoWork);
			_consoleReader.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ConsoleReader_WorkCompleted);
			_consoleReader.RunWorkerAsync ();

			Stopwatch timer = null;
			while (_consoleReader.IsBusy || !_completed) 
			{
				if (timer == null || timer.ElapsedMilliseconds > 60000) {
					_reader.ProcessQueue ();
					// create and start watcher for each item
					timer = new Stopwatch ();
					timer.Start ();
					foreach (TorrentTarget idleItem in _reader.IdleItems()) {
						if (_workers.Find(x=>x.Name==idleItem.Name)==null) {
							TorrentBackgroundWorker newWorker = AddWatch (idleItem);
							_workers.Add (newWorker);
							_console.Debug ("Watcher [{0}] added to queue.", newWorker.Name);
						}
					}
				}
				//TODO: define sleep
				Thread.Sleep (1000);
			}
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

