using System;
using System.Collections.Generic;

namespace TorrentWatcher
{
	public class TargetReader : ITargetReader
	{
		#region ITargetReader implementation

		#endregion

		private string _repository;
		public TargetReader(string repository)
		{
			_repository = repository;
		}
	}
}
