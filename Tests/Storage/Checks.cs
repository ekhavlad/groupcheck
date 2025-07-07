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
	public class Checks
	{
		[Test, Order(0)]
		public void CRUD()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;
				var STEP = "";

				var check = new Check()
				{
					GroupID = 11,
					CheckID = 1,
					DateAndTime = new DateTime(2019, 4, 12),
					Description = "test check",
					Creditors = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } },
					Debitors = new Dictionary<int, int>() { { 8, 7 }, { 6, 5 }, { 4, 3 }, { 2, 1 } },
					Confirmed = false,
					Created = new DateTime(1985, 8, 16, 1, 2, 3),
					CreatedByID = 22,
					Updated = new DateTime(2019, 4, 12, 13, 30, 0),
					UpdatedByID = 33,
					Deleted = false
				};

				//==================================================================
				STEP = "CREATE Check";
				var created = storage.Checks.Create(check.CreateCopy());
				Assert.IsNotNull(created, Messages.NOT_CREATED);
				check.Revision = created.Revision;
				ServerEntitiesValidatior.ValidateChecks(created, check, STEP);

				//==================================================================
				STEP = "GET Check";
				var fromDb = storage.Checks.Get(check.GroupID, check.CheckID);
				Assert.IsNotNull(fromDb, Messages.NOT_FOUND);
				ServerEntitiesValidatior.ValidateChecks(fromDb, check, STEP);

				//==================================================================
				STEP = "UPDATE Check";
				var toUpdate = fromDb.CreateCopy();
				toUpdate.DateAndTime = new DateTime(1985, 8, 16);
				toUpdate.Description = "UPDATED";
				toUpdate.Confirmed = true;
				toUpdate.Created = new DateTime(1944, 9, 13, 4, 5, 6);
				toUpdate.CreatedByID = 555;
				toUpdate.Updated = new DateTime(1955, 5, 14, 7, 8, 9);
				toUpdate.UpdatedByID = 666;
				toUpdate.Creditors = new Dictionary<int, int>() { { 8, 71 }, { 6, 5 }, { 4, 3 }, { 2, 11 } };
				toUpdate.Debitors = new Dictionary<int, int>() { { 1, 21 }, { 3, 4 }, { 5, 6 }, { 7, 81 } };
				toUpdate.Deleted = true;

				var updated = storage.Checks.Update(toUpdate.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_UPDATED);
				ServerEntitiesValidatior.ValidateRevision(updated.Revision, toUpdate.Revision, STEP);
				ServerEntitiesValidatior.ValidateChecks(updated, toUpdate, STEP);

				//==================================================================
				STEP = "RESTORE Check";
				var restored = storage.Checks.Update(check.CreateCopy());
				Assert.IsNotNull(updated, Messages.NOT_RESTORED);
				ServerEntitiesValidatior.ValidateRevision(restored.Revision, updated.Revision, STEP);
				ServerEntitiesValidatior.ValidateChecks(restored, check, STEP);

				//==================================================================
				STEP = "DELETE Check";
				storage.Checks.Delete(check.GroupID, check.CheckID);
				var deleted = storage.Checks.Get(check.GroupID, check.CheckID);
				Assert.IsNull(deleted, Messages.NOT_DELETED);
			}
		}

		[Test, Order(2)]
		public void GetChecksByGroup()
		{
			using (var target = new EmptyStorageOnTest())
			{
				var storage = target.Storage;

				var check1 = storage.Checks.Create(new Check() { GroupID = -1, CheckID = 1, DateAndTime = new DateTime(2019, 1, 1), Description = "check 1", Created = DateTime.Now, Updated = DateTime.Now });
				var check2 = storage.Checks.Create(new Check() { GroupID = -1, CheckID = 2, DateAndTime = new DateTime(2019, 1, 2), Description = "check 2", Created = DateTime.Now, Updated = DateTime.Now });
				var check3 = storage.Checks.Create(new Check() { GroupID = -1, CheckID = 3, DateAndTime = new DateTime(2019, 1, 3), Description = "check 3", Created = DateTime.Now, Updated = DateTime.Now });
				var check4 = storage.Checks.Create(new Check() { GroupID = 0, CheckID = 3, DateAndTime = new DateTime(2019, 1, 4), Description = "check 4", Created = DateTime.Now, Updated = DateTime.Now });

				var checks = storage.Checks.GetChecksByGroup(-1, 0, 5).ToList().OrderBy(g => g.CheckID).ToList();
				Assert.AreEqual(3, checks.Count, "Invalid checks count");
				Assert.AreEqual(check1.CheckID, checks[0].CheckID, "Invalid check ID");
				Assert.AreEqual(check2.CheckID, checks[1].CheckID, "Invalid check ID");
				Assert.AreEqual(check3.CheckID, checks[2].CheckID, "Invalid check ID");

				var minRevision = checks.Min(g => g.Revision);

				checks = storage.Checks.GetChecksByGroup(-1, 0, 1).ToList();
				Assert.AreEqual(2, checks.Count, "Invalid batch size");

				checks = storage.Checks.GetChecksByGroup(-1, minRevision, 5).ToList();
				Assert.AreEqual(2, checks.Count, "Invalid min revision restriction");
			}
		}
	}
}
