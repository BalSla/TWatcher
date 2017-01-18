using NUnit.Framework;
using System;
using CsQuery;
using TorrentWatcher.Parsers;
using Moq;
using System.Collections.Generic;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TorrentBackgroundWorkerTests
	{
		[Test()]
		public void DoPersonalWork_Adds_New_Link_To_TorrentTarget()
		{
			TorrentTarget target = new TorrentTarget("Героин",SearchCondition.Movie);
			MyConsole console = new MyConsole ();
			Mock<IParsersManager> parserMock = new Mock<IParsersManager> ();
			List<string> returnedLinks = new List<string> ();
			returnedLinks.Add ("stest link");
			parserMock.Setup(x => x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>())).Returns(returnedLinks);
			IParsersManager parser = parserMock.Object;

			TorrentBackgroundWorker worker = new TorrentBackgroundWorker (target, console, parser);
			worker.DoPersonalWork ();

			Assert.AreEqual (1, target.Discovered.Count);
		}

		[Test()]
		public void DoPersonalWork_Doesnt_Add_Already_Reported_Link()
		{
			TorrentTarget target = new TorrentTarget("Героин",SearchCondition.Movie);
			MyConsole console = new MyConsole ();
			Mock<IParsersManager> parserMock = new Mock<IParsersManager> ();
			List<string> returnedLinks = new List<string> ();
			returnedLinks.Add ("stest link");
			returnedLinks.Add ("stest link");
			parserMock.Setup(x => x.FindLinks(It.IsAny<string>(), It.IsAny<SearchCondition>())).Returns(returnedLinks);
			IParsersManager parser = parserMock.Object;

			TorrentBackgroundWorker worker = new TorrentBackgroundWorker (target, console, parser);
			worker.DoPersonalWork ();

			Assert.AreEqual (1, target.Discovered.Count);
		}

	}
}

