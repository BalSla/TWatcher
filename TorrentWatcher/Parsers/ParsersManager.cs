using System;
using System.Collections.Generic;

namespace TorrentWatcher.Parsers
{
	public class ParsersManager : IParsersManager
	{
		#region IParsersManager implementation

		public IList<string> FindLinks (string searchString)
		{
			throw new NotImplementedException ();
		}

		#endregion

		private List<IParser> _parsers;
		public ParsersManager (params IParser[] args)
		{
			_parsers.AddRange (args);
		}
	}
}

