using System;
using System.Collections.Generic;

namespace TorrentWatcher.Parsers
{
	public class ParsersManager : IParsersManager
	{
		#region IParsersManager implementation

		public IList<string> FindLinks (string searchString, SearchCondition condition, string site)
		{
		List<string> list = new List<string> ();
			foreach (IParser item in _parsers) {
				if (String.IsNullOrEmpty (site) || item.ToString ().ToLower ().Contains (site.ToLowerInvariant ())) {
					try {
						list.AddRange (item.FindLinks (searchString, condition));
					} catch (Exception ex) {
						_console.Write (string.Format ("Error ({1}): {0}", ex.Message, item));
					}
				}
			}
			return list;
		}

		#endregion
		private MyConsole _console;
		private List<IParser> _parsers = new List<IParser> ();
		public ParsersManager (MyConsole console, params IParser[] args)
		{
			_console = console;
			_parsers.AddRange (args);
		}
	}
}

