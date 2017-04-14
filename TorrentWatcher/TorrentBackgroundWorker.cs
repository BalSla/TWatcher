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
	public class TorrentBackgroundWorker : ITorrentBackgroundWorker
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

		public void DoPersonalWork (){
			_console.Debug ("Analyzing query [{0}]...", _torrent.Name);
			_newLinks.Clear ();
			IList<string> links=_parser.FindLinks(_torrent.Name, _torrent.SearchCondition);
			foreach (string item in links) {
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
		}
	}
}

