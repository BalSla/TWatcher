using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Xml.Serialization;

namespace TorrentWatcher
{
	public class TargetReader : ITargetReader
	{
		private  const string QUEUE="queue.xml";
		#region ITargetReader implementation

		public void SaveIncompleted ()
		{
			XmlSerializer serializer = new XmlSerializer (typeof(List<TorrentTarget>));
			using (TextWriter writer = new StreamWriter(QUEUE)) {
				serializer.Serialize (writer, _targets);
			}
		}

		public IList<TorrentTarget> IdleItems ()
		{
			return _targets;
		}

		void RemoveItem (string str)
		{
			TorrentTarget target = _targets.Find (x => x.Name == str);
			if (target!=null) {
				_targets.Remove (target);
			}
		}

		public void ReadQueue(){
			if (_targets == null) {
				if (File.Exists (QUEUE)) {
					XmlSerializer deserializer = new XmlSerializer (typeof(List<TorrentTarget>));
					using (TextReader reader = new StreamReader(QUEUE)) {
						_targets = (List<TorrentTarget>)deserializer.Deserialize (reader);
					}
				} else {
					_targets = new List<TorrentTarget> ();
				}
			}
		}

		public void ProcessQueue ()
		{
			ReadQueue ();
			foreach (var item in new DirectoryInfo(".").GetFiles("*.txt")) {
			string content = File.ReadAllText (item.FullName);
				if (content.StartsWith ("Completed:")) {
					RemoveItem (content.Replace ("Completed:", ""));
				} else {
					TorrentTarget newTarget = new TorrentTarget (content);
					if (!_targets.Exists(x=>x.Name==newTarget.Name)) {
						_targets.Add (newTarget);
					}
				}
			}
		}

		#endregion

		private string _repository;
		private List<TorrentTarget > _targets;

		public TargetReader(string repository)
		{
			_repository = repository;
		}
	}
}
