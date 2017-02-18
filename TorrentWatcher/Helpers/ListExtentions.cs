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

		public static string KeyValue(this List<string> args, string key)
		{
			string t = args.Find(x=>x.StartsWith("/"+key+":"));
			if (t!=null) {
				return t.Replace ("/" + key + ":", "");
			}
			return "";
		}

		public static bool KeyExists (this List<string> args, string key)
		{
			return !string.IsNullOrEmpty (args.Find (x => x.StartsWith ("/" + key)));
		}
	}
}

