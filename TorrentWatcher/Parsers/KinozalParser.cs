using System;
using TorrentWatcher.Parsers;
using System.Web;
using CsQuery;
using System.Collections.Generic;

namespace TorrentWatcher
{
	public class KinozalParser : IParser
	{
		#region IParser implementation

		public System.Collections.Generic.IList<string> FindLinks (string searchString, SearchCondition condition)
		{
			//TODO: Specify genar (http://kinozal.tv/browse.php?s=%CC%F3%E4%F0%EE%F1%F2%FC&g=0&c=1002&v=0&d=0&w=0&t=0&f=0)
			string encUrl = HttpUtility.UrlPathEncode (string.Format ("http://kinozal.tv/browse.php?s={0}", searchString));
			CQ pageData = CQ.CreateFromUrl(encUrl);
			List<string> torrents = new List<string> ();
			string test = pageData.Document.Body.InnerHTML;
			foreach (CsQuery.Implementation.HtmlAnchorElement item in pageData.Select("table[class='t_peer w100p']").Find("a")) {
				if (item.Href.StartsWith ("/details")) {
					torrents.Add ("http://kinozal.tv"+item.Href);
				}
			}
			return torrents;
		}

		#endregion

		public KinozalParser ()
		{
		}
	}
}

