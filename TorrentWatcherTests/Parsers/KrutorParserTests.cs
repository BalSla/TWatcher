using TorrentWatcher.Parsers;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TorrentWatcher
{
    [TestClass]
    public class KrutorParserTests
    {
        [TestMethod]
        public void FindLinks_Returns_At_Least_One_Movie()
        {
            KrutorParser parser = new KrutorParser();

            IList<string> links = parser.FindLinks("Пассажиры", SearchCondition.Movie);

            Assert.IsTrue(links.Count > 0);
        }

        [TestMethod]
        public void FindLinks_Returns_At_Least_One_Sport()
        {
            KrutorParser parser = new KrutorParser();

            IList<string> links = parser.FindLinks("Формула 1", SearchCondition.Sport);

            Assert.IsTrue(links.Count > 0);
        }

        [TestMethod]
        public void FindLinks_Returns_At_Least_One_TVSeries()
        {
            KrutorParser parser = new KrutorParser();

            IList<string> links = parser.FindLinks("Big Little Lies", SearchCondition.TvSeries);

            Assert.IsTrue(links.Count > 0);
        }
    }
}
