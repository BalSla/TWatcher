using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestFixture()]
	public class KinozalParserTests
	{
		[Test()]
		public void FindLinks_Returns_At_Least_One ()
		{
			KinozalParser parser = new KinozalParser ();

			IList<string> links = parser.FindLinks ("Пассажиры", SearchCondition.Movie);

			Assert.IsTrue (links.Count > 0);
		}

		[Test]
		public void Matches_Finds_Mastch()
		{
			string html = "&#1055;&#1072;&#1089;&#1089;&#1072;&#1078;&#1080;&#1088;&#1099; / Passengers / 2016 / &#1044;&#1041;, &#1057;&#1058; / 3D (OU) / BDRip (1080p)";

			Assert.IsTrue (KinozalParser.Matches(html, "Пассажиры"));
		}
	}
}

