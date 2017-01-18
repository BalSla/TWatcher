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

namespace TorrentWatcher
{
	public class TorrentBackgroundWorker : BackgroundWorker
	{
		public void DoPersonalWork (){
			_console.Debug ("Analyzing query [{0}]...", _torrent.Name);

			foreach (string item in _parser.FindLinks(_torrent.Name, _torrent.SearchCondition)) {

				if (_torrent.Discovered.AddUnique (item)) {
					_console.Debug ("  Found torrent [{0}]", item);
				}
			}
		}

		private TorrentTarget _torrent;
		private MyConsole _console;
		private IParsersManager _parser;

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

