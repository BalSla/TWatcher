using System;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public TorrentTarget (string content)
		{
			Name = content.Trim ();
		}

		public string Name{ get; private set; }
		public string Site{ get; private set; }

		public TorrentTarget ()
		{

		}
	}
}

