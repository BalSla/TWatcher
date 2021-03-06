﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace TorrentWatcher
{
    //TODO: Cyrillic output (to console)
	public class MyConsole
	{
		public void Debug (string text, params object[] args)
		{
			if (DebugOn) {
				Write (text,args);
			}
		}
		private List<string> _debugOutput= new List<string>();

		public MyConsole ()
		{
		}

		public bool DebugOn { get; set;}

		public void Write(string text)
		{
			Console.WriteLine (text);
			if (DebugOn) {
				_debugOutput.Add (text);
			}
		}

		public void Write (string text, params object[] args)
		{
			Write(string.Format(text, args));
			System.Diagnostics.Debug.Print (text, args);
		}
	}
}

