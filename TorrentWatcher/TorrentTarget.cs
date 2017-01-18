using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public TorrentTarget (string content, SearchCondition condition)
		{
			DefaultInitialization (content, condition);
		}

		public TorrentTarget (string content, SearchCondition condition, int season, int episode)
		{
			DefaultInitialization (content, condition);
			TvSeries = true;
			Season = season;
			Episode = episode;
		}

		private void DefaultInitialization(string content, SearchCondition condition)
		{
			Name = content.Trim ();
			Discovered=new List<string>();
			SearchCondition = condition;
		}

		public string Name{ get; private set; }
		public List<string> Discovered {get; private set; }

		public TorrentTarget ()
		{
			Discovered=new List<string>();
		}

		public bool TvSeries { get; private set; }
		public int Season { get; private set; }
		public int Episode { get; private set; }
		public SearchCondition SearchCondition { get; private set; }
	}
}

