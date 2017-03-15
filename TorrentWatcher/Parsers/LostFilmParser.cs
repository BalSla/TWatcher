using System;
using TorrentWatcher.Parsers;
using System.Collections.Generic;
using System.Web;
using CsQuery;
using TorrentWatcher.Helpers;

namespace TorrentWatcher
{
	public class LostFilmParser : IParser
	{
		#region IParser implementation

		public IList<string> FindLinks (string searchString, SearchCondition condition)
		{
			List<string> torrents = new List<string> ();
			if (condition == SearchCondition.TvSeries) {
				string encUrl = HttpUtility.UrlPathEncode (string.Format ("http://www.lostfilm.tv/series/{0}", searchString));
				CQ pageData = CQ.CreateFromUrl (encUrl);
				string test = pageData.Document.Body.InnerHTML;
				CQ series = pageData.Select ("table[class='movie-parts-list']");
				foreach (CsQuery.Implementation.DomElement item in series.Select("td[onclick]")) {
					string h = item.GetAttribute ("onclick").ToString ();
					if (!h.Contains ("/comments")) {
						torrents.AddUnique (string.Format ("http://lostfilm.tv{0}", h.Replace ("goTo('", "").Replace ("',false)", "")));
					}
				}
				foreach (CsQuery.Implementation.DomElement item in series.Select("tr[class='not-available'] td[onclick]")) {
					string h = item.GetAttribute ("onclick").ToString ();
					torrents.Remove (string.Format ("http://lostfilm.tv{0}", h.Replace ("goTo('", "").Replace ("',false)", "")));
				}
			}
			return torrents;
		}

		#endregion

		public LostFilmParser ()
		{
		}
	}
}

