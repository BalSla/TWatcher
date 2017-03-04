using System;
using System.Collections.Generic;
using TorrentWatcher.Helpers;

namespace TorrentWatcher
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			MyConsole console = new MyConsole ();
			TargetReader reader = new TargetReader (console, "queue.xml");
			List<string> arguments = new List<string> ();
			arguments.AddRange (args);

			string item=arguments.KeyValue("add");
			string category = arguments.KeyValue("category");
			string remove = arguments.KeyValue ("remove");
			string hide = arguments.KeyValue ("hide");
			bool debug = arguments.KeyExists ("debug");
			bool hideall = arguments.KeyExists ("hideall");
			bool watch = arguments.KeyExists ("watch");

			Watcher watcher = new Watcher (console, reader, debug);

			if (!string.IsNullOrEmpty (category) && !string.IsNullOrEmpty (item)) {
				watcher.Add (item, category);
				Environment.Exit (0);
			} else if (!string.IsNullOrEmpty (item)) {
				watcher.Add (item, "movie");
				Environment.Exit (0);
			} else if (!string.IsNullOrEmpty (remove)) {
				watcher.Remove (remove);
				Environment.Exit (0);
			} else if (!string.IsNullOrEmpty (hide)) {
				watcher.Hide (hide);
				Environment.Exit (0);
			} else if (hideall) {
				watcher.HideAllLinks ();
				Environment.Exit (0);
			} else if(watch) {
				try
				{
					watcher.Start ();
					Environment.Exit(0);
				}
				catch (Exception ex) {
					console.Write ("Unhandled exception:\r\n{0}", ex);
				}
			} else {
				console.Write (@"USAGE:
TorrentWatcher.exe [/COMMAND:TITLE[/category:movie/tvseries/book]][/debug]
WHERE:
  COMMAND:
     watch    - start watching
     add      - add watcher
     remove   - remove watcher
     hide     - hide links from published link for specified watcher
     hideall  - hide all links from published link for all watchers
     debug    - print debug messages
     TITLE    - movie titl. If contains spaces then should be quoted.
     category - category of watcher (by default - movie)
");
				Environment.Exit (0);
			}
			//TODO: Command to watch for next series
			//TODO: Possibility to add comment (add coment on links page as well)
			//TODO: Possibility toadd date tostart search from
		}
	}
}
