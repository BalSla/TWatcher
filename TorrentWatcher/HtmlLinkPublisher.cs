using System;
using System.Collections.Generic;
using System.IO;
using CsQuery;
using CsQuery.ExtensionMethods;

namespace TorrentWatcher
{
	public class HtmlLinkPublisher : IPublisher
	{
		private const string OUTPUT_HTML="links.html";
		private object writeLocker = new object ();

		#region IPublisher implementation

		string UnsortedListOfLinks (IList<string> links)
		{
			string list = "";
			links.ForEach(x=>list+=string.Format("<li><a href='{0}'>{0}</a></li>", x));
			return list;
		}

		public void Publish (string title, IList<string> links)
		{
			lock (writeLocker) {
				if (links.Count != 0) {
					CQ doc;
					if (!File.Exists (OUTPUT_HTML)) {
						doc = CQ.CreateDocument ("<html><body><H1 id=header>TorrentWatcher links</H1><body><html>");
					} else {
						doc = CQ.CreateFromFile (OUTPUT_HTML);
					}
					CQ fragment;
					if (doc.Select (string.Format ("h2:contains({0})", title)).Length != 0) {
						fragment = CQ.CreateFragment (UnsortedListOfLinks (links));
						doc.Select (string.Format ("h2:contains({0})", title)).Next ().Children ().Last ().After (fragment).Render ();
					} else {
						fragment = CQ.CreateFragment (string.Format ("<H2>{0}</H2><ul>{1}</ul>", title, UnsortedListOfLinks (links)));
						doc.Select ("#header").After (fragment).Render ();
					}
					doc.Save (OUTPUT_HTML, DomRenderingOptions.Default);
				}
			}
		}

		#endregion

		public HtmlLinkPublisher ()
		{
		}
	}
}

