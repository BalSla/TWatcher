using NUnit.Framework;
using System;
using CsQuery;
using TorrentWatcher.Parsers;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TorrentBackgroundWorkerTests
	{
		[Test()]
		public void DoPersonalWork_Reads_Krutor_Page ()
		{
			TorrentTarget target = new TorrentTarget("Героин");
			MyConsole console = new MyConsole ();
			console.DebugOn = true;
			TorrentBackgroundWorker worker = new TorrentBackgroundWorker (target, console, new ParsersManager(null));
			worker.DoPersonalWork ();
		}

		[Test()]
		public void DoPersonalWork_Adds_New_Link_To_TorrentTarget()
		{
			throw new NotImplementedException ();
		}

		[Test()]
		public void DoPersonalWork_Doesnt_Add_Already_Reported_Link()
		{
			throw new NotImplementedException ();
		}

	}
}

