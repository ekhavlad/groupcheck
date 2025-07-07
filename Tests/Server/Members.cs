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
	public class Members
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
				STEP = "CREATE Member";
				var newMember = new NewMember()
				{
					GroupID = group.ID,
					Name = "test group"
				};
				var member = server.Members.Create(newMember.CreateCopy());
				Assert.IsNotNull(member, Messages.NOT_CREATED);
				Assert.Greater(member.MemberID, 0, "Created ID == 0");
				Assert.Greater(member.Revision, 0, "Created Revision == 0");
				Assert.AreEqual(newMember.Name, member.Name, STEP);
				Assert.AreEqual(false, member.Confirmed, STEP);

				//==================================================================
				STEP = "CONFIRM Member By ID";
				var confirmed = server.Members.Confirm(member.GroupID, member.MemberID);
				Assert.IsNotNull(confirmed, Messages.NOT_CONFIRMED);
				Assert.AreEqual(true, confirmed.Confirmed, Messages.NOT_CONFIRMED);
				member.Confirmed = true;
				member.Revision = confirmed.Revision;
				member.Updated = confirmed.Updated;
				ServerEntitiesValidatior.ValidateMembers(confirmed, member, STEP);

				//==================================================================
				STEP = "GET Member By ID";
				var fromDb = server.Members.Get(member.GroupID, member.MemberID);
				Assert.IsNotNull(fromDb, STEP + Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateMembers(fromDb, member, STEP);

				//==================================================================
				STEP = "UPDATE Member";
				var toUpdate = new UpdateMember() { GroupID = member.GroupID, MemberID = member.MemberID, Name = "UPDATED" };
				var updated = server.Members.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, member.Revision, STEP);
				ServerEntitiesValidatior.ValidateUpdated(updated.Updated, member.Updated, STEP);
				member.Name = "UPDATED";
				member.Revision = updated.Revision;
				member.Updated = updated.Updated;
				ServerEntitiesValidatior.ValidateMembers(updated, member, STEP);

				//==================================================================
				STEP = "DELETE Member";
				server.Members.Delete(member.GroupID, member.MemberID);
				Assert.Throws<DeletedException>(() => { var acc = server.Members.Get(member.GroupID, member.MemberID); }, Messages.NOT_DELETED);
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

				var member = server.Members.Create(new NewMember() { GroupID = group.ID, Name = "member" });

				//Assert.Throws<AccessDeniedException>(() => { server.Members.Get(member.GroupID, member.MemberID); }, "Get");
				//Assert.Throws<AccessDeniedException>(() => { server.Members.Update(new UpdateMember() { GroupID = member.GroupID, MemberID = member.MemberID, Name = "update" }); }, "Update");
				//Assert.Throws<AccessDeniedException>(() => { server.Members.Delete(member.GroupID, member.MemberID); }, "Delete");
			}
		}

		[Test, Order(2)]
		public void ConfirmByNotFromGroup()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				var author = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(author.ID);

				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = author.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "group", Members = members });
				server.Groups.Confirm(group.ID);

				var member = server.Members.Create(new NewMember() { GroupID = group.ID, Name = "member" });

				var user = server.Accounts.Register(new NewAccount() { Email = "user@sample.com", Password = "123" });
				server.SetCurrentAccount(user.ID);

				Assert.Throws<AccessDeniedException>(() => { server.Members.Confirm(member.GroupID, member.MemberID); });
			}
		}

		[Test, Order(2)]
		public void ConfirmByFromGroup()
		{
			using (var target = new EmptyServerOnTest())
			{
				var server = target.Server;

				var author = server.Accounts.Register(new NewAccount() { Email = "admin@sample.com", Password = "123" });
				server.SetCurrentAccount(author.ID);

				var members = new List<NewGroupMember>() { new NewGroupMember() { AccountID = author.ID, Name = "admin" } };
				var group = server.Groups.Create(new NewGroup() { Name = "group", Members = members });
				server.Groups.Confirm(group.ID);

				var secondUser = server.Accounts.Register(new NewAccount() { Email = "user@sample.com", Password = "123" });

				var member = server.Members.Create(new NewMember() { GroupID = group.ID, AccountID = secondUser.ID, Name = "second user" });

				server.SetCurrentAccount(secondUser.ID);

				server.Members.Confirm(member.GroupID, member.MemberID);
			}
		}
	}
}
