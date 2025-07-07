using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.Server;

namespace GroupCheck.UnitTests
{
	public class ServerEntitiesValidatior
	{
		public static void ValidateRevision(long newRevision, long oldRevision, string step)
		{
			Assert.Greater(newRevision, oldRevision, $"{step}: Revision not grown");
		}
		public static void ValidateUpdated(DateTime newDateTime, DateTime oldDateTime, string step)
		{
			Assert.GreaterOrEqual(newDateTime, oldDateTime, $"{step}: Updated timestamp not grown");
		}

		public static void ValidateAccounts(Account actual, Account expected, string step)
		{
			Assert.Greater(expected.Revision, 0, $"{step}: Account Revision");
			Assert.AreEqual(expected.ID, actual.ID, $"{step}: Account ID");
			Assert.AreEqual(expected.Phone, actual.Phone, $"{step}: Account Phone");
			Assert.AreEqual(expected.Name, actual.Name, $"{step}: Account Name");
			Assert.AreEqual(expected.Email, actual.Email, $"{step}: Account Email");
			Assert.AreEqual(expected.Password, actual.Password, $"{step}: Account Password");
			Assert.AreEqual(expected.Salt, actual.Salt, $"{step}: Account Salt");
			Assert.AreEqual(expected.BlockedUntil, actual.BlockedUntil, $"{step}: Account BlockedUntil");
			Assert.AreEqual(expected.Created, actual.Created, $"{step}: Account Registered");
			Assert.AreEqual(expected.Updated, actual.Updated, $"{step}: Account Updated");
			Assert.AreEqual(expected.Deleted, actual.Deleted, $"{step}: Account Deleted");
		}

		public static void ValidateGroups(Group actual, Group expected, string step, bool validateMembers = true)
		{
			Assert.Greater(expected.Revision, 0, $"{step}: Revision");
			Assert.AreEqual(expected.ID, actual.ID, $"{step}: ID");
			Assert.AreEqual(expected.Name, actual.Name, $"{step}: Name");

			if (validateMembers)
			{
				Assert.AreEqual(expected.Members.Count(), actual.Members.Count(), $"{step}: Members Count");
				var expectedMembers = expected.Members.OrderBy(m => m.MemberID).ToArray();
				var actualMembers = actual.Members.OrderBy(m => m.MemberID).ToArray();
				for (var i = 0; i < expectedMembers.Length; i++)
					ValidateMembers(actualMembers[i], expectedMembers[i], step);
			}

			Assert.AreEqual(expected.Confirmed, actual.Confirmed, $"{step} Group Confirmed");
			Assert.AreEqual(expected.Created, actual.Created, $"{step} Group Created");
			Assert.AreEqual(expected.CreatedByID, actual.CreatedByID, $"{step} Group CreatedByID");
			Assert.AreEqual(expected.Updated, actual.Updated, $"{step} Group Updated");
			Assert.AreEqual(expected.UpdatedByID, actual.UpdatedByID, $"{step} Group UpdatedByID");
			Assert.AreEqual(expected.Deleted, actual.Deleted, $"{step} Group Deleted");
		}

		public static void ValidateMembers(Member actual, Member expected, string step)
		{
			Assert.Greater(expected.Revision, 0, $"{step}: Member Revision");
			Assert.AreEqual(expected.MemberID, actual.MemberID, $"{step}: Member ID");
			Assert.AreEqual(expected.GroupID, actual.GroupID, $"{step}: Member GroupID");
			Assert.AreEqual(expected.AccountID, actual.AccountID, $"{step}: Member AccountID");
			Assert.AreEqual(expected.Name, actual.Name, $"{step}: Member Name");
			Assert.AreEqual(expected.Confirmed, actual.Confirmed, $"{step}: Member Confirmed");
			Assert.AreEqual(expected.Created, actual.Created, $"{step}: Member Created");
			Assert.AreEqual(expected.CreatedByID, actual.CreatedByID, $"{step}: Member CreatedByID");
			Assert.AreEqual(expected.Updated, actual.Updated, $"{step}: Member Updated");
			Assert.AreEqual(expected.UpdatedByID, actual.UpdatedByID, $"{step}: Member UpdatedByID");
			Assert.AreEqual(expected.Deleted, actual.Deleted, $"{step}: Member Deleted");
		}

		public static void ValidateChecks(Check actual, Check expected, string step)
		{
			Assert.Greater(expected.Revision, 0, $"{step}: Check Revision");
			Assert.AreEqual(expected.CheckID, actual.CheckID, $"{step}: Check ID");
			Assert.AreEqual(expected.GroupID, actual.GroupID, $"{step}: Check GroupID");
			Assert.AreEqual(expected.DateAndTime, actual.DateAndTime, $"{step}: Check DateAndTime");
			Assert.AreEqual(expected.Description, actual.Description, $"{step}: Check Description");
			Assert.AreEqual(expected.Confirmed, actual.Confirmed, $"{step}: Check Confirmed");
			Assert.AreEqual(expected.Created, actual.Created, $"{step}: Check Created");
			Assert.AreEqual(expected.CreatedByID, actual.CreatedByID, $"{step}: Check CreatedByID");
			Assert.AreEqual(expected.Updated, actual.Updated, $"{step}: Check Updated");
			Assert.AreEqual(expected.UpdatedByID, actual.UpdatedByID, $"{step}: Check UpdatedByID");
			Assert.AreEqual(Hash(expected.Creditors), Hash(actual.Creditors), $"{step}: Check Creditors");
			Assert.AreEqual(Hash(expected.Debitors), Hash(actual.Debitors), $"{step}: Check Debitors");
			Assert.AreEqual(expected.Deleted, actual.Deleted, $"{step}: Check Deleted");
		}

		private static string Hash(IDictionary<int, int> dict)
		{
			if (dict == null) return null;
			return string.Join("::", dict.OrderBy(d => d.Key).ThenBy(d => d.Value).Select(d => $"{d.Key}-{d.Value}"));
		}
	}
}
