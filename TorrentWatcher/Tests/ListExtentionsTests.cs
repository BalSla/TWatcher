using NUnit.Framework;
using System;
using System.Collections.Generic;
using TorrentWatcher.Helpers;

namespace TorrentWatcher
{
	[TestFixture()]
	public class ListExtentionsTests
	{
		[Test()]
		public void KeyValue_Returns_Value ()
		{
			List<string> args = new List<string>() {"/k:ggg","/fff"};

			Assert.AreEqual("ggg", args.KeyValue("k"));
		}

		[Test()]
		public void KeyValue_Returns_Empty_If_No_Key ()
		{
			List<string> args = new List<string>() {"/k:ggg","/fff"};

			Assert.AreEqual("", args.KeyValue("ee"));
		}
	}
}

