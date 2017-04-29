using System;

namespace TorrentWatcher
{
	[Serializable]
	public class Ticket
	{
		public Ticket(){
		}

		public Ticket (Action action, string title, string category, string site)
		{
			Action = action;
			Title = title;
			Category = category;
			Site = site;
		}
		public String Title {
			get;
			private set;
		}

		public String Category {
			get;
			private set;
		}

		public String Site {
			get;
			private set;
		}

		public Action Action {
			get;
			private set;
		}
	}

	public enum Action
	{
		Add,
		Remove,
	}
}

