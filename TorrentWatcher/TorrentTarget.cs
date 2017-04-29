using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public TorrentTarget (string content, SearchCondition condition, string site)
		{
			Name = content.Trim ();
			Discovered = new List<string> ();
			SearchCondition = condition;
			Site = site;
		}

		public string Name{ get; private set; }
		public List<string> Discovered {get; private set; }

		public TorrentTarget ()
		{
			Discovered=new List<string>();
		}

		public SearchCondition SearchCondition { get; private set; }
		public string Site { get; private set; }
	}
}

