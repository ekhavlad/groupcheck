using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using GroupCheck.Server;
using GroupCheck.Tools.Extensions.RuntimeExt;
using System.Collections.Concurrent;

namespace GroupCheck.ServerStorage
{
	public partial class InMemoryStorage : IServerStorageMembers
	{
		private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Member>> _members = new ConcurrentDictionary<int, ConcurrentDictionary<int, Member>>();

		Member IServerStorageMembers.Create(Member member)
		{
			lock (_checks)
			{
				var created = member.CreateCopy();
				var list = _members.GetOrAdd(created.GroupID, new ConcurrentDictionary<int, Member>());
				created.MemberID = list.Count + 1;
				list.TryAdd(created.MemberID, created);
				return created;
			}
		}
		Member IServerStorageMembers.Get(int groupID, int memberID)
		{
			return _members[groupID][memberID];
		}
		Member IServerStorageMembers.Update(Member member)
		{
			_members[member.GroupID][member.MemberID] = member;
			return member;
		}
		void IServerStorageMembers.Delete(int groupID, int memberID)
		{
			_members[groupID].TryRemove(memberID, out Member removed);
		}
		IEnumerable<Member> IServerStorageMembers.GetMembersByGroup(int groupID, long minRevision, int batchSize)
		{
			return _members[groupID].Values;
		}
	}
}
