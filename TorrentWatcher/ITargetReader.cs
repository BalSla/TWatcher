using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	public interface ITargetReader
	{
		/// <summary>
		/// Saves the incompleted TorrentTarget items.
		/// </summary>
		void SaveIncompleted ();
		/// <summary>
		/// Reads saved collection of TorrentTarget items if they not red yet.
		/// </summary>
		/// <returns>The incomplete TorrentTarget items.</returns>
		IList<TorrentTarget> IdleItems ();
		/// <summary>
		/// Reads new item. Create TorrentTarget or delet for completed.
		/// </summary>
		void ProcessQueue ();
	}
}

