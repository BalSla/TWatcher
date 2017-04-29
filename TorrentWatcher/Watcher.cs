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
using System.Xml.Serialization;

namespace TorrentWatcher
{
	public class Watcher
	{
		private MyConsole _console;
		private ITargetReader _reader;
		private int _linksFoundCount;
		private IPublisher _publisher;

		public Watcher (MyConsole console, ITargetReader reader, bool debug)
		{
			_console = console;
			_reader = reader;
			_console.DebugOn = debug;
			_publisher = new HtmlLinkPublisher ("links.html");
		}

		public void Remove (string remove)
		{
			AddTicket (Action.Remove, remove, "Movie", "");
			_publisher.Remove (remove);
		}

		public void AddTicket (Action action, string item, string category, string site, string ticketFile="")
		{
			Ticket ticket = new Ticket (action, item, category, site);
			if (string.IsNullOrEmpty (ticketFile)) {
				ticketFile = Path.GetRandomFileName () + ".txt";
			}

			XmlSerializer serializer = new XmlSerializer (typeof(Ticket));
			using (TextWriter writer = new StreamWriter(ticketFile)) {
				serializer.Serialize (writer, ticket);
				_console.Debug ("Ticket {0} to {1} has been saved.", ticket, action);
			}
		}

		public void HideAllLinks ()
		{
			_publisher.HideAllLinks ();
		}

		public void Hide (string hide)
		{
			_publisher.Hide (hide);
		}

		private List<TorrentBackgroundWorker> _workers = new List<TorrentBackgroundWorker> ();

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
				_reader.ProcessQueue ();
				// create and start watcher for each item
				foreach (TorrentTarget idleItem in _reader.IdleItems()) {
					if (_workers.Find (x => x.Name == idleItem.Name) == null) {
						TorrentBackgroundWorker newWorker = AddWatch (idleItem);
						_console.Debug ("[{0}]...", newWorker.Name);
						newWorker.DoPersonalWork ();
						_publisher.Publish (newWorker.Name, newWorker.NewLinks);
						_linksFoundCount += newWorker.NewLinks.Count;
						if (newWorker.NewLinks.Count>0) {
							_console.Debug ("   {0} torrent link(s) found.", newWorker.NewLinks.Count);
						}
					}
				}
		}
	}
}

