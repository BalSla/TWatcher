using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestClass]
	public class KinozalParserTests
	{
		[TestMethod]
		public void FindLinks_Returns_At_Least_One ()
		{
			KinozalParser parser = new KinozalParser ();

			IList<string> links = parser.FindLinks ("Пассажиры", SearchCondition.Movie);

			Assert.IsTrue (links.Count > 0);
		}

		[TestMethod]
		public void Matches_Finds_Mastch()
		{
			string html = "&#1055;&#1072;&#1089;&#1089;&#1072;&#1078;&#1080;&#1088;&#1099; / Passengers / 2016 / &#1044;&#1041;, &#1057;&#1058; / 3D (OU) / BDRip (1080p)";

			Assert.IsTrue (KinozalParser.Matches(html, "Пассажиры"));
		}

		[TestMethod]
		public void Matches_Doesnt_Find_Mastch_If_It_Is_Not_Exact()
		{
			string html = "&#1055;&#1072;&#1089;&#1089;&#1072;&#1078;&#1080;&#1088;&#1099; / Passengers / 2016 / &#1044;&#1041;, &#1057;&#1058; / 3D (OU) / BDRip (1080p)";

			Assert.IsFalse (KinozalParser.Matches(html, "The Passengers"));
		}
	}
}

