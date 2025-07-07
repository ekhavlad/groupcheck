using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.Server;
using GroupCheck.ServerStorage;
using GroupCheck.Tools.Extensions.RuntimeExt;

namespace GroupCheck.UnitTests.ServerStorage
{
	[TestFixture]
	public class Accounts
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;
				var STEP = "";

				var account = new Account()
				{
					Name = "test account",
					Email = "test.account@sample.com",
					Password = "password",
					Salt = "salt",
					Created = new DateTime(1985, 8, 16, 1, 2, 3),
					Updated = new DateTime(2019, 4, 12, 13, 30, 0),
					Deleted = false
				};

				//==================================================================
				STEP = "CREATE Account";
				var created = storage.Accounts.Create(account.CreateCopy());
				Assert.IsNotNull(created, Messages.NOT_CREATED);
				account.ID = created.ID;
				account.Revision = created.Revision;
				ServerEntitiesValidatior.ValidateAccounts(created, account, STEP);

				//==================================================================
				STEP = "GET Account";
				var fromDb = storage.Accounts.Get(account.ID);
				Assert.IsNotNull(fromDb, Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateAccounts(fromDb, account, STEP);

				//==================================================================
				STEP = "UPDATE Account";
				var toUpdate = fromDb.CreateCopy();
				toUpdate.Name = "UPDATED";
				toUpdate.Password = "UPDATED";
				toUpdate.Salt = "UPDATED";
				toUpdate.BlockedUntil = new DateTime(1944, 9, 13, 4, 5, 6);
				toUpdate.Created = new DateTime(1944, 9, 13, 4, 5, 6);
				toUpdate.Updated = new DateTime(1955, 5, 14, 7, 8, 9);
				toUpdate.Deleted = true;

				var updated = storage.Accounts.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, toUpdate.Revision, STEP);
				ServerEntitiesValidatior.ValidateAccounts(updated, toUpdate, STEP);

				//==================================================================
				STEP = "RESTORE Account";
				var restored = storage.Accounts.Update(account.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_RESTORED);
				ServerEntitiesValidatior.ValidateRevision(restored.Revision, updated.Revision, STEP);
				ServerEntitiesValidatior.ValidateAccounts(restored, account, STEP);

				//==================================================================
				STEP = "DELETE Account";
				storage.Accounts.Delete(account.ID);
				var deleted = storage.Accounts.Get(account.ID);
				Assert.IsNull(deleted, Messages.NOT_DELETED);
			}
		}

		[Test, Order(1)]
		public void Auth()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;

				foreach (var schemeID in Enum.GetValues(typeof(AuthScheme)))
				{
					var schemeName = Enum.GetName(typeof(AuthScheme), schemeID);
					var STEP = schemeName;

					var account1 = storage.Accounts.FindOrCreate((AuthScheme)schemeID, "test");
					Assert.IsNotNull(account1, Messages.NOT_CREATED);

					var account2 = storage.Accounts.FindOrCreate((AuthScheme)schemeID, "test");
					Assert.IsNotNull(account2, Messages.NOT_CREATED);

					ServerEntitiesValidatior.ValidateAccounts(account1, account2, STEP);

					var account3 = storage.Accounts.FindOrCreate((AuthScheme)schemeID, "test2");
					Assert.IsNotNull(account3, Messages.NOT_CREATED);
					Assert.AreNotEqual(account3.ID, account1.ID);
				}
			}
		}
	}
}
