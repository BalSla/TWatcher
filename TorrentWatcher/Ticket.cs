using System;

namespace TorrentWatcher
{
	[Serializable]
	public class Ticket
	{
		public Ticket(){
		}

		public override string ToString ()
		{
			return string.Format ("[Ticket: Title={0}, Category={1}, Site={2}, Action={3}]", Title, Category, Site, Action);
		}

		public Ticket (Action action, string title, string category, string site)
		{
			Action = action;
			Title = title;
			Category = (SearchCondition)Enum.Parse (typeof(SearchCondition), category, true);
			Site = site;
		}

		public String Title {
			get;
			set;
		}

		public SearchCondition Category {
			get;
			set;
		}

		public String Site {
			get;
			set;
		}

		public Action Action {
			get;
			set;
		}
	}

	public enum Action
	{
		Add,
		Remove,
	}
}

