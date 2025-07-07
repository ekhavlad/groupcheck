using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;
using GroupCheck.ServerStorage;
using System.Threading;

namespace GroupCheck.Server
{
	public partial class Core : IServer, IServerAccounts, IServerGroups, IServerMembers, IServerChecks
	{
		private readonly IServerStorage storage;

		private const int ANONYMOUS = 0;
		private Account _currentAccount;
		public Account CurrentAccount { get { return _currentAccount; } }

		public Core(IServerStorage storage) :
			this(ANONYMOUS, storage)
		{ }

		public Core(int accountID, IServerStorage storage)
		{
			this.storage = storage;
			SetCurrentAccount(accountID);
		}

		public void SetCurrentAccount(int accountID)
		{
			if (accountID == ANONYMOUS)
			{
				_currentAccount = null;
				return;
			}

			var account = storage.Accounts.Get(accountID);
			RequireExistance(account);

			if (account.BlockedUntil.HasValue)
			{ 
				if (account.BlockedUntil > Now())
					throw new AccountBlockedException(account.BlockedUntil.Value);

				account.BlockedUntil = null;
				account = storage.Accounts.Update(account);
			}

			_currentAccount = account;
		}
		public void WorkAsAnonymous()
		{
			SetCurrentAccount(ANONYMOUS);
		}
		public bool IsAnonymous { get { return _currentAccount == null; } }

		public IServerAccounts Accounts { get { return this; } }
		public IServerGroups Groups { get { return this; } }
		public IServerMembers Members { get { return this; } }
		public IServerChecks Checks { get { return this; } }

		private DateTime Now()
		{
			return DateTime.UtcNow;
		}
	}
}
