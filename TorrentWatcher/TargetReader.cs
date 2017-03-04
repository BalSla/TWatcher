using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Xml.Serialization;

namespace TorrentWatcher
{
	public class TargetReader : ITargetReader
	{
		private int _addedItems;
		private string _queue;

		public string Queue {
			get { return _queue;}
		}

		#region ITargetReader implementation

		public int RemovedItems {
			get {
				return _removedItems;
			}
		}

		private int _removedItems;

		public void SaveIncompleted ()
		{
			XmlSerializer serializer = new XmlSerializer (typeof(List<TorrentTarget>));
			using (TextWriter writer = new StreamWriter(_queue)) {
				serializer.Serialize (writer, _targets);
				_console.Debug ("Waiting list has been saved.");
			}
		}

		public IList<TorrentTarget> IdleItems ()
		{
			return _targets;
		}

		void RemoveItem (string str)
		{
			str = str.TrimEnd ();
			TorrentTarget target = _targets.Find (x => x.Name == str);
			if (target!=null) {
				_targets.Remove (target);
				_removedItems++;
				_console.Write ("Item [{0}] removed from watching list.", target.Name);
			}
		}

		void AddItem(TorrentTarget target)
		{
			_targets.Add (target);
			_console.Write ("New target [{0}] has been added to watching list.", target.Name);
		}

		public void ReadQueue(){
			if (_targets == null) {
				_console.Write ("Reading watching list...");
				if (File.Exists (_queue)) {
					XmlSerializer deserializer = new XmlSerializer (typeof(List<TorrentTarget>));
					using (TextReader reader = new StreamReader(_queue)) {
						_targets = (List<TorrentTarget>)deserializer.Deserialize (reader);
					}
				} else {
					_targets = new List<TorrentTarget> ();
				}
				_console.Write ("Read {0} watched items.", _targets.Count);
			}
		}

		private string ExctractName(string text)
		{
			return text.Split (':') [1];
		}

		private int ExctractSeason(string content){
			return Convert.ToInt32(content.Split ('[') [1].Split (',') [0]);
		}

		private int ExctractEpisode(string content){
			return Convert.ToInt32(content.Split ('[') [1].Split (']') [0].Split(',')[1]);
		}

		public void ProcessQueue ()
		{
			ReadQueue ();
			_console.Debug ("Reading new entries within working folder...");
			foreach (FileInfo item in new DirectoryInfo(".").GetFiles("*.txt")) {
			string content = File.ReadAllText (item.FullName);
				//TODO:add books, documental and russian
				if (content.StartsWith ("Completed:")) {
					_console.Debug ("Found completed item [{0}].", content);
					RemoveItem (content.Replace ("Completed:", ""));
				} else if(content.StartsWith ("CompletedTVS[")){
					string name = ExctractName (content);
					RemoveItem (name);
					int season = ExctractSeason(content);
					int episode = ExctractEpisode(content);
					TorrentTarget newTarget = new TorrentTarget (name, SearchCondition.TvSeries, season, episode);
					if (!_targets.Exists(x=>x.Name==newTarget.Name)) {
						AddTarget(newTarget);
					}
				}else if (content.StartsWith ("TVS[")) {
					string name = ExctractName (content);
					int season = ExctractSeason(content);
					int episode = ExctractEpisode(content);
					TorrentTarget newTarget = new TorrentTarget (name, SearchCondition.TvSeries, season, episode);
					if (!_targets.Exists(x=>x.Name==newTarget.Name)) {
						AddTarget(newTarget);
					}
				} else {
					TorrentTarget newTarget = new TorrentTarget (content, SearchCondition.Movie);
					if (!_targets.Exists(x=>x.Name==newTarget.Name)) {
						AddTarget(newTarget);
					}
				}
				// delete input file
				File.Delete (item.FullName);
			}
		}

		public int AddedItems
		{ get { return _addedItems; } }

		private void AddTarget(TorrentTarget target)
		{
			if (!target.Name.Contains("]")) {
				_targets.Add (target);
				_addedItems++;
				_console.Write ("Added new watching target [{0}] ({1}).", target.Name, target.SearchCondition);
			}
		}

		#endregion

		private MyConsole _console;
		private List<TorrentTarget > _targets;

		public TargetReader(MyConsole console, string queuePath)
		{
			_console = console;
			_queue = queuePath;
			_console.Debug ("Queue file is [{0}]", queuePath);
		}
	}
}
