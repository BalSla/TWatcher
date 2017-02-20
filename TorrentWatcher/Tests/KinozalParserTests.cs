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
	}
}

