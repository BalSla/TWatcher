using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using CsQuery;
using System.Net;
using System.Web;
using System.IO;
using TorrentWatcher.Parsers;
using System.Diagnostics;
using System.Timers;

namespace TorrentWatcher
{
	interface IPublisher
	{
		void Hide (string hide);
		void Publish(string title, IList<string> links);
		void Remove (string remove);
	}
}

