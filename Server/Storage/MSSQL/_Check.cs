using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using GroupCheck.Server;
using GroupCheck.Tools.Extensions.RuntimeExt;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorageChecks
	{
		Check IServerStorageChecks.Create(Check check)
		{
			if (check == null) return null;

			var created = GetItem<DBCheck>(@"
							INSERT INTO Checks (GroupID, CheckID, DateAndTime, Description, Creditors, Debitors, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
							OUTPUT inserted.*
							VALUES (@GroupID, (SELECT ISNULL(MAX(CheckID), 0) FROM Checks WHERE GroupID = @GroupID) + 1, @DateAndTime, @Description, @Creditors, @Debitors, @Confirmed, @Created, @CreatedByID, @Updated, @UpdatedByID, @Deleted);",
							check.ConvertTo<DBCheck>());

			return created;
		}
		Check IServerStorageChecks.Get(int groupID, int checkID)
		{
			var check = GetItem<DBCheck>($"SELECT * FROM Checks WHERE GroupID = @GroupID AND CheckID = @CheckID",
							new { GroupID = groupID, CheckID = checkID });

			return check;
		}
		Check IServerStorageChecks.Update(Check check)
		{
			if (check == null) return null;

			var updated = GetItem<DBCheck>(@"
							UPDATE Checks SET
								DateAndTime = @DateAndTime,
								Description = @Description,
								Confirmed = @Confirmed,
								Creditors = @Creditors,
								Debitors = @Debitors,
								Created = @Created,
								CreatedByID = @CreatedByID,
								Updated = @Updated,
								UpdatedByID = @UpdatedByID,
								Deleted = @Deleted
							OUTPUT inserted.*
							WHERE GroupID = @GroupID AND CheckID = @CheckID",
							check.ConvertTo<DBCheck>());

			return updated;
		}
		void IServerStorageChecks.Delete(int groupID, int checkID)
		{
			Execute("DELETE FROM Checks WHERE GroupID = @GroupID AND CheckID = @CheckID", new { GroupID = groupID, CheckID = checkID });
		}
		IEnumerable<Check> IServerStorageChecks.GetChecksByGroup(int groupID, long minRevision, int batchSize)
		{
			var cmd = PrepareSQL($"SELECT TOP {batchSize + 1} * FROM Checks WHERE GroupID = @GroupID AND Revision > @Revision ORDER BY Revision",
				new { GroupID = groupID, Revision = minRevision });
			var checks = GetList<DBCheck>(cmd);
			var result = checks.Select(_ => (Check)_).ToList();
			return result;
		}

		class DBCheck : Check
		{
			public string DB_Creditors
			{
				get
				{
					if (base.Creditors == null)
						return string.Empty;
					else
						return string.Join(",", base.Creditors.Select(m => $"{m.Key}:{m.Value}"));
				}
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						base.Creditors = new Dictionary<int, int>();
					}
					else
					{
						base.Creditors = value.Split(',').ToDictionary(
							x => int.Parse(x.Split(':')[0]),
							x => int.Parse(x.Split(':')[1]));
					}
				}
			}
			public string DB_Debitors
			{
				get
				{
					if (base.Debitors == null)
						return string.Empty;
					else
						return string.Join(",", base.Debitors.Select(m => $"{m.Key}:{m.Value}"));
				}
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						base.Debitors = new Dictionary<int, int>();
					}
					else
					{
						base.Debitors = value.Split(',').ToDictionary(
							x => int.Parse(x.Split(':')[0]),
							x => int.Parse(x.Split(':')[1]));
					}
				}
			}
		}
	}
}
