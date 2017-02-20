using NUnit.Framework;
using System;
using System.Xml.Serialization;
using System.IO;

namespace TorrentWatcher
{
	[TestFixture()]
	public class TorrentTargetTests
	{
		[Test()]
		public void TorrentTarget_Discovered_Links_Are_Serializable ()
		{
			DeleteTestDataFiles ();
			TorrentTarget target = new TorrentTarget ();
			target.Discovered.Add ("test1");
			target.Discovered.Add ("test2");

			XmlSerializer serializer = new XmlSerializer (typeof(TorrentTarget));
			using (TextWriter writer = new StreamWriter(QUEUE)) {
				serializer.Serialize (writer, target);
			}

			XmlSerializer deserializer = new XmlSerializer (typeof(TorrentTarget));
			using (TextReader reader = new StreamReader(QUEUE)) {
				target = (TorrentTarget)deserializer.Deserialize (reader);
			}

			Assert.AreEqual (2, target.Discovered.Count, "Wrong nuber of serialized items!");
			Assert.AreEqual ("test2", target.Discovered [1], "Wrong content of Discovered!");
		}
		private const string QUEUE = "queue.test.xml";
		private void DeleteTestDataFiles()
		{
			if (File.Exists (QUEUE)) {
				File.Delete (QUEUE);
			}
		}
	}
}

