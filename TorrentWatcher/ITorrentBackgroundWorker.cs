using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace TorrentWatcher
{
	public interface ITorrentBackgroundWorker
	{
		string Name { get; }
		IList<string> NewLinks { get;}
		int LinksFoundCount{ get; }
	}
}

