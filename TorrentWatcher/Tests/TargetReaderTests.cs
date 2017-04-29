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
		private const string NEW_MOVIE = @"<?xml version='1.0' encoding='utf-8'?>
<Ticket xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <Title>movie_title</Title>
  <Category>Movie</Category>
  <Site>site_value</Site>
  <Action>Add</Action>
</Ticket>";
		private const string NEW_TV_SERIES = @"<?xml version='1.0' encoding='utf-8'?>
<Ticket xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <Title>movie_series_title</Title>
  <Category>TvSeries</Category>
  <Site>site_value</Site>
  <Action>Add</Action>
</Ticket>";
		private const string COMPLETED_MOVIE = @"<?xml version='1.0' encoding='utf-8'?>
<Ticket xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <Title>movie_title</Title>
  <Category>Movie</Category>
  <Site>site_value</Site>
  <Action>Remove</Action>
</Ticket>";
		private const string COMPLETED_TV_SERIES = @"<?xml version='1.0' encoding='utf-8'?>
<Ticket xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <Title>movie_series_title</Title>
  <Category>TvSeries</Category>
  <Site>site_value</Site>
  <Action>Remove</Action>
</Ticket>";
		private const string SAVED_ITEMS = @"";
		//private TargetReader _reader;

		[TestFixtureSetUp()]
		public void TestFixtureSetup()
		{
			//_reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
		}

		[Test()]
		public void Add_Creates_Ticket_File_As_XML()
		{
			MyConsole console = new MyConsole ();
			TargetReader _reader = new TargetReader (console, Path.GetRandomFileName()+".test");
			Watcher watcher = new Watcher(console, _reader, true);
			string ticketFile = Path.GetRandomFileName () + ".test";

			watcher.AddTicket (Action.Add, "movie_title", "movie", "site_value", ticketFile);

			string context = File.ReadAllText (ticketFile);

			Assert.IsTrue(context.Contains("<Title>movie_title</Title>"), "Title saved incorrectly!");
			Assert.IsTrue(context.Contains("<Category>Movie</Category>"), "Category saved incorrectly!");
			Assert.IsTrue(context.Contains("<Site>site_value</Site>"), "Site saved incorrectly!");
		}

		[Test()]
		public void ReadQueue_Reads_Target()
		{
			string content= @"<?xml version='1.0' encoding='utf-8'?>
<ArrayOfTorrentTarget xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <TorrentTarget>
    <Name>Дюнкерк</Name>
    <Discovered>
      <string>http://kinozal.tv/details.php?id=1506560</string>
      <string>http://kinozal.tv/details.php?id=1244052</string>
    </Discovered>
    <SearchCondition>Movie</SearchCondition>
  </TorrentTarget>
</ArrayOfTorrentTarget>";
			string queue = Path.GetRandomFileName () + ".test";
			File.WriteAllText (queue, content);
			TargetReader _reader = new TargetReader (new MyConsole(), queue);
			_reader.ReadQueue ();

			Assert.AreEqual(1,_reader.IdleItems().Count, "Wrong number of items from queue!");
		}

		[Test()]
		public void ProcessQueue_New_Item()
		{
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			Assert.AreEqual("movie_title", _reader.IdleItems()[0].Name, "Wrong name of new item!");
			Assert.AreEqual(SearchCondition.Movie, _reader.IdleItems()[0].SearchCondition, "Wrong type of new item!");
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

			Assert.AreEqual("movie_title", _reader.IdleItems()[0].Name, "Wrong name of new item!");
			Assert.AreEqual(1, _reader.IdleItems().Count, "Duplicated item created!");
		}

		[Test()]
		public void ProcessQueue_Remove_Completed_Item()
		{
			File.WriteAllText (NEW_ITEM_FILE, NEW_MOVIE);
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();
			File.WriteAllText (NEW_ITEM_FILE, COMPLETED_MOVIE);
			_reader.ProcessQueue ();
			File.Delete (NEW_ITEM_FILE);

			Assert.AreEqual(0, _reader.IdleItems().Count, "Item hasn't been removed!");
		}

		[Test()]
		public void SaveIncompleted_Saves_Target_Site()
		{
			_file = Path.GetRandomFileName()+".test";
			TargetReader _reader = new TargetReader (new MyConsole(), _file);
			_reader.AddItem(new TorrentTarget("movie", SearchCondition.Book, "test_site"));

			_reader.SaveIncompleted();

			string queue = File.ReadAllText (_file);

			Assert.IsTrue (queue.Contains ("<Site>test_site"));
		}

		[Test()]
		public void SaveIncompleted_Saves_Items()
		{
			_file = Path.GetRandomFileName()+".test";
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

			Assert.AreEqual("movie_title", nodes[0].SelectSingleNode("Name").InnerText, "Wrong name of the first item!");
			Assert.AreEqual(2, nodes.Count, "Wrong number of saved items!");
		}
		private string _file;

		[Test()]
		public void ProcessQueue_Doesnt_Throw_Exception_If_Queue_File_Is_Not_Existing()
		{
			TargetReader _reader = new TargetReader (new MyConsole(), Path.GetRandomFileName()+".test");
			_reader.ProcessQueue ();

			Assert.AreEqual(0, _reader.IdleItems().Count, "Wrong number of saved items!");
		}

	}
}

