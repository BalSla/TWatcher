using NUnit.Framework;
using System;
using System.Xml;
using System.IO;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TargetReaderTests
	{
		private const string NEW_ITEM_FILE = "newitem.txt";
		private const string NEW_ITEM_FILE_1 = "newitem1.txt";
		private const string NEW_MOVIE = "New Movie";
		private const string NEW_TV_SERIES = "TVS[2,3]:New TV Series";
		private const string COMPLETED_MOVIE = @"Completed:New Movie
";
		private const string COMPLETED_TV_SERIES = "CompletedTVS[3,4]:New TV Series";
		private const string SAVED_ITEMS = @"";
		//private TargetReader _reader;

		[TestFixtureSetUp()]
		public void TestFixtureSetup()
		{
			//_reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
		}

		[Test()]
		public void ProcessQueue_Correctly_Reads_TVSeries_New_Item()
		{
			DeleteTestDataFiles ();
			File.WriteAllText (NEW_ITEM_FILE, NEW_TV_SERIES);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			Assert.AreEqual("New TV Series", _reader.IdleItems()[0].Name, "Wrong name!");
			Assert.AreEqual (2, _reader.IdleItems() [0].Season, "Wrong season!");
			Assert.AreEqual (3, _reader.IdleItems() [0].Episode, "Wrong episode!");
		}

		[Test()]
		public void ProcessQueue_Correctly_Reads_TVSeries_Completed_Data_And_Correct_Existing_Watcher()
		{
			DeleteTestDataFiles ();
			File.WriteAllText (NEW_ITEM_FILE, NEW_TV_SERIES);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			DeleteTestDataFiles ();
			File.WriteAllText (NEW_ITEM_FILE, COMPLETED_TV_SERIES);
			_reader.ProcessQueue ();

			Assert.AreEqual (1, _reader.IdleItems ().Count, "Wrong number of items!");
			Assert.AreEqual (3, _reader.IdleItems() [0].Season, "Wrong season!");
			Assert.AreEqual (4, _reader.IdleItems() [0].Episode, "Wrong episode!");
		}

		[Test()]
		public void ProcessQueue_New_Item()
		{
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			Assert.AreEqual(NEW_MOVIE, _reader.IdleItems()[0].Name, "Wrong name of new item!");
			Assert.IsFalse(_reader.IdleItems()[0].TvSeries, "Wrong type of new item!");
		}

		[Test()]
		public void ProcessQueue_Duplicate_Item_Wont_Created()
		{
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			File.WriteAllText (NEW_ITEM_FILE_1, NEW_MOVIE);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();
			File.Delete (NEW_ITEM_FILE);
			File.Delete (NEW_ITEM_FILE_1);

			Assert.AreEqual(NEW_MOVIE, _reader.IdleItems()[0].Name, "Wrong name of new item!");
			Assert.AreEqual(1, _reader.IdleItems().Count, "Duplicated item created!");
		}

		[Test()]
		public void ProcessQueue_Remove_Completed_Item()
		{
			DeleteTestDataFiles ();
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();
			File.WriteAllText (NEW_ITEM_FILE, COMPLETED_MOVIE);
			_reader.ProcessQueue ();
			File.Delete (NEW_ITEM_FILE);

			Assert.AreEqual(0, _reader.IdleItems().Count, "Item hasn't been removed!");
		}

		private void DeleteTestDataFiles()
		{
			File.Delete ("*.test");
			if (File.Exists (NEW_ITEM_FILE)) {
				File.Delete (NEW_ITEM_FILE);
			}
			if (File.Exists (NEW_ITEM_FILE_1)) {
				File.Delete (NEW_ITEM_FILE_1);
			}
		}

		[Test()]
		public void SaveIncompleted_Saves_Items()
		{
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			File.WriteAllText (NEW_ITEM_FILE_1, NEW_TV_SERIES);
			TargetReader _reader = new TargetReader (new MyConsole(), _file);
			_reader.ProcessQueue ();
			File.Delete (NEW_ITEM_FILE);
			File.Delete (NEW_ITEM_FILE_1);
			_reader.SaveIncompleted ();

			XmlDocument doc = new XmlDocument ();
			doc.Load (_reader.Queue);
			XmlNodeList nodes = doc.SelectNodes ("//TorrentTarget");

			Assert.AreEqual(NEW_MOVIE, nodes[0].SelectSingleNode("Name").InnerText, "Wrong name of the first item!");
			Assert.AreEqual(2, nodes.Count, "Wrong number of saved items!");
		}
		private string _file;

		[Test()]
		public void ProcessQueue_Reads_Items_Saved_Before()
		{
			_file=Path.GetRandomFileName()+".test";
			SaveIncompleted_Saves_Items ();
			TargetReader _reader = new TargetReader (new MyConsole(), _file);
			_reader.ReadQueue ();

			Assert.AreEqual(NEW_MOVIE, _reader.IdleItems()[0].Name, "Wrong name of the first item!");
			Assert.AreEqual(2, _reader.IdleItems().Count, "Wrong number of saved items!");
		}

		[Test()]
		public void ProcessQueue_Doesnt_Throw_Exception_If_Queue_File_Is_Not_Existing()
		{
			DeleteTestDataFiles ();
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			Assert.AreEqual(0, _reader.IdleItems().Count, "Wrong number of saved items!");
		}

	}
}

