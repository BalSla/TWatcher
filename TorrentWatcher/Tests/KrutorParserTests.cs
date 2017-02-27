using NUnit.Framework;
using System;
using TorrentWatcher.Parsers;
using System.Collections.Generic;
using TorrentWatcher.Helpers;

namespace TorrentWatcher
{
	[TestFixture()]
	public class KrutorParserTests
	{
		[Test()]
		public void FindLinks_Returns_At_Least_One_Movie ()
		{
			KrutorParser parser = new KrutorParser ();

			IList<string> links = parser.FindLinks ("Пассажиры", SearchCondition.Movie);

			Assert.IsTrue (links.Count > 0);
		}

		[Test()]
		public void FindLinks_Returns_At_Least_One_TVSeries ()
		{
			KrutorParser parser = new KrutorParser ();

			IList<string> links = parser.FindLinks ("Big Little Lies", SearchCondition.TvSeries);

			Assert.IsTrue (links.Count > 0);
		}
	}
}

