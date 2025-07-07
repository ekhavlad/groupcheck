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
	public class Groups
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

				//==================================================================
				STEP = "CREATE Group";
				var members = new List<NewGroupMember>();
				members.Add(new NewGroupMember() { AccountID = account.ID, Name = account.Name });
				members.Add(new NewGroupMember() { AccountID = null, Name = "Additional account" });
				var newGroup = new NewGroup()
				{
					Name = "test group",
					Members = members
				};
				var group = server.Groups.Create(newGroup.CreateCopy());
				Assert.IsNotNull(group, Messages.NOT_CREATED);
				Assert.Greater(group.ID, 0, "Created ID == 0");
				Assert.Greater(group.Revision, 0, "Created Revision == 0");
				Assert.AreEqual(newGroup.Name, group.Name, STEP);
				Assert.AreEqual(false, group.Confirmed, STEP);

				//==================================================================
				STEP = "CONFIRM Group By ID";
				var confirmed = server.Groups.Confirm(group.ID);
				Assert.IsNotNull(confirmed, Messages.NOT_CONFIRMED);
				Assert.AreEqual(true, confirmed.Confirmed, Messages.NOT_CONFIRMED);
				group.Confirmed = true;
				group.Revision = confirmed.Revision;
				group.Updated = confirmed.Updated;
				ServerEntitiesValidatior.ValidateGroups(confirmed, group, STEP);

				//==================================================================
				STEP = "GET Group By ID";
				var fromDb = server.Groups.Get(group.ID);
				Assert.IsNotNull(fromDb, STEP + Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateGroups(fromDb, group, STEP);

				//==================================================================
				STEP = "UPDATE Group";
				var toUpdate = new UpdateGroup() { ID = group.ID, Name = "UPDATED" };
				var updated = server.Groups.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, group.Revision, STEP);
				ServerEntitiesValidatior.ValidateUpdated(updated.Updated, group.Updated, STEP);
				group.Name = "UPDATED";
				group.Revision = updated.Revision;
				group.Updated = updated.Updated;
				ServerEntitiesValidatior.ValidateGroups(updated, group, STEP);

				//==================================================================
				STEP = "EXIT Group";
				server.Groups.Exit(group.ID);
				Assert.Throws<AccessDeniedException>(() => { var acc = server.Groups.Get(group.ID); }, Messages.NOT_DELETED);
			}
		}

		[Test, Order(1)]
		public void CreateWithMembers()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var account = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(account.ID);

				var member1 = new NewGroupMember() { AccountID = account.ID, Name = "admin" };
				var member2 = new NewGroupMember() { AccountID = null, Name = "member 2" };
				var member3 = new NewGroupMember() { AccountID = null, Name = "member 3" };

				var group = server.Groups.Create(
					new NewGroup()
					{
						Name = "test group",
						Members = new List<NewGroupMember>() { member1, member2, member3 }
					});

				Assert.IsNotNull(group.Members, "Members null");
				Assert.AreEqual(3, group.Members.Count(), "Members count");

				var found1 = group.Members.Single(m => m.Name == member1.Name);
				Assert.IsNotNull(found1, "Member not found");
				Assert.AreEqual(member1.AccountID, found1.AccountID, "Invalid member account ID");
				Assert.AreEqual(group.ID, found1.GroupID, "Invalid member account ID");

				var found2 = group.Members.Single(m => m.Name == member2.Name);
				Assert.IsNotNull(found2, "Member not found");
				Assert.AreEqual(member2.AccountID, found2.AccountID, "Invalid member account ID");
				Assert.AreEqual(group.ID, found2.GroupID, "Invalid member account ID");

				var found3 = group.Members.Single(m => m.Name == member3.Name);
				Assert.IsNotNull(found3, "Member not found");
				Assert.AreEqual(member3.AccountID, found3.AccountID, "Invalid member account ID");
				Assert.AreEqual(group.ID, found3.GroupID, "Invalid member account ID");
			}
		}

		[Test, Order(2)]
		public void AccessToNonConfirmed()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var account = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(account.ID);

				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = account.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "test group", Members = members });

				Assert.Throws<AccessDeniedException>(() => { server.Groups.Get(group.ID); }, "Get");
				Assert.Throws<AccessDeniedException>(() => { server.Groups.Update(new UpdateGroup() { ID = group.ID, Name = "update" }); }, "Update");
				Assert.Throws<AccessDeniedException>(() => { server.Groups.Exit(group.ID); }, "Exit");
			}
		}

		[Test, Order(3)]
		public void CreateWithoutYourself()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;
				var account = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(account.ID);
				Assert.Throws<ValidationException>(() => { server.Groups.Create(new NewGroup() { Name = "test group" }); });
			}
		}
	}
}
