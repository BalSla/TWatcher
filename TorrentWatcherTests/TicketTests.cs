using System.Xml.Serialization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TorrentWatcher
{
	[TestClass]
	public class TicketTests
	{
		private const string NEW_MOVIE = @"<?xml version='1.0' encoding='utf-8'?>
<Ticket xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <Title>movie_title</Title>
  <Category>Movie</Category>
  <Site>site_value</Site>
  <Action>Add</Action>
</Ticket>";

		[TestMethod]
		public void Ticket_May_Be_Deserilized ()
		{
			string file = Path.GetRandomFileName () + ".test";
			File.WriteAllText (file, NEW_MOVIE);
			XmlSerializer deserializer = new XmlSerializer (typeof(Ticket));
			using (TextReader reader = new StreamReader(file)) {
				Ticket ticket = (Ticket)deserializer.Deserialize (reader);
				Assert.IsNotNull (ticket);
			}
		}
	}
}

