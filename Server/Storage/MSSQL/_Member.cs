using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using GroupCheck.Server;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorageMembers
	{
		Member IServerStorageMembers.Create(Member member)
		{
			if (member == null) return null;

			var cmd = GetCreateMemberSQL(member);
			var created = GetItem<Member>(cmd);
			return created;
		}
		Member IServerStorageMembers.Get(int groupID, int memberID)
		{
			var member = GetItem<Member>($"SELECT * FROM Members WHERE GroupID = @GroupID AND MemberID = @MemberID", new { GroupID = groupID, MemberID = memberID });
			return member;
		}
		Member IServerStorageMembers.Update(Member member)
		{
			if (member == null) return null;

			var updated = GetItem<Member>(@"
							UPDATE Members SET
								AccountID = @AccountID,
								Name = @Name,
								Confirmed = @Confirmed,
								Created = @Created,
								CreatedByID = @CreatedByID,
								Updated = @Updated,
								UpdatedByID = @UpdatedByID,
								Deleted = @Deleted
							OUTPUT inserted.*
							WHERE GroupID = @GroupID AND MemberID = @MemberID",
							member);

			return updated;
		}
		void IServerStorageMembers.Delete(int groupID, int memberID)
		{
			Execute("DELETE FROM Members WHERE GroupID = @GroupID AND MemberID = @MemberID", new { GroupID = groupID, MemberID = memberID });
		}
		IEnumerable<Member> IServerStorageMembers.GetMembersByGroup(int groupID, long minRevision, int batchSize)
		{
			var cmd = PrepareSQL($"SELECT TOP {batchSize + 1} * FROM Members WHERE GroupID = @GroupID AND Revision > @Revision ORDER BY Revision",
						new { GroupID = groupID, Revision = minRevision });
			var members = GetList<Member>(cmd);
			return members;
		}

		private SqlCommand GetCreateMemberSQL(Member member)
		{
			return PrepareSQL(@"
				INSERT INTO Members (GroupID, MemberID, AccountID, Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
				OUTPUT inserted.*
				VALUES (@GroupID, (SELECT ISNULL(MAX(MemberID), 0) FROM Members WHERE GroupID = @GroupID) + 1, @AccountID, @Name, @Confirmed, @Created, @CreatedByID, @Updated, @UpdatedByID, @Deleted);",
				member);
		}
	}
}
