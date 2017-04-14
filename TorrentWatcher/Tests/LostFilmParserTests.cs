using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestFixture()]
	public class LostFilmParserTests
	{
		[Test()]
		public void FindLinks_Returns_At_Least_One ()
		{
			LostFilmParser parser = new LostFilmParser ();

			IList<string> links = parser.FindLinks ("Better Call Saul", SearchCondition.TvSeries);

			Assert.IsTrue (links.Count > 0);
		}
	}
}

