using System;
using System.Collections.Generic;

namespace TorrentWatcher.Helpers
{
	public static class ListExtentions
	{
		public static bool AddUnique<T> (this List<T> list, T item)
		{
			if (!list.Contains(item)) {
				list.Add (item);
				return true;
			}
			return false;
		}
	}
}

