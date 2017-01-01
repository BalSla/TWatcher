using System;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public string Name{ get; private set; }
		public string Site{ get; private set; }

		public TorrentTarget ()
		{

		}
	}
}

