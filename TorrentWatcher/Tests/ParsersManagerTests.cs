using NUnit.Framework;
using System;
using TorrentWatcher.Parsers;
using Moq;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestFixture()]
	public class ParsersManagerTests
	{
		[Test()]
		public void FindLinks_Calls_Specified_By_Site_Parser ()
		{
			Mock<IParser> parser = new Mock<IParser> ();
			parser.Setup (x => x.ToString ()).Returns ("Tesst");
			parser.Setup (x => x.FindLinks (It.IsAny<string> (), It.IsAny<SearchCondition> ())).Returns (new List<string> ());
			ParsersManager man = new ParsersManager (new MyConsole(), parser.Object);
			man.FindLinks ("dd", SearchCondition.Book, "teS");

			parser.Verify(x=>x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>()), Times.Once());
		}

		[Test()]
		public void FindLinks_Dosent_Call_Unspecified_By_Site_Parser ()
		{
			Mock<IParser> parser = new Mock<IParser> ();
			parser.Setup (x => x.ToString ()).Returns ("Tesst");
			parser.Setup (x => x.FindLinks (It.IsAny<string> (), It.IsAny<SearchCondition> ())).Returns (new List<string> ());
			ParsersManager man = new ParsersManager (new MyConsole(), parser.Object);
			man.FindLinks ("dd", SearchCondition.Book, "another");

			parser.Verify(x=>x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>()), Times.Never);
		}

		[Test()]
		public void FindLinks_Calls_All_Parsers_If_Site_Unspecified ()
		{
			Mock<IParser> parser1 = new Mock<IParser> ();
			parser1.Setup (x => x.ToString ()).Returns ("Tesst");
			parser1.Setup (x => x.FindLinks (It.IsAny<string> (), It.IsAny<SearchCondition> ())).Returns (new List<string> ());
			Mock<IParser> parser2 = new Mock<IParser> ();
			parser2.Setup (x => x.ToString ()).Returns ("Tesst");
			parser2.Setup (x => x.FindLinks (It.IsAny<string> (), It.IsAny<SearchCondition> ())).Returns (new List<string> ());
			ParsersManager man = new ParsersManager (new MyConsole(), parser1.Object, parser2.Object);
			man.FindLinks ("dd", SearchCondition.Book, "");

			parser1.Verify(x=>x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>()), Times.Once, "Parser 1 hasn't been called!");
			parser2.Verify(x=>x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>()), Times.Once, "Parser 2 hasn't been called!");
		}
	}
}

