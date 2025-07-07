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
	public partial class InMemoryStorage : IServerStorageChecks
	{
		private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Check>> _checks = new ConcurrentDictionary<int, ConcurrentDictionary<int, Check>>();

		Check IServerStorageChecks.Create(Check check)
		{
			lock(_checks)
			{
				var created = check.CreateCopy();
				var list = _checks.GetOrAdd(created.GroupID, new ConcurrentDictionary<int, Check>());
				created.CheckID = list.Count + 1;
				list.TryAdd(created.CheckID, created);
				return created;
			}
		}
		Check IServerStorageChecks.Get(int groupID, int checkID)
		{
			return _checks[groupID][checkID];
		}
		Check IServerStorageChecks.Update(Check check)
		{
			_checks[check.GroupID][check.CheckID] = check;
			return check;
		}
		void IServerStorageChecks.Delete(int groupID, int checkID)
		{
			_checks[groupID].TryRemove(checkID, out Check removed);
		}
		IEnumerable<Check> IServerStorageChecks.GetChecksByGroup(int groupID, long minRevision, int batchSize)
		{
			return _checks[groupID].Values;
		}
	}
}
