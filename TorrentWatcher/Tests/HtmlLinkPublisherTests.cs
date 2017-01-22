using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using CsQuery;

namespace TorrentWatcher
{
	[TestFixture()]
	public class HtmlLinkPublisherTests
	{
		private HtmlLinkPublisher _publisher;
		private const string OUTPUT_HTML="test_links.html";

		[TestFixtureSetUp()]
		public void TestFixtureSetup()
		{
			_publisher = new HtmlLinkPublisher ();
		}

		void DeleteTestFiles ()
		{
			if (File.Exists(OUTPUT_HTML)) {
				File.Delete (OUTPUT_HTML);
			}
		}

		[Test()]
		public void Publish_Creates_New_Html_File ()
		{
			DeleteTestFiles ();
			_publisher.Publish ("Title", new List<string> () { "testlink1" });

			Assert.IsTrue (File.Exists (OUTPUT_HTML), "Output hasn't been created!");
		}

		[Test()]
		public void Publish_Creates_New_Html_File_With_Title_Within ()
		{
			DeleteTestFiles ();
			_publisher.Publish ("Title", new List<string> () { "testlink1", "testlink2" });
			string context = File.ReadAllText (OUTPUT_HTML);

			Assert.IsTrue (context.Contains("<h2>Title</h2>"), "Output doesn't contain title!");
			Assert.IsTrue (context.Contains("testlink1"), "Output doesn't contain 1-st link!");
			Assert.IsTrue (context.Contains("testlink2"), "Output doesn't contain 2-nd link!");
		}

		void CreateSimpleSample ()
		{
			File.WriteAllText (OUTPUT_HTML, "<html><head></head><body><h1 id=\"header\">TorrentWatcher links</h1><h2>Title</h2><ul><li>testlink1</li><li>testlink2</li></ul></body></html>");
		}

		[Test()]
		public void Publish_Append_New_Title ()
		{
			DeleteTestFiles ();
			CreateSimpleSample ();

			_publisher.Publish ("Title1", new List<string> () { "testlink3" });
			string context = File.ReadAllText (OUTPUT_HTML);

			Assert.IsTrue (context.Contains("<h2>Title1</h2>"), "Output doesn't contain new title!");
		}

		[Test()]
		public void Publish_Append_New_Link_To_Existing_Title ()
		{
			DeleteTestFiles ();
			CreateSimpleSample ();

			_publisher.Publish ("Title", new List<string> () { "testlink3" });
			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);
					
			Assert.AreEqual (1, doc["h2"].Length, "Extra Title has been added!");
			Assert.AreEqual (1, doc.Select("li:contains(testlink3)").Length, "Output doesn't contain 3-st link!");
		}
	}
}

