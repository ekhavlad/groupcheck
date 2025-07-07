using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace GroupCheck.ServerStorage
{
	public partial class InMemoryStorage// : IServerStorage, IServerStorageAccounts, IServerStorageGroups, IServerStorageMembers, IServerStorageChecks
	{
		private readonly IServerStorage _storage;

		public InMemoryStorage(IServerStorage storage)
		{
			_storage = storage;
		}

		public IServerStorageAccounts Accounts { get { return this; } }
		public IServerStorageGroups Groups { get { return this; } }
		public IServerStorageMembers Members { get { return this; } }
		public IServerStorageChecks Checks { get { return this; } }
	}
}
