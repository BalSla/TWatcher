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

		public IList<string> FindLinks(string searchString, SearchCondition condition)
		{
			string conditionFlag = "";
			switch (condition) {
			case SearchCondition.Book:
				conditionFlag = "13";
				break;
			case SearchCondition.Movie:
				conditionFlag = "1";
				break;
			case SearchCondition.RussianMovie:
				conditionFlag = "2";
				break;
			case SearchCondition.Sport:
				conditionFlag = "11";
				break;
			case SearchCondition.Documental:
				conditionFlag = "3";
				break;
				case SearchCondition.TvSeries:
				conditionFlag = "4";
				break;
			default:
				conditionFlag = "0";
				break;
			}
			string encUrl = HttpUtility.UrlPathEncode (string.Format ("http://krutor.org/search/0/{1}/0/0/{0}", searchString, conditionFlag));
			CQ pageData = CQ.CreateFromUrl(encUrl);
			List<string> torrents = new List<string> ();
			foreach (CsQuery.Implementation.HtmlAnchorElement item in pageData.Select("div[id='index']").Find("a")) {
				if (item.Href.StartsWith ("/torrent/")) {
					torrents.Add ("http://krutor.org"+item.Href);
				}
			}
			return torrents;
		}
	}
}

