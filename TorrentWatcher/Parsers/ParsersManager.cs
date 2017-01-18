using System;
using System.Collections.Generic;

namespace TorrentWatcher.Parsers
{
	public class ParsersManager : IParsersManager
	{
		#region IParsersManager implementation

		public IList<string> FindLinks (string searchString, SearchCondition condition)
		{
		List<string> list = new List<string> ();
			foreach (IParser item in _parsers) {
				list.AddRange(item.FindLinks (searchString, condition));
			}
			return list;
		}

		#endregion

		private List<IParser> _parsers = new List<IParser> ();
		public ParsersManager (params IParser[] args)
		{
			_parsers.AddRange (args);
		}
	}
}

