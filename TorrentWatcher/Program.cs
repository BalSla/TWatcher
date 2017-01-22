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
			TargetReader reader = new TargetReader (console);
			List<string> arguments = new List<string> ();
			arguments.AddRange (args);

			string item=arguments.KeyValue("add");
			string category = arguments.KeyValue("category");

			Watcher watcher = new Watcher (console, reader);

			if (!string.IsNullOrEmpty (category) && !string.IsNullOrEmpty (item)) {
				watcher.Add (item, category);
				Environment.Exit (0);
			} else if (!string.IsNullOrEmpty (item)) {
				watcher.Add (item, "movie");
				Environment.Exit (0);
			}

			//TODO: Command to remove watcher (also should remove specified title in links.html)
			//TODO: Command to hide links (remove all links under specified title in links.html)
			//TODO: Command to watch for next series

			try
			{
				watcher.Start ();
				Environment.Exit(0);
			}
			catch (Exception ex) {
				console.Write ("Unhandled exception:\r\n{0}", ex);
			}
		}
	}
}
