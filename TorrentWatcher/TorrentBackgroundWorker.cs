using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using CsQuery;
using TorrentWatcher.Parsers;
using TorrentWatcher.Helpers;
using System.Timers;
using System.Diagnostics;

namespace TorrentWatcher
{
	public class TorrentBackgroundWorker : BackgroundWorker, ITorrentBackgroundWorker
	{
		private TorrentTarget _torrent;
		private MyConsole _console;
		private IParsersManager _parser;
		List<string> _newLinks = new List<string> ();

		public IList<string> NewLinks
		{
			get {
				return _newLinks;
			}
		}

		public void DoPersonalWork (DoWorkEventArgs e){
			_console.Debug ("Analyzing query [{0}]...", _torrent.Name);
			_newLinks.Clear ();

			foreach (string item in _parser.FindLinks(_torrent.Name, _torrent.SearchCondition)) {
				if (CancellationPending) {
					break;
				}
				if (_torrent.Discovered.AddUnique (item)) {
					_newLinks.Add (item);
					_console.Debug ("  Found torrent [{0}]", item);
				}
			}
		}

		public string Name
		{
			get{ return _torrent.Name;}
		}

		public TorrentBackgroundWorker (TorrentTarget idleItem, MyConsole console, IParsersManager parser)
		{
			_parser = parser;
			_torrent = idleItem;
			_console = console;
			WorkerSupportsCancellation = true;
		}
	}
}

