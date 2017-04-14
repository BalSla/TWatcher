using NUnit.Framework;
using System;
using CsQuery;
using TorrentWatcher.Parsers;
using Moq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TorrentBackgroundWorkerTests
	{
		private string _test;

		[Test()]
		public void DoWork_Waits_For_Work_Compled(){
			_test = "fail";
			BackgroundWorker worker = new BackgroundWorker ();
			worker.DoWork += new DoWorkEventHandler (Worker_Do_Work);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler (Worker_Completed);
			worker.WorkerSupportsCancellation = true;
			worker.RunWorkerAsync ();
			Thread.Sleep (1000);
			worker.CancelAsync ();
			while (_test!="test") {
				Thread.Sleep (50);
			}

			Assert.AreEqual ("test", _test, "Work_Completed hasn't been completed!");
		}

		void Worker_Do_Work (object sender, DoWorkEventArgs e)
		{
			while (!((BackgroundWorker)sender).CancellationPending) {
				Thread.Sleep (100);
			}
			e.Cancel = true;
		}

		void Worker_Completed (object sender, RunWorkerCompletedEventArgs e)
		{
			Thread.Sleep (2000);
			_test = "test";
		}

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

