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
			console.Write ("TorrentWatcher 2017");
			string argsLine = String.Concat (args);
			console.Write ("Current command:{0}", argsLine);

			string item=arguments.KeyValue("add");
			string category = arguments.KeyValue("category");
			string remove = arguments.KeyValue ("remove");
			string hide = arguments.KeyValue ("hide");
			bool debug = !arguments.KeyExists ("nodebug");
			bool hideall = arguments.KeyExists ("hideall");
			bool watch = arguments.KeyExists ("watch");
			string site = arguments.KeyValue ("site");

			Watcher watcher = new Watcher (console, reader, debug);

			// TODO: excludes (if something exists in header or link ignor such file)
			// TODO: limit search by single site (Lostflm.tv for tv series)
			// TODO: List unhided links
			if (!string.IsNullOrEmpty (category) && !string.IsNullOrEmpty (item)) {
				watcher.Add (item, category, site);
				Environment.Exit (0);
			} else if (!string.IsNullOrEmpty (item)) {
				watcher.Add (item, "movie", site);
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
					Environment.Exit(watcher.Start ());
				}
				catch (Exception ex) {
					console.Write ("Unhandled exception:\r\n{0}", ex);
				}
			} else {
				console.Write (@"USAGE:
TorrentWatcher.exe [/COMMAND:TITLE[/category:movie/tvseries/book/sport]][/debug][/single]
WHERE:
  COMMAND:
     watch    - start watching
     add      - add watcher
     remove   - remove watcher
     hide     - hide links from published link for specified watcher
     hideall  - hide all links from published link for all watchers
     nodebug  - do not print debug messages
     TITLE    - movie title. If contains spaces then should be quoted.
     category - category of watcher (by default - movie)
     site     - single site to search (in one world, like 'lostfilm')
");
				Environment.Exit (0);
			}
			//TODO: Possibility to add date tostart search from
			//TODO: Possibility to add comment (add coment on links page as well)
		}
	}
}
