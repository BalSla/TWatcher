using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestClass]
	public class LostFilmParserTests
	{
		[TestMethod]
		public void FindLinks_Returns_At_Least_One ()
		{
			LostFilmParser parser = new LostFilmParser ();

			IList<string> links = parser.FindLinks ("Better Call Saul", SearchCondition.TvSeries);

			Assert.IsTrue (links.Count > 0);
		}

		[TestMethod]
		public void ToString_Returns_Class_Type()
		{
			LostFilmParser p = new LostFilmParser ();

			Assert.IsTrue (p.ToString ().Contains ("LostFilmParser"));
		}
	}


}

