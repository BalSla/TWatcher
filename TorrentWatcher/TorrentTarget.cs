using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public TorrentTarget (string content)
		{
			Name = content.Trim ();
			Discovered=new List<string>();
		}

		public string Name{ get; private set; }
		public List<string> Discovered {get; private set; }

		public TorrentTarget ()
		{
			Discovered=new List<string>();
		}
	}
}

