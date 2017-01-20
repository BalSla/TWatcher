using NUnit.Framework;
using System;
using TorrentWatcher.Parsers;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestFixture()]
	public class KrutorParserTests
	{
		[Test()]
		public void FindLinks_Returns_At_Least_One ()
		{
			KrutorParser parser = new KrutorParser ();

			IList<string> links = parser.FindLinks ("Пассажиры", SearchCondition.Movie);

			Assert.IsTrue (links.Count > 0);
		}
	}
}

