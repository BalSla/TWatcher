using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	public interface ITargetReader
	{
		/// <summary>
		/// Items removed in current session
		/// </summary>
		/// <value>The removed items.</value>
		int RemovedItems {
			get;
		}

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
		int AddedItems{ get; }
		string Queue { get;}
	}
}

