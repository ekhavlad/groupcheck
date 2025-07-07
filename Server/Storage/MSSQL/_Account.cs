using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using GroupCheck.Server;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorageAccounts
	{
		private static Dictionary<AuthScheme, string> authColumns = new Dictionary<AuthScheme, string>()
		{ {AuthScheme.Phone, "Phone"} };

		Account IServerStorageAccounts.FindOrCreate(AuthScheme schemeID, string uid)
		{
			var column = authColumns[schemeID];
			var account = GetItem<Account>($"SELECT * FROM Accounts WHERE [{column}] = @UID", uid);
			if (account == null)
			{
				account = GetItem<Account>($@"
							INSERT INTO Accounts (Created, Updated, [{column}])
							OUTPUT inserted.*
							VALUES(@Created, @Updated, @UID)",
							new { Created = DateTime.UtcNow, Updated = DateTime.UtcNow, UID = uid });
			}

			return account;
		}
		Account IServerStorageAccounts.FindByEmail(string email)
		{
			var account = GetItem<Account>("SELECT * FROM Accounts WHERE Email = @Email", email);
			return account;
		}
		Account IServerStorageAccounts.Create(Account account)
		{
			if (account == null) return null;

			var created = GetItem<Account>(@"
							INSERT INTO Accounts (Phone, Email, Name, Salt, Password, BlockedUntil, Created, Updated, Deleted)
							OUTPUT inserted.*
							VALUES (@Phone, @Email, @Name, @Salt, @Password, @BlockedUntil, @Created, @Updated, @Deleted);",
							account);

			return created;
		}
		Account IServerStorageAccounts.Get(int accountID)
		{
			var account = GetItem<Account>("SELECT * FROM Accounts WHERE ID = @ID", accountID);
			return account;
		}
		Account IServerStorageAccounts.Update(Account account)
		{
			if (account == null) return null;

			var updated = GetItem<Account>(@"
							UPDATE Accounts SET
								Phone = @Phone,
								Email = @Email,
								Name = @Name,
								Salt = @Salt,
								Password = @Password,
								BlockedUntil = @BlockedUntil,
								Created = @Created,
								Updated = @Updated,
								Deleted = @Deleted
							OUTPUT inserted.*
							WHERE ID = @ID ",
							account);

			return updated;
		}
		void IServerStorageAccounts.Delete(int accountID)
		{
			Execute("DELETE FROM Accounts WHERE ID = @ID", accountID);
		}
	}
}
