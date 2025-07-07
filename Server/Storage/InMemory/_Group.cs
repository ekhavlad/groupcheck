using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using GroupCheck.Server;
using GroupCheck.Tools.Extensions;

namespace GroupCheck.ServerStorage
{
	public partial class InMemoryStorage : IServerStorageGroups
	{
		private readonly ConcurrentDictionary<int, Group> _groups = new ConcurrentDictionary<int, Group>();

		Group IServerStorageGroups.Create(Group group)
		{
			var created = _storage.Groups.Create(group);
			_groups.TryAdd(created.ID, created);
			return created;
		}
		Group IServerStorageGroups.Get(int groupID)
		{
			if (_groups.TryGetValue(groupID, out Group group))
				return group;

			var found = _storage.Groups.Get(groupID);
			if (found != null)
				_groups.TryAdd(found.ID, found);

			return found;
		}
		Group IServerStorageGroups.Update(Group group)
		{
			var updated = _storage.Groups.Update(group);
			_groups.AddOrUpdate(updated.ID, updated, (id, acc) => acc);
			return updated;
		}
		void IServerStorageGroups.Delete(int groupID)
		{
			_storage.Groups.Delete(groupID);
			_groups.TryRemove(groupID, out Group deleted);
		}
		IEnumerable<Group> IServerStorageGroups.GetGroupsByAccount(int accountID, long minRevision, int batchSize)
		{
			return null;
		}

		public Dictionary<int, List<int>> GetAllMembers()
		{
			var result = new Dictionary<int, List<int>>();
			foreach (var group in _members)
			{
				var groupID = group.Key;
				var members = group.Value.Select(_ => _.Value.MemberID).ToList();
				result.Add(groupID, members);
			}
			return result;
		}
	}
}
