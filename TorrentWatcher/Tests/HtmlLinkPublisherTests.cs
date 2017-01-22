using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using CsQuery;
using System.Text;
using System.Web;

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
			_publisher = new HtmlLinkPublisher (OUTPUT_HTML);
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
			File.WriteAllText (OUTPUT_HTML, "<html><head><meta charset='utf-8'/></head><body><h1 id=\"header\">TorrentWatcher links</h1><h2>Title</h2><ul><li>testlink1</li><li>testlink2</li></ul></body></html>");
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

			_publisher.Publish ("Title1", new List<string> () { "testlink4" });
			_publisher.Publish ("Title", new List<string> () { "testlink3" });
			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);
					
			Assert.AreEqual (1, doc["h2:contains(Title)"].Filter(x=>x.InnerText=="Title").Length, "Extra Title has been added!");
			Assert.AreEqual (1, doc.Select("h2:contains(Title)").Filter(x=>x.InnerText=="Title").Next().Select("li:contains(testlink3)").Length, "Output doesn't contain 3-st link!");
		}

		[Test()]
		public void Remove_Title_And_Related_Links ()
		{
			DeleteTestFiles ();
			CreateSimpleSample ();

			_publisher.Publish ("Title", new List<string> () { "testlink3" });
			_publisher.Publish ("Title1", new List<string> () { "testlink4" });
			_publisher.Remove ("Title");

			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);

			Assert.AreEqual (1, doc["h2"].Length, "Extra Title does exist!");
			Assert.AreEqual (1, doc.Select("li:contains(testlink4)").Length, "Output doesn't contain 4-st link!");
			Assert.AreEqual (0, doc.Select("li:contains(testlink3)").Length, "Output contains 3-st link!");
		}

		[Test()]
		public void Publish_Hide_Hides_All_Titles_Links ()
		{
			DeleteTestFiles ();
			CreateSimpleSample ();
			_publisher.Publish ("Title1", new List<string> () { "testlink3" });

			_publisher.Hide ("Title");

			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);

			Assert.AreEqual (2, doc.Select ("h2").Length, "Missed title!");
			Assert.AreEqual (1, doc.Select ("li").Length, "Extra links!");
			Assert.AreEqual (1, doc.Select ("li:contains(link3)").Length, "Missing link!");
		}

		[Test()]
		public void Publish_Hide_Hides_All_Titles_Cyrillic_Links ()
		{
			DeleteTestFiles ();
			CreateSimpleSample ();
			_publisher.Publish ("Мумия", new List<string> () { "testlink3" });

			CQ doc1 = CQ.CreateFromFile (OUTPUT_HTML);
			string h = doc1.Select ("h2:contains(Мумия)").RenderSelection ();
			string h1 = doc1.Select ("h2:contains(Мумия)").Filter(x=>x.InnerText=="Мумия").RenderSelection ();

			_publisher.Hide ("Мумия");

			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);

			Assert.AreEqual (2, doc.Select ("h2").Length, "Missed title!");
			Assert.AreEqual (2, doc.Select ("li").Length, "Extra links!");
			Assert.AreEqual (1, doc.Select ("li:contains(link1)").Length, "Missing link!");
		}

		[Test()]
		public void Save_Cyrillic_In_Html()
		{
			DeleteTestFiles ();
			_publisher.Publish ("Мумия", new List<string> () { "testlink3" });

			CQ doc = CQ.CreateFromFile (OUTPUT_HTML);
			string t0=doc.Select ("h2:contains(Мумия)").RenderSelection ();
			string t = doc.Select ("h2:contains(Мумия)").Filter (x => HttpUtility.HtmlDecode(x.InnerText) == "Мумия").RenderSelection ();

			Assert.AreEqual (1, doc.Select ("h2:contains(Мумия)").Filter(x=>HttpUtility.HtmlDecode(x.InnerText)=="Мумия").Length, "Missed title!");
		}
	}
}

