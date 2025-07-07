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
	public class Checks
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var STEP = "";

				var account = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(account.ID);
				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = account.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "group", Members = members });
				server.Groups.Confirm(group.ID);

				//==================================================================
				STEP = "CREATE Check";
				var newCheck = new NewCheck()
				{
					GroupID = group.ID,
					DateAndTime = DateTime.Today,
					Description = "test check"
				};
				var check = server.Checks.Create(newCheck.CreateCopy());
				Assert.IsNotNull(check, Messages.NOT_CREATED);
				Assert.Greater(check.CheckID, 0, "Created ID == 0");
				Assert.Greater(check.Revision, 0, "Created Revision == 0");
				Assert.AreEqual(newCheck.Description, check.Description, STEP);
				Assert.AreEqual(newCheck.DateAndTime, check.DateAndTime, STEP);
				Assert.AreEqual(false, check.Confirmed, STEP);

				//==================================================================
				STEP = "CONFIRM Check By ID";
				var confirmed = server.Checks.Confirm(check.GroupID, check.CheckID);
				Assert.IsNotNull(confirmed, Messages.NOT_CONFIRMED);
				Assert.AreEqual(true, confirmed.Confirmed, Messages.NOT_CONFIRMED);
				check.Confirmed = true;
				check.Revision = confirmed.Revision;
				check.Updated = confirmed.Updated;
				ServerEntitiesValidatior.ValidateChecks(confirmed, check, STEP);

				//==================================================================
				STEP = "GET Check By ID";
				var fromDb = server.Checks.Get(check.GroupID, check.CheckID);
				Assert.IsNotNull(fromDb, STEP + Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateChecks(fromDb, check, STEP);

				//==================================================================
				STEP = "UPDATE Check";
				var toUpdate = new UpdateCheck() { GroupID = check.GroupID, CheckID = check.CheckID, Description = "UPDATED", DateAndTime = DateTime.Today.AddHours(1) };
				var updated = server.Checks.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, check.Revision, STEP);
				ServerEntitiesValidatior.ValidateUpdated(updated.Updated, check.Updated, STEP);
				check.Description = toUpdate.Description;
				check.DateAndTime = toUpdate.DateAndTime;
				check.Revision = updated.Revision;
				check.Updated = updated.Updated;
				ServerEntitiesValidatior.ValidateChecks(updated, check, STEP);

				//==================================================================
				STEP = "DELETE Check";
				server.Checks.Delete(check.GroupID, check.CheckID);
				Assert.Throws<DeletedException>(() => { var acc = server.Checks.Get(check.GroupID, check.CheckID); }, Messages.NOT_DELETED);
			}
		}

		[Test, Order(1)]
		public void AccessToNonConfirmed()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var account = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(account.ID);

				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = account.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "group", Members = members });
				server.Groups.Confirm(group.ID);
				var check = server.Checks.Create(new NewCheck() { GroupID = group.ID, DateAndTime = DateTime.Today, Description = "test" });

				Assert.Throws<AccessDeniedException>(() => { server.Checks.Get(check.GroupID, check.CheckID); }, "Get");
				Assert.Throws<AccessDeniedException>(() => { server.Checks.Update(new UpdateCheck() { GroupID = check.GroupID, CheckID = check.CheckID, Description = "update", DateAndTime = DateTime.Today.AddHours(1) }); }, "Update");
				Assert.Throws<AccessDeniedException>(() => { server.Checks.Delete(check.GroupID, check.CheckID); }, "Delete");
			}
		}

		[Test, Order(2)]
		public void ConfirmByNonAuthor()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				var author = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(author.ID);

				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = author.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "group", Members = members });
				server.Groups.Confirm(group.ID);
				var check = server.Checks.Create(new NewCheck() { GroupID = group.ID, DateAndTime = DateTime.Today, Description = "test" });

				var user = server.Accounts.Register(new NewAccount() { Email = "user@sample.com", Password = "123" });
				server.SetCurrentAccount(user.ID);

				Assert.Throws<AccessDeniedException>(() => { server.Checks.Confirm(check.GroupID, check.CheckID); });
			}
		}
	}
}
