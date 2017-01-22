using System;
using System.Collections.Generic;
using System.IO;
using CsQuery;
using CsQuery.ExtensionMethods;
using System.Text;
using System.Web;

namespace TorrentWatcher
{
	public class HtmlLinkPublisher : IPublisher
	{
		private string _targetFile;
		private object writeLocker = new object ();

		#region IPublisher implementation

		public void Remove (string title)
		{
			lock (writeLocker) {
				if (File.Exists (_targetFile)) {
					CQ doc = CQ.CreateFromFile (_targetFile);
					doc.Select (string.Format ("h2:contains({0})", title)).Filter(x=>HttpUtility.HtmlDecode(x.InnerText)==title).Next ().Remove().Render();
					doc.Select (string.Format ("h2:contains({0})", title)).Filter(x=>HttpUtility.HtmlDecode(x.InnerText)==title).Remove().Render();
					doc.Save (_targetFile, DomRenderingOptions.Default);
				}
			}
		}

		public void Hide (string title)
		{
			lock (writeLocker) {
				if (File.Exists (_targetFile)) {
					CQ doc = CQ.CreateFromFile (_targetFile);
					doc.Select (string.Format ("h2:contains({0})", title)).Filter(x=>HttpUtility.HtmlDecode(x.InnerText)==title).Next ().Remove().Render();
					doc.Save (_targetFile, DomRenderingOptions.Default);
				}
			}
		}

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
					if (!File.Exists (_targetFile)) {
						doc = CQ.CreateDocument ("<html><body><H1 id=header>TorrentWatcher links</H1><body><html>");
					} else {
						doc = CQ.CreateFromFile (_targetFile);
					}
					CQ fragment;
					if (doc.Select (string.Format ("h2:contains({0})", title)).Length != 0) {
						fragment = CQ.CreateFragment (UnsortedListOfLinks (links));
						doc.Select (string.Format ("h2:contains({0})", title)).Next ().Children ().Last ().After (fragment).Render ();
					} else {
						fragment = CQ.CreateFragment (string.Format ("<H2>{0}</H2><ul>{1}</ul>", title, UnsortedListOfLinks (links)));
						doc.Select ("#header").After (fragment).Render ();
					}
					doc.Save (_targetFile, DomRenderingOptions.Default);
				}
			}
		}

		#endregion

		public HtmlLinkPublisher (string target)
		{
			_targetFile = target;
		}
	}
}

