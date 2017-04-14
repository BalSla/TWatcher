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
		private bool _singleCycle;

		public Watcher (MyConsole console, ITargetReader reader, bool debug, bool singleCycle)
		{
			_console = console;
			_reader = reader;
			_console.DebugOn = debug;
			_publisher = new HtmlLinkPublisher ("links.html");
			_singleCycle = singleCycle;
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
			string context = item;
			switch(category){
			case "tvseries":
				context = string.Format ("TVS[{0}]", item);
					break;
			case "movie":
				break;
			case "sport":
				context = string.Format ("Sport[{0}]", item);
				break;
			default:
				throw new NotImplementedException (string.Format("Ctaegory [{0}] is not supported yet!", category));
		}

			File.WriteAllText (Path.GetRandomFileName()+".txt" ,context);
			_console.Write ("Created ticket for [{0}] ({1}).", item, category);
		}

		private List<TorrentBackgroundWorker> _workers = new List<TorrentBackgroundWorker> ();
		private BackgroundWorker _consoleReader = new BackgroundWorker();

		void PublishStatistics ()
		{
			_console.Write("SESSION STATISTICS:");
			_console.Write ("  {0} item(s) added to watching list", _reader.AddedItems);
			_console.Write ("  {0} item(s) removed from watching list", _reader.RemovedItems);
			_console.Write ("  {0} link(s) found", _linksFoundCount);
		}


		private int _activeWatchersCounter=0;

		void TorrentBackgroundWorker_WorkCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			ITorrentBackgroundWorker worker = (ITorrentBackgroundWorker)sender;
			_publisher.Publish (worker.Name, worker.NewLinks);
			_linksFoundCount += worker.NewLinks.Count;
			_console.Debug ("Torrent watcher [{0}] has completed work.", worker.Name);
			_console.Debug ("   found {0} torrent(s).", worker.NewLinks.Count);
			_activeWatchersCounter--;

			if (_singleCycle && _activeWatchersCounter==0) {
				_consoleReader.CancelAsync ();
			}
		}

		TorrentBackgroundWorker AddWatch (TorrentTarget idleItem)
		{
			var worker = new TorrentBackgroundWorker (idleItem, _console, _parserManager);
			return worker;
		}

		IParsersManager _parserManager;

		public int Start ()
		{
			_console.Write ("Starting TorrentWatcher...");
			_console.Debug ("Debug mode is on.");
			IParser krutor = new KrutorParser ();
			IParser kinozal = new KinozalParser ();
			IParser lostfilm = new LostFilmParser ();
			_parserManager = new ParsersManager (_console, krutor,kinozal,lostfilm);
			//TODO: Implement rgfootball.net parser
			_console.Debug ("Watcher started");
			ProcessQueue ();
			_reader.SaveIncompleted ();
			_console.Write ("Work finished.");
			PublishStatistics ();

			return _linksFoundCount == 0 ? 0 : 2;
		}

		void ProcessQueue ()
		{
			_activeWatchersCounter = 0;
			if (!_canceling) {
				_reader.ProcessQueue ();
				// create and start watcher for each item
				foreach (TorrentTarget idleItem in _reader.IdleItems()) {
					if (_workers.Find (x => x.Name == idleItem.Name) == null) {
						TorrentBackgroundWorker newWorker = AddWatch (idleItem);
						_console.Debug ("Watcher [{0}] added to queue.", newWorker.Name);
						newWorker.DoPersonalWork ();
						_publisher.Publish (newWorker.Name, newWorker.NewLinks);
						_linksFoundCount += newWorker.NewLinks.Count;
						_console.Debug ("Torrent watcher [{0}] has completed work.", newWorker.Name);
						_console.Debug ("   found {0} torrent(s).", newWorker.NewLinks.Count);
					}
				}
			}
		}
	}
}

