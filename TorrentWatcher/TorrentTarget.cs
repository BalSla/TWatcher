using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[Serializable]
	public class TorrentTarget
	{
		public TorrentTarget (string content, SearchCondition condition, string site)
		{
			DefaultInitialization (content, condition, site);
		}

		public TorrentTarget (string content, SearchCondition condition, int season, int episode, string site)
		{
			DefaultInitialization (content, condition, site);
			TvSeries = true;
			Season = season;
			Episode = episode;
		}

		private void DefaultInitialization(string content, SearchCondition condition, string site)
		{
			Name = content.Trim ();
			Discovered=new List<string>();
			SearchCondition = condition;
			Site = site;
		}

		public string Name{ get; private set; }
		public List<string> Discovered {get; private set; }

		public TorrentTarget ()
		{
			Discovered=new List<string>();
		}

		public bool TvSeries { get; private set; }
		public SearchCondition SearchCondition { get; private set; }
		public string Site { get; private set; }

		//TODO: remove unused properties
		public int Season { get; private set; }
		public int Episode { get; private set; }
	}
}

