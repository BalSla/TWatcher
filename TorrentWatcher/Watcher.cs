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
using System.Timers;

namespace TorrentWatcher
{
	public class Watcher
	{
		private MyConsole _console;
		private ITargetReader _reader;
		private int _linksFoundCount;
		private IPublisher _publisher;
		private bool _canceling;

		public Watcher (MyConsole console, ITargetReader reader, bool debug)
		{
			_console = console;
			_reader = reader;
			_console.DebugOn = debug;
			_publisher = new HtmlLinkPublisher ("links.html");
		}

		public void Remove (string remove)
		{
			File.WriteAllText (Path.GetRandomFileName()+".txt" ,string.Format("Completed:{0}",remove));
			_console.Write ("Created ticket to delete [{0}].", remove);
			_publisher.Remove (remove);
		}

		public void HideAllLinks ()
		{
			_publisher.HideAllLinks ();
		}

		public void Hide (string hide)
		{
			_publisher.Hide (hide);
		}

		public void Add (string item, string category)
		{
			File.WriteAllText (Path.GetRandomFileName()+".txt" ,item);
			_console.Write ("Created ticket for [{0}] ({1}).", item, category);
		}

		private List<TorrentBackgroundWorker> _workers = new List<TorrentBackgroundWorker> ();
		private BackgroundWorker _consoleReader = new BackgroundWorker();

		void ConsoleReader_DoWork (object sender, DoWorkEventArgs e)
		{
			while (true) {
				ConsoleKeyInfo key = Console.ReadKey ();
				if (key.Key==ConsoleKey.C)
				{ 
					ConsoleReader_WorkCompleted ();
					return;
				}
			}
		}

		void PublishStatistics ()
		{
			_console.Write("SESSION STATISTICS:");
			_console.Write ("  {0} item(s) added to watching list", _reader.AddedItems);
			_console.Write ("  {0} item(s) removed from watching list", _reader.AddedItems);
			_console.Write ("  {0} link(s) found", _linksFoundCount);
		}

		void ConsoleReader_WorkCompleted ()
		{
			_canceling = true;
			_console.Write ("Canceling work...");
			_console.Debug ("Console reader state(0) is busy={0}", _consoleReader.IsBusy);
			// stop _workers
			foreach (var item in _workers) {
				_console.Debug ("Stopping [{0}]...", item.Name);
				item.CancelAsync ();
			}
			bool allStopped = false;
		int stopCounter = 0;
			while (!allStopped) {
				allStopped = true;
				foreach (var item in _workers) {
					if (item.IsBusy) {
						_console.Debug ("[{0}] still active...", item.Name);
						if (stopCounter==10) {
							_console.Debug ("Stopping [{0}]...", item.Name);
							item.CancelAsync ();
					}
						if (stopCounter==20) {
							_console.Write ("Unfinished watchers. Exiting...");
							allStopped = true;
							break;
						}
						allStopped=false;
					}
				}
				stopCounter++;
				_console.Debug ("Console reader state(1) is busy={0}", _consoleReader.IsBusy);
				_console.Debug ("allStopped={0}", allStopped);
				Thread.Sleep (6000);
			}
			_reader.SaveIncompleted ();
			_console.Write ("Work finished.");
			PublishStatistics ();
		}

		void TorrentBackgroundWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			((TorrentBackgroundWorker)sender).DoPersonalWork (e);
		}

		void TorrentBackgroundWorker_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			ITorrentBackgroundWorker worker = (ITorrentBackgroundWorker)sender;
			_publisher.Publish (worker.Name, worker.NewLinks);
			_linksFoundCount += worker.NewLinks.Count;
			_console.Debug ("Torrent watcher [{0}] has completed work.", worker.Name);
			_console.Debug ("   found {0} torrent(s).", worker.NewLinks.Count);
		}

		TorrentBackgroundWorker AddWatch (TorrentTarget idleItem)
		{
			var worker = new TorrentBackgroundWorker (idleItem, _console, _parserManager);
			worker.DoWork += new DoWorkEventHandler (TorrentBackgroundWorker_DoWork);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler (TorrentBackgroundWorker_WorkCompleted);
			return worker;
		}

		IParsersManager _parserManager;

		public void Start ()
		{
			_console.Write ("Starting TorrentWatcher...");
			_console.Debug ("Debug mode is on.");
			IParser krutor = new KrutorParser ();
			IParser kinozal = new KinozalParser ();
			_parserManager = new ParsersManager (krutor,kinozal);
			//TODO: implement lostfilm.tv parser
			_console.Debug ("Watcher started");
			_consoleReader.WorkerSupportsCancellation = true;
			_consoleReader.DoWork += new DoWorkEventHandler (ConsoleReader_DoWork);
			_consoleReader.RunWorkerAsync ();

			RenewQueue ();

			System.Timers.Timer tim = new System.Timers.Timer ();
			tim.Interval = 600000;
			tim.AutoReset = true;
			tim.Elapsed += new ElapsedEventHandler (RenewQueueAndStartProcess);
			tim.Start ();

			while (_consoleReader.IsBusy) {
				Thread.Sleep (1000);
			}
		}

		void RenewQueue ()
		{
			if (!_canceling) {
				_reader.ProcessQueue ();
				// create and start watcher for each item
				foreach (TorrentTarget idleItem in _reader.IdleItems()) {
					if (_workers.Find (x => x.Name == idleItem.Name) == null) {
						TorrentBackgroundWorker newWorker = AddWatch (idleItem);
						_workers.Add (newWorker);
						_console.Debug ("Watcher [{0}] added to queue.", newWorker.Name);
					}
				}
				foreach (var item in _workers) {
					item.RunWorkerAsync ();
				}
			}
		}

		void RenewQueueAndStartProcess (object sender, ElapsedEventArgs e)
		{
			RenewQueue ();
		}
	}
}

