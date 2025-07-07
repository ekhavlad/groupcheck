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
	public class Members
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;
				var STEP = "";

				var member = new Member()
				{
					GroupID = 357,
					MemberID = 1,
					AccountID = 271,
					Name = "test member",
					Confirmed = false,
					Revision = 0,
					Created = new DateTime(1985, 8, 16, 1, 2, 3),
					CreatedByID = 1,
					Updated = new DateTime(2019, 4, 12, 13, 30, 0),
					UpdatedByID = 2,
					Deleted = false
				};

				//==================================================================
				STEP = "CREATE Member";
				var created = storage.Members.Create(member.CreateCopy());
				Assert.IsNotNull(created, Messages.NOT_CREATED);
				member.Revision = created.Revision;
				ServerEntitiesValidatior.ValidateMembers(created, member, STEP);

				//==================================================================
				STEP = "GET Member";
				var fromDb = storage.Members.Get(member.GroupID, member.MemberID);
				Assert.IsNotNull(fromDb, Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateMembers(fromDb, member, STEP);

				//==================================================================
				STEP = "UPDATE Member";
				var toUpdate = fromDb.CreateCopy();
				toUpdate.AccountID = 111;
				toUpdate.Name = "updated";
				toUpdate.Confirmed = true;
				toUpdate.Created = new DateTime(1944, 9, 13, 4, 5, 6);
				toUpdate.CreatedByID = 3;
				toUpdate.Updated = new DateTime(1955, 5, 14, 7, 8, 9);
				toUpdate.UpdatedByID = 4;
				toUpdate.Deleted = true;

				var updated = storage.Members.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, toUpdate.Revision, STEP);
				ServerEntitiesValidatior.ValidateMembers(updated, toUpdate, STEP);

				//==================================================================
				STEP = "RESTORE Member";
				var restored = storage.Members.Update(member.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_RESTORED);
				ServerEntitiesValidatior.ValidateRevision(restored.Revision, updated.Revision, STEP);
				ServerEntitiesValidatior.ValidateMembers(restored, member, STEP);

				//==================================================================
				STEP = "DELETE Member";
				storage.Members.Delete(member.GroupID, member.MemberID);
				var deleted = storage.Members.Get(member.GroupID, member.MemberID);
				Assert.IsNull(deleted, Messages.NOT_DELETED);
			}
		}

		[Test, Order(2)]
		public void GetMembersByGroup()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;

				var member1 = new Member() { MemberID = 1, AccountID = null, Name = "anonim 1", Created = DateTime.Now, Updated = DateTime.Now };
				var member2 = new Member() { MemberID = 2, AccountID = null, Name = "anonim 2", Created = DateTime.Now, Updated = DateTime.Now };
				var member3 = new Member() { MemberID = 3, AccountID = 7, Name = "user 1", Created = DateTime.Now, Updated = DateTime.Now };
				var member4 = new Member() { MemberID = 4, AccountID = 9, Name = "user 2", Created = DateTime.Now, Updated = DateTime.Now };

				var group = new Group()
				{
					Name = "test group 1",
					Members = new List<Member>() { member1, member2, member3, member4 },
					Created = DateTime.Now,
					Updated = DateTime.Now
				};

				group = storage.Groups.Create(group);

				var members = storage.Members.GetMembersByGroup(group.ID, 0, 5).ToList().OrderBy(g => g.Name).ToList();
				Assert.AreEqual(4, members.Count, "Invalid members count");
				Assert.AreEqual(member1.Name, members[0].Name, "Invalid member name");
				Assert.AreEqual(member2.Name, members[1].Name, "Invalid member name");
				Assert.AreEqual(member3.Name, members[2].Name, "Invalid member name");
				Assert.AreEqual(member4.Name, members[3].Name, "Invalid member name");

				var minRevision = members.Min(g => g.Revision);

				members = storage.Members.GetMembersByGroup(group.ID, 0, 1).ToList();
				Assert.AreEqual(2, members.Count, "Invalid batch size");

				members = storage.Members.GetMembersByGroup(group.ID, minRevision, 5).ToList();
				Assert.AreEqual(3, members.Count, "Invalid min revision restriction");
			}
		}
	}
}
