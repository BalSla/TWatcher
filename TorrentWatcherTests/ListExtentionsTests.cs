using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TorrentWatcher.Helpers;

namespace TorrentWatcher
{
    [TestClass]
	public class ListExtentionsTests
	{
		[TestMethod]
		public void KeyValue_Returns_Value ()
		{
			List<string> args = new List<string>() {"/k:ggg","/fff"};

			Assert.AreEqual("ggg", args.KeyValue("k"));
		}

		[TestMethod]
		public void KeyValue_Returns_Empty_If_No_Key ()
		{
			List<string> args = new List<string>() {"/k:ggg","/fff"};

			Assert.AreEqual("", args.KeyValue("ee"));
		}
	}
}

