using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using GroupCheck.Server;
using System.Collections.Generic;
using System.Collections.Concurrent;

using GroupCheck.Tools.Extensions;

namespace GroupCheck.ServerStorage
{
	public partial class InMemoryStorage : IServerStorageAccounts
	{
		private readonly ConcurrentDictionary<int, Account> _accounts = new ConcurrentDictionary<int, Account>();

		Account IServerStorageAccounts.FindOrCreate(AuthScheme schemeID, string uid)
		{
			var account = _storage.Accounts.FindOrCreate(schemeID, uid);
			return account;
		}
		Account IServerStorageAccounts.Create(Account account)
		{
			var created = _storage.Accounts.Create(account);
			_accounts.TryAdd(created.ID, created);
			return created;
		}
		Account IServerStorageAccounts.Get(int accountID)
		{
			if (_accounts.TryGetValue(accountID, out Account account))
				return account;

			var found = _storage.Accounts.Get(accountID);
			if (found != null)
				_accounts.TryAdd(found.ID, found);

			return found;
		}
		Account IServerStorageAccounts.FindByEmail(string email)
		{
			return _storage.Accounts.FindByEmail(email);
		}
		Account IServerStorageAccounts.Update(Account account)
		{
			var updated = _storage.Accounts.Update(account);
			_accounts.AddOrUpdate(updated.ID, updated, (id, acc) => acc);
			return updated;
		}
		void IServerStorageAccounts.Delete(int accountID)
		{
			_storage.Accounts.Delete(accountID);
			_accounts.TryRemove(accountID, out Account deleted);
		}
	}
}
