using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using GroupCheck.Server;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorageGroups
	{
		Group IServerStorageGroups.Create(Group group)
		{
			if (group == null) return null;

			Group created;

			using (SqlConnection conn = GetConnection())
			{
				conn.Open();
				using (var tran = conn.BeginTransaction())
				{
					try
					{
						// create group
						var cmdCreateGroup = GetCreateGroupSQL(group);
						cmdCreateGroup.Connection = conn;
						cmdCreateGroup.Transaction = tran;
						var newGroupRow = GetDataRow(cmdCreateGroup);
						created = ParseRow<Group>(newGroupRow);
						if (created == null) return null;

						// create members
						var members = new List<Member>();
						foreach (var member in group.Members)
						{
							member.GroupID = created.ID;
							var cmdCreateMember = GetCreateMemberSQL(member);
							cmdCreateMember.Connection = conn;
							cmdCreateMember.Transaction = tran;
							var newMemberRow = GetDataRow(cmdCreateMember);
							var newMember = ParseRow<Member>(newMemberRow);
							members.Add(newMember);
						}

						tran.Commit();
						created.Members = members;
						return created;
					}
					catch (SqlException)
					{
						tran.Rollback();
						throw;
					}
				}
			}
		}
		Group IServerStorageGroups.Get(int groupID)
		{
			using (SqlConnection conn = GetConnection())
			{
				conn.Open();

				var cmd = PrepareSQL("SELECT * FROM Groups WHERE ID = @GroupID", groupID);
				cmd.Connection = conn;
				var group = GetItem<Group>(cmd);

				if (group == null) return null;

				var getListCmd = PrepareSQL("SELECT * FROM Members WHERE GroupID = @GroupID", groupID);
				getListCmd.Connection = conn;
				group.Members = GetList<Member>(getListCmd);

				return group;
			}
		}
		Group IServerStorageGroups.Update(Group group)
		{
			if (group == null) return null;

			var updated = GetItem<Group>(@"
							UPDATE Groups SET
								Name = @Name,
								Confirmed = @Confirmed,
								Created = @Created,
								CreatedByID = @CreatedByID,
								Updated = @Updated,
								UpdatedByID = @UpdatedByID,
								Deleted = @Deleted
							OUTPUT inserted.*
							WHERE ID = @ID ",
							group);

			if (updated == null) return null;

			updated.Members = group.Members;

			return updated;
		}
		void IServerStorageGroups.Delete(int groupID)
		{
			Execute("DELETE FROM Groups WHERE ID = @ID", groupID);
		}
		IEnumerable<Group> IServerStorageGroups.GetGroupsByAccount(int accountID, long minRevision, int batchSize)
		{
			var cmd = PrepareSQL($@"
						SELECT TOP {batchSize + 1} g.*
						FROM Members m
						JOIN Groups g ON g.ID = m.GroupID
						WHERE m.AccountID = @AccountID AND g.Revision > @Revision
						ORDER BY g.Revision",
						new { AccountID = accountID, Revision = minRevision });
			var groups = GetList<Group>(cmd);
			return groups;
		}

		private SqlCommand GetCreateGroupSQL(Group group)
		{
			return PrepareSQL(@"
				INSERT INTO Groups (Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
				OUTPUT inserted.*
				VALUES(@Name, @Confirmed, @Created, @CreatedByID, @Updated, @UpdatedByID, @Deleted)",
				group);
		}
	}
}
