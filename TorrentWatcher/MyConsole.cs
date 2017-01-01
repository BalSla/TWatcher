using System;

namespace TorrentWatcher
{
	public class MyConsole
	{
		public void Debug (string text)
		{
			if (DebugOn) {
				Write (text);
			}
		}

		public MyConsole ()
		{
		}

		public bool DebugOn { get; set;}

		public void Write(string text)
		{
			Console.WriteLine (text);
		}

		public void Write (string text, params object[] args)
		{
			Console.WriteLine (text, args);
		}
	}
}

