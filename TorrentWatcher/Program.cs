using System;

namespace TorrentWatcher
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			MyConsole console = new MyConsole ();
			TargetReader reader = new TargetReader (console);

			try
			{
			Watcher watcher = new Watcher (console, reader);
				watcher.Start ();
			}
			catch (Exception ex) {
				console.Write ("Unhandled exception:\r\n{0}", ex);
			}
		}
	}
}
