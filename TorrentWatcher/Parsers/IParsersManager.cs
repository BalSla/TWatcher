using System;
using System.Collections.Generic;

namespace TorrentWatcher.Parsers
{
	public interface IParsersManager
	{
		IList<string> FindLinks (string searchString, SearchCondition condition, string site);
	}
}

