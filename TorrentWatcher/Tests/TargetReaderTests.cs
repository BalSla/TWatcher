using NUnit.Framework;
using System;
using System.Xml;
using System.IO;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TargetReaderTests
	{
		private const string QUEUE = "queue.xml";
		[Test()]
		public void ProcessQueue_New_Item()
		{
			File.WriteAllText ("newitem.txt", "New Movie");
			TargetReader r = new TargetReader (".");
			r.ProcessQueue ();
			File.Delete ("newitem.txt");

			Assert.AreEqual("New Movie", r.IdleItems()[0].Name, "Wrong name of new item!");
		}

		[Test()]
		public void ProcessQueue_Duplicate_Item_Wont_Created()
		{
			File.WriteAllText ("newitem.txt", "New Movie");
			File.WriteAllText ("newitem1.txt", "New Movie");
			TargetReader r = new TargetReader (".");
			r.ProcessQueue ();
			File.Delete ("newitem.txt");
			File.Delete ("newitem1.txt");

			Assert.AreEqual("New Movie", r.IdleItems()[0].Name, "Wrong name of new item!");
			Assert.AreEqual(1, r.IdleItems().Count, "Duplicated item created!");
		}

		[Test()]
		public void ProcessQueue_Remove_Completed_Item()
		{
			DeleteTestDataFiles ();
			File.WriteAllText ("newitem.txt", "New Movie");
			TargetReader r = new TargetReader (".");
			r.ProcessQueue ();
			File.WriteAllText ("newitem.txt", "Completed:New Movie");
			r.ProcessQueue ();
			File.Delete ("newitem.txt");

			Assert.AreEqual(0, r.IdleItems().Count, "Item hasn't been removed!");
		}

		private void DeleteTestDataFiles()
		{
			if (File.Exists (QUEUE)) {
				File.Delete (QUEUE);
			}
			if (File.Exists ("newitem.txt")) {
				File.Delete ("newitem.txt");
			}
			if (File.Exists ("newitem1.txt")) {
				File.Delete ("newitem1.txt");
			}
		}

		[Test()]
		public void SaveIncompleted_Saves_Items()
		{
			File.WriteAllText ("newitem.txt", "New Movie");
			File.WriteAllText ("newitem1.txt", "New Movie1");
			TargetReader r = new TargetReader (".");
			r.ProcessQueue ();
			File.Delete ("newitem.txt");
			File.Delete ("newitem1.txt");
			r.SaveIncompleted ();

			XmlDocument doc = new XmlDocument ();
			doc.Load (QUEUE);
			XmlNodeList nodes = doc.SelectNodes ("//TorrentTarget");

			Assert.AreEqual("New Movie", nodes[0].SelectSingleNode("Name").InnerText, "Wrong name of the first item!");
			Assert.AreEqual(2, nodes.Count, "Wrong number of saved items!");
		}

		[Test()]
		public void ProcessQueue_Reads_Items_Saved_Before()
		{
			SaveIncompleted_Saves_Items ();
			TargetReader r = new TargetReader (".");
			r.ReadQueue ();

			Assert.AreEqual("New Movie", r.IdleItems()[0].Name, "Wrong name of the first item!");
			Assert.AreEqual(2, r.IdleItems().Count, "Wrong number of saved items!");
		}

		[Test()]
		public void ProcessQueue_Doesnt_Throw_Exception_If_Queue_File_Is_Not_Existing()
		{
			DeleteTestDataFiles ();
			TargetReader r = new TargetReader (".");
			r.ProcessQueue ();

			Assert.AreEqual(0, r.IdleItems().Count, "Wrong number of saved items!");
		}
	}
}

