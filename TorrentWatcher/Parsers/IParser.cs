using System;
using System.Collections.Generic;

namespace TorrentWatcher.Parsers
{
	public interface IParser
	{
		IList<string> FindLinks (string searchString, SearchCondition condition);
	}
}

