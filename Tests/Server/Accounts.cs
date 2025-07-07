using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.Server;
using GroupCheck.Tools.Extensions.RuntimeExt;
using GroupCheck.UnitTests.ServerStorage;

namespace GroupCheck.UnitTests.Server
{
	[TestFixture]
	public class Accounts
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var STEP = "";

				//==================================================================
				STEP = "CREATE Account";
				var newAccount = new NewAccount()
				{
					Email = "test.account@sample.com",
					Name = "test name",
					Password = "pass"
				};
				var account = server.Accounts.Register(newAccount.CreateCopy());
				Assert.IsNotNull(account, Messages.NOT_CREATED);
				Assert.Greater(account.ID, 0, "Created ID == 0");
				Assert.Greater(account.Revision, 0, "Created Revision == 0");
				Assert.AreEqual(newAccount.Email, account.Email, STEP);
				Assert.AreEqual(newAccount.Name, account.Name, STEP);

				server.SetCurrentAccount(account.ID);

				//==================================================================
				STEP = "GET Account By ID";
				var fromDb = server.Accounts.Get(account.ID);
				Assert.IsNotNull(fromDb, STEP + Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateAccounts(fromDb, account, STEP);

				//==================================================================
				STEP = "GET Current Account";
				var current = server.Accounts.GetCurrent();
				Assert.IsNotNull(current, STEP + Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateAccounts(current, account, STEP);

				//==================================================================
				Assert.Throws<NotFoundException>(() => { var acc = server.Accounts.Get(-1); }, "Found not existing account!");

				//==================================================================
				STEP = "UPDATE Account";
				var toUpdate = new UpdateAccount() { Name = "UPDATED" };
				var updated = server.Accounts.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, account.Revision, STEP);
				ServerEntitiesValidatior.ValidateUpdated(updated.Updated, account.Updated, STEP);
				account.Name = "UPDATED";
				account.Revision = updated.Revision;
				account.Updated = updated.Updated;
				ServerEntitiesValidatior.ValidateAccounts(updated, account, STEP);

				//==================================================================
				STEP = "CHANGEPASS";
				var newPass = server.Accounts.ChangePassword("UPDATED");
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(newPass.Revision, account.Revision, STEP);
				ServerEntitiesValidatior.ValidateUpdated(newPass.Updated, account.Updated, STEP);
				Assert.AreNotEqual(newPass.Salt, account.Salt, STEP + Messages.NOT_UPDATED);
				Assert.AreNotEqual(newPass.Password, account.Password, STEP + Messages.NOT_UPDATED);
				account.Password = newPass.Password;
				account.Salt = newPass.Salt;
				account.Revision = newPass.Revision;
				account.Updated = newPass.Updated;
				ServerEntitiesValidatior.ValidateAccounts(newPass, account, STEP);

				//==================================================================
				STEP = "DELETE Account";
				server.Accounts.Delete();
				Assert.Throws<AuthenticationRequiredException>(() => { var acc = server.Accounts.GetCurrent(); });
				Assert.Throws<AuthenticationRequiredException>(() => { var acc = server.Accounts.Get(account.ID); });
			}
		}

		[Test, Order(1)]
		public void CreateDuplicateEmail()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				var acc1 = new NewAccount()
				{
					Email = "test.account@sample.com",
					Name = "test name 1",
					Password = "pass"
				};

				var acc2 = new NewAccount()
				{
					Email = "test.account@sample.com",
					Name = "test name 2",
					Password = "pass"
				};

				var created1 = server.Accounts.Register(acc1);
				Assert.Throws<AlreadyExistsException>(() => { var created2 = server.Accounts.Register(acc2); });
			}
		}

		[Test, Order(2)]
		public void AnonymousAccess()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.ChangePassword("UPDATED"); }, "ChangePassword");
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.GetCurrent(); }, "GetAccount");
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.Get(1); }, "GetAccount");
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.Delete(); }, "DeleteAccount");
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.FindByEmail("email"); }, "FindAccount");
				Assert.Throws<AuthenticationRequiredException>(() => { server.Accounts.Update(new UpdateAccount() { }); }, "UpdateAccount");
			}
		}

		[Test, Order(3)]
		public void Authentication()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				var newAccount = new NewAccount()
				{
					Email = "test.account@sample.com",
					Name = "test name",
					Password = "pass"
				};
				var account = server.Accounts.Register(newAccount.CreateCopy());

				Assert.Throws<NotFoundException>(() => { server.Accounts.ValidateEmail("invalid.email@sample.com", "invalid.password"); }, "invalid.email");
				Assert.Throws<ValidationException>(() => { server.Accounts.ValidateEmail("test.account@sample.com", "invalid.password"); }, "invalid.password");

				var authed = server.Accounts.ValidateEmail("test.account@sample.com", "pass");
				Assert.IsNotNull(authed, Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateAccounts(authed, account, "AUTH");
			}
		}
		[Test, Order(4)]
		public void RegisterInvalidEmail()
		{
			var emails = new List<string>()
			{
				"test1(atas@sample.com",
				"test1<atasa@sample.com",
				"test1<atas>@sample.com",
				"test1atas)@sample.com",
				"test1@sample;",
			};

			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				foreach (var email in emails)
				{
					Assert.Throws<ValidationException>(() => { server.Accounts.Register(new NewAccount() { Email = email }); }, email);
				}
			}
		}

		[Test, Order(5)]
		public void FindByEmail()
		{
			var emails = new List<string>()
			{
				"test1@sample.com",
				"Test_2@sample.com",
				"Test.3@sample.com",
				"Test4@sample.com",
			};
			var created = new Dictionary<string, Account>();

			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var account = server.Accounts.Register(
					new NewAccount()
					{
						Email = "test.account@sample.com",
						Name = "test name",
						Password = "pass"
					});
				server.SetCurrentAccount(account.ID);


				foreach (var email in emails)
				{
					created[email] = server.Accounts.Register(new NewAccount() { Email = email, Name = email.Split('@')[0], Password = "123" });
				}

				foreach (var email in emails)
				{
					var found = server.Accounts.FindByEmail(email.ToUpper());
					Assert.IsNotNull(found, Messages.NOT_FOUND + ":" + email);
					ServerEntitiesValidatior.ValidateAccounts(found, created[email], email);
				}

				var notExisting = server.Accounts.FindByEmail("notexisting@sample.com");
				Assert.IsNull(notExisting, "Found not existing account");
			}
		}
	}
}
