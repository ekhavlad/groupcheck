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
	public class Groups
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;
				var STEP = "";

				var group = new Group()
				{
					Name = "test group",
					Members = new List<Member>()
				{
					new Member()
					{
						MemberID = 1,
						AccountID = 1,
						Name = "member 1",
						Confirmed = false,
						Created = new DateTime(2000, 1, 1, 1, 2, 3),
						CreatedByID = 123,
						Updated = new DateTime(2001, 2, 3, 4, 5, 6),
						UpdatedByID = 456,
						Deleted = true
					},
					new Member()
					{
						MemberID = 2,
						AccountID = null,
						Name = "member 2",
						Confirmed = true,
						Created = new DateTime(2002, 4, 5, 6, 7, 8),
						CreatedByID = 111,
						Updated = new DateTime(2003, 1, 2, 3, 4, 5),
						UpdatedByID = 222,
						Deleted = false
					}
				},
					Confirmed = false,
					Created = new DateTime(1985, 8, 16, 1, 2, 3),
					CreatedByID = 1,
					Updated = new DateTime(2019, 4, 12, 13, 30, 0),
					UpdatedByID = 2,
					Deleted = false
				};

				//==================================================================
				STEP = "CREATE Group";
				var created = storage.Groups.Create(group.CreateCopy());
				Assert.IsNotNull(created, Messages.NOT_CREATED);
				group.ID = created.ID;
				group.Revision = created.Revision;
				Assert.AreEqual(group.Members.Count(), created.Members.Count(), "Created members not equal to expected");
				foreach (var m in created.Members)
				{
					var member = group.Members.Single(_ => _.AccountID == m.AccountID && _.Name == m.Name && _.MemberID == m.MemberID);
					member.GroupID = m.GroupID;
					member.Revision = m.Revision;
				}
				ServerEntitiesValidatior.ValidateGroups(created, group, STEP);

				//==================================================================
				STEP = "GET Group With members";
				var fromDb = storage.Groups.Get(group.ID);
				Assert.IsNotNull(fromDb, Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateGroups(fromDb, group, STEP);

				//==================================================================
				STEP = "UPDATE Group";
				var toUpdate = fromDb.CreateCopy();
				toUpdate.Name = "UPDATED";
				toUpdate.Confirmed = true;
				toUpdate.Created = new DateTime(1944, 9, 13, 4, 5, 6);
				toUpdate.CreatedByID = 3;
				toUpdate.Updated = new DateTime(1955, 5, 14, 7, 8, 9);
				toUpdate.UpdatedByID = 4;
				toUpdate.Members = new List<Member>();
				toUpdate.Deleted = true;

				var updated = storage.Groups.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, toUpdate.Revision, STEP);
				ServerEntitiesValidatior.ValidateGroups(updated, toUpdate, STEP, false);

				//==================================================================
				STEP = "RESTORE Group";
				var restored = storage.Groups.Update(group.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_RESTORED);
				ServerEntitiesValidatior.ValidateRevision(restored.Revision, updated.Revision, STEP);
				ServerEntitiesValidatior.ValidateGroups(restored, group, STEP, false);

				//==================================================================
				STEP = "DELETE Group";
				storage.Groups.Delete(group.ID);
				var deleted = storage.Groups.Get(group.ID);
				Assert.IsNull(deleted, Messages.NOT_DELETED);
			}
		}

		[Test, Order(2)]
		public void GetGroupsByAccount()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;

				var account = new Account()
				{
					Email = "test@sample.com",
					Name = "test account",
					Password = "123",
					Salt = "123",
					Created = DateTime.Now,
					Updated = DateTime.Now
				};
				account = storage.Accounts.Create(account);

				var member1 = new Member() { MemberID = 1, AccountID = null, Name = "anonim 1", Created = DateTime.Now, Updated = DateTime.Now };
				var member2 = new Member() { MemberID = 2, AccountID = account.ID, Name = "user", Created = DateTime.Now, Updated = DateTime.Now };

				var group1 = new Group() { Name = "test group 1", Members = new List<Member>() { member1, member2 }, Created = DateTime.Now, Updated = DateTime.Now };
				var group2 = new Group() { Name = "test group 2", Members = new List<Member>() { member1, member2 }, Created = DateTime.Now, Updated = DateTime.Now };
				var group3 = new Group() { Name = "test group 3", Members = new List<Member>() { member1 }, Created = DateTime.Now, Updated = DateTime.Now };
				var group4 = new Group() { Name = "test group 4", Members = new List<Member>() { member2 }, Created = DateTime.Now, Updated = DateTime.Now };

				group1 = storage.Groups.Create(group1);
				group2 = storage.Groups.Create(group2);
				group3 = storage.Groups.Create(group3);
				group4 = storage.Groups.Create(group4);

				var groups = storage.Groups.GetGroupsByAccount(account.ID, 0, 5).ToList().OrderBy(g => g.ID).ToList();
				Assert.AreEqual(3, groups.Count, "Invalid groups count");
				Assert.AreEqual(group1.ID, groups[0].ID, "Invalid group");
				Assert.AreEqual(group2.ID, groups[1].ID, "Invalid group");
				Assert.AreEqual(group4.ID, groups[2].ID, "Invalid group");

				var minRevision = groups.Min(g => g.Revision);

				groups = storage.Groups.GetGroupsByAccount(account.ID, 0, 1).ToList();
				Assert.AreEqual(2, groups.Count, "Invalid batch size");

				groups = storage.Groups.GetGroupsByAccount(account.ID, minRevision, 5).ToList();
				Assert.AreEqual(2, groups.Count, "Invalid min revision restriction");
			}
		}
	}
}
