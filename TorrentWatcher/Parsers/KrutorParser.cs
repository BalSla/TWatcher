using System;
using System.Collections.Generic;
using CsQuery;
using System.Web;

namespace TorrentWatcher.Parsers
{
	public class KrutorParser : IParser
	{
		public KrutorParser ()
		{
		}

		public IList<string> FindLinks(string searchString)
		{
			string encUrl = HttpUtility.UrlPathEncode (string.Format ("http://krutor.org/search/0/0/0/0/{0}", searchString));
			CQ pageData = CQ.CreateFromUrl(encUrl);
			List<string> torrents = new List<string> ();
			foreach (CsQuery.Implementation.HtmlAnchorElement item in pageData.Select("div[id='index']").Find("a")) {
				if (item.Href.StartsWith ("/torrent/")) {
					torrents.Add (item.Href);
				}
			}
			return torrents;
		}
	}
}

