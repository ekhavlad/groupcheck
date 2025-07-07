//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Data;
//using System.Data.SqlClient;
//using GroupCheck.Tools;
//using System.Diagnostics;
//using GroupCheck.ServerStorage;
//using GroupCheck.Server;

//namespace ConsoleTest
//{
//	internal class BigDatabase
//	{
//		const uint DELETED_ACCOUNTS = 15;
//		const uint DELETED_GROUPS = 15;
//		const uint DELETED_MEMBERS = 40;
//		const uint DELETED_CHECKS = 50;

//		const uint CONFIRMED_GROUPS = 85;
//		const uint CONFIRMED_MEMBERS = 85;
//		const uint CONFIRMED_CHECKS = 85;

//		const uint MEMBERS_WITH_ACCOUNT = 30;

//		const int STEP_COUNT = 50000;

//		private readonly MSSQLStorage storage;
//		public BigDatabase(string connectionString)
//		{
//			storage = new MSSQLStorage(connectionString);
//		}
//		public BigDatabase(string host, string user, string pass, string db)
//		{
//			storage = new MSSQLStorage(host, user, pass, db);
//		}

//		public void Fill(int users, int groups, int members, int checks)
//		{
//			FillUsers(users);
//			FillGroups(users, groups);
//			FillMembers(users, groups, members);
//			FillChecks(users, groups, checks);
//		}

//		private void FillUsers(int users)
//		{
//			Console.WriteLine("Creating users...");
//			var userExisted = (int)storage.ExecuteScalar(System.Data.CommandType.Text, "SELECT COUNT(*) FROM Accounts", null);
//			if (userExisted < users)
//			{
//				var sw = new Stopwatch();
//				sw.Start();
//				var usersL = users.ToString().Length;
//				var batch = new List<Account>();
//				for (int i = userExisted + 1; i <= users; i++)
//				{
//					var name = "User " + i.ToString().PadLeft(usersL, '0');
//					var email = "user_" + i.ToString().PadLeft(usersL, '0') + "@sample.com";
//					batch.Add(
//						new Account()
//						{
//							Email = email,
//							Name = name,
//							Password = new string(' ', 16),
//							Salt = new string(' ', 44),
//							Created = DateTime.UtcNow,
//							Updated = DateTime.UtcNow,
//							Deleted = IsPercent(DELETED_ACCOUNTS)
//						});

//					if (i % STEP_COUNT == 0)
//					{
//						PushBatch(batch);
//						batch = new List<Account>();
//						Console.WriteLine("User created: {0} in {1}", i, sw.ElapsedMilliseconds);
//						sw.Restart();
//					}
//				}

//				if (batch.Any()) PushBatch(batch);

//				sw.Stop();
//				Console.WriteLine("Users created. Backing up...");
//				storage.SysCreateBackup("d:\\_Backups\\" + storage.DB + "_users.bak");
//				Console.WriteLine("Back up finished");
//			}
//			Console.WriteLine("Creating users finished");
//		}
//		private void FillGroups(int users, int groups)
//		{
//			Console.WriteLine("Creating groups...");
//			var groupsExisted = (int)storage.ExecuteScalar(System.Data.CommandType.Text, "SELECT COUNT(*) FROM Groups", null);
//			if (groupsExisted < groups)
//			{
//				var sw = new Stopwatch();
//				sw.Start();
//				var groupsL = groups.ToString().Length;
//				var batch = new List<Group>();
//				for (int i = groupsExisted + 1; i <= groups; i++)
//				{
//					var name = "Group " + i.ToString().PadLeft(groupsL, '0');
//					batch.Add(
//						new Group()
//						{
//							Name = name,
//							Confirmed = IsPercent(CONFIRMED_GROUPS),
//							Created = DateTime.UtcNow,
//							CreatedByID = RandID(users),
//							Updated = DateTime.UtcNow,
//							UpdatedByID = RandID(users),
//							Deleted = IsPercent(DELETED_GROUPS),
//							Members = null
//						});

//					if (i % STEP_COUNT == 0)
//					{
//						PushBatch(batch);
//						batch = new List<Group>();
//						Console.WriteLine("Group created: {0} in {1}", i, sw.ElapsedMilliseconds);
//						sw.Restart();
//					}
//				}

//				if (batch.Any()) PushBatch(batch);

//				sw.Stop();
//				Console.WriteLine("Groups created. Backing up...");
//				storage.SysCreateBackup("d:\\_Backups\\" + storage.DB + "_groups.bak");
//				Console.WriteLine("Back up finished");
//			}
//			Console.WriteLine("Creating groups finished");
//		}
//		private void FillMembers(int users, int groups, int members)
//		{
//			Console.WriteLine("Creating members...");
//			var membersExisted = (int)storage.ExecuteScalar(System.Data.CommandType.Text, "SELECT COUNT(*) FROM Members", null);
//			if (membersExisted < members)
//			{
//				var sw = new Stopwatch();
//				sw.Start();
//				var membersL = members.ToString().Length;
//				var batch = new List<Member>();
//				for (int i = membersExisted + 1; i <= members; i++)
//				{
//					var groupID = RandID(groups);
//					var name = "Member " + i.ToString().PadLeft(membersL, '0');

//					batch.Add(
//						new Member()
//						{
//							GroupID = groupID,
//							Name = name,
//							AccountID = IsPercent(MEMBERS_WITH_ACCOUNT) ? (int?)RandID(users) : null,
//							Confirmed = IsPercent(CONFIRMED_MEMBERS),
//							Created = DateTime.UtcNow,
//							CreatedByID = RandID(users),
//							Updated = DateTime.UtcNow,
//							UpdatedByID = RandID(users),
//							Deleted = IsPercent(DELETED_MEMBERS)
//						});

//					if (i % STEP_COUNT == 0)
//					{
//						PushBatch(batch);
//						batch = new List<Member>();
//						Console.WriteLine("Member created: {0} in {1}", i, sw.ElapsedMilliseconds);
//						sw.Restart();
//					}
//				}

//				if (batch.Any()) PushBatch(batch);

//				sw.Stop();
//				Console.WriteLine("Members created. Backing up...");
//				storage.SysCreateBackup("d:\\_Backups\\" + storage.DB + "_members.bak");
//				Console.WriteLine("Back up finished");
//			}
//			Console.WriteLine("Creating members finished");
//		}
//		private void FillChecks(int users, int groups, int checks)
//		{
//			Console.WriteLine("Creating checks...");
//			var countCmd = new SqlCommand("SELECT COUNT(*) FROM Checks") { CommandTimeout = 60 };
//			var checksExisted = (int)storage.ExecuteScalar(countCmd);
//			if (checksExisted < checks)
//			{
//				Console.WriteLine("    getting members...");
//				var members = GetAllMembers();
//				Console.WriteLine("    got all members");

//				var sw = new Stopwatch();
//				sw.Start();
//				var checksL = checks.ToString().Length;
//				var batch = new List<Check>();
//				for (int i = checksExisted + 1; i <= checks; i++)
//				{
//					var groupID = RandID(groups);
//					var descr = "Check " + i.ToString().PadLeft(checksL, '0');

//					var creditors = (members.ContainsKey(groupID)) ? new Dictionary<int, int>() { { members[groupID].First(), (int)Hash.GenerateInt(0, 100000) } } : new Dictionary<int, int>();
//					var debitors = (members.ContainsKey(groupID)) ? members[groupID].ToDictionary(m => m, m => (int)Hash.GenerateInt(0, 30000)) : new Dictionary<int, int>();

//					batch.Add(
//						new Check()
//						{
//							GroupID = groupID,
//							DateAndTime = DateTime.Today,
//							Description = descr,
//							Creditors = creditors,
//							Debitors = debitors,
//							Confirmed = IsPercent(CONFIRMED_CHECKS),
//							Created = DateTime.UtcNow,
//							CreatedByID = RandID(users),
//							Updated = DateTime.UtcNow,
//							UpdatedByID = RandID(users),
//							Deleted = IsPercent(DELETED_CHECKS)
//						});

//					if (i % STEP_COUNT == 0)
//					{
//						PushBatch(batch);
//						batch = new List<Check>();
//						Console.WriteLine("Check created: {0} in {1}", i, sw.ElapsedMilliseconds);
//						sw.Restart();
//					}
//				}
//				if (batch.Any())
//				{
//					PushBatch(batch);
//				}

//				sw.Stop();
//				Console.WriteLine("Checks created. Backing up...");
//				storage.SysCreateBackup("d:\\_Backups\\" + storage.DB + "_checks.bak");
//				Console.WriteLine("Back up finished");
//			}
//			Console.WriteLine("Creating checks finished");
//		}

//		private void PushBatch(IEnumerable<Account> accounts)
//		{
//			var table = new DataTable();
//			table.Columns.Add("Email", typeof(string));
//			table.Columns.Add("Name", typeof(string));
//			table.Columns.Add("Salt", typeof(string));
//			table.Columns.Add("Password", typeof(string));
//			table.Columns.Add("Created", typeof(DateTime));
//			table.Columns.Add("Updated", typeof(DateTime));
//			table.Columns.Add("Deleted", typeof(bool));

//			foreach (var account in accounts)
//			{
//				var row = table.NewRow();
//				row["Email"] = account.Email;
//				row["Name"] = account.Name;
//				row["Salt"] = account.Salt;
//				row["Password"] = account.Password;
//				row["Created"] = account.Created;
//				row["Updated"] = account.Updated;
//				row["Deleted"] = account.Deleted;

//				table.Rows.Add(row);
//			}

//			var sql = @"INSERT INTO Accounts (Email, Name, Salt, Password, Created, Updated, Deleted)
//						SELECT Email, Name, Salt, Password, Created, Updated, Deleted FROM @NewEntities;";

//			var cmd = new SqlCommand(sql);
//			cmd.Parameters.Add(new SqlParameter("@NewEntities", SqlDbType.Structured) { Value = table, TypeName = "CreateAccountTableType" });

//			var rowNbr = storage.Execute(cmd);
//		}
//		private void PushBatch(IEnumerable<Group> groups)
//		{
//			var table = new DataTable();
//			table.Columns.Add("Name", typeof(string));
//			table.Columns.Add("Confirmed", typeof(bool));
//			table.Columns.Add("Created", typeof(DateTime));
//			table.Columns.Add("CreatedByID", typeof(int));
//			table.Columns.Add("Updated", typeof(DateTime));
//			table.Columns.Add("UpdatedByID", typeof(int));
//			table.Columns.Add("Deleted", typeof(bool));

//			foreach (var group in groups)
//			{
//				var row = table.NewRow();
//				row["Name"] = group.Name;
//				row["Confirmed"] = group.Confirmed;
//				row["Created"] = group.Created;
//				row["CreatedByID"] = group.CreatedByID;
//				row["Updated"] = group.Updated;
//				row["UpdatedByID"] = group.UpdatedByID;
//				row["Deleted"] = group.Deleted;

//				table.Rows.Add(row);
//			}

//			var sql = @"INSERT INTO Groups (Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
//						SELECT Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted FROM @NewEntities;";

//			var cmd = new SqlCommand(sql);
//			cmd.Parameters.Add(new SqlParameter("@NewEntities", SqlDbType.Structured) { Value = table, TypeName = "CreateGroupTableType" });

//			var rowNbr = storage.Execute(cmd);
//		}
//		private void PushBatch(IEnumerable<Member> members)
//		{
//			var table = new DataTable();
//			table.Columns.Add("GroupID", typeof(int));
//			table.Columns.Add("MemberID", typeof(int));
//			table.Columns.Add("AccountID", typeof(int));
//			table.Columns.Add("Name", typeof(string));
//			table.Columns.Add("Confirmed", typeof(bool));
//			table.Columns.Add("Created", typeof(DateTime));
//			table.Columns.Add("CreatedByID", typeof(int));
//			table.Columns.Add("Updated", typeof(DateTime));
//			table.Columns.Add("UpdatedByID", typeof(int));
//			table.Columns.Add("Deleted", typeof(bool));

//			foreach (var member in members)
//			{
//				var row = table.NewRow();
//				row["GroupID"] = member.GroupID;
//				row["MemberID"] = member.MemberID;
//				row["AccountID"] = (member.AccountID == null) ? DBNull.Value : (object)member.AccountID;
//				row["Name"] = member.Name;
//				row["Confirmed"] = member.Confirmed;
//				row["Created"] = member.Created;
//				row["CreatedByID"] = member.CreatedByID;
//				row["Updated"] = member.Updated;
//				row["UpdatedByID"] = member.UpdatedByID;
//				row["Deleted"] = member.Deleted;

//				table.Rows.Add(row);
//			}

//			var sql = @"INSERT INTO Members (GroupID, AccountID, Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
//						SELECT GroupID, AccountID, Name, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted FROM @NewEntities;";

//			var cmd = new SqlCommand(sql);
//			cmd.Parameters.Add(new SqlParameter("@NewEntities", SqlDbType.Structured) { Value = table, TypeName = "CreateMemberTableType" });

//			var rowNbr = storage.Execute(cmd);
//		}
//		private void PushBatch(IEnumerable<Check> checks)
//		{
//			var table = new DataTable();
//			table.Columns.Add("GroupID", typeof(int));
//			table.Columns.Add("CheckID", typeof(int));
//			table.Columns.Add("DateAndTime", typeof(DateTime));
//			table.Columns.Add("Description", typeof(string));
//			table.Columns.Add("Creditors", typeof(string));
//			table.Columns.Add("Debitors", typeof(string));
//			table.Columns.Add("Confirmed", typeof(bool));
//			table.Columns.Add("Created", typeof(DateTime));
//			table.Columns.Add("CreatedByID", typeof(int));
//			table.Columns.Add("Updated", typeof(DateTime));
//			table.Columns.Add("UpdatedByID", typeof(int));
//			table.Columns.Add("Deleted", typeof(bool));

//			foreach (var check in checks)
//			{
//				var row = table.NewRow();
//				row["GroupID"] = check.GroupID;
//				row["CheckID"] = check.CheckID;
//				row["DateAndTime"] = check.DateAndTime;
//				row["Description"] = check.Description;
//				row["Creditors"] = string.Join(",", check.Creditors?.Select(m => $"{m.Key}:{m.Value}"));
//				row["Debitors"] = string.Join(",", check.Debitors?.Select(m => $"{m.Key}:{m.Value}"));
//				row["Confirmed"] = check.Confirmed;
//				row["Created"] = check.Created;
//				row["CreatedByID"] = check.CreatedByID;
//				row["Updated"] = check.Updated;
//				row["UpdatedByID"] = check.UpdatedByID;
//				row["Deleted"] = check.Deleted;

//				table.Rows.Add(row);
//			}


//			var sql = @"INSERT INTO Checks (GroupID, DateAndTime, Description, Creditors, Debitors, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted)
//						SELECT GroupID, DateAndTime, Description, Creditors, Debitors, Confirmed, Created, CreatedByID, Updated, UpdatedByID, Deleted FROM @NewEntities;";

//			var cmd = new SqlCommand(sql);
//			cmd.Parameters.Add(new SqlParameter("@NewEntities", SqlDbType.Structured) { Value = table, TypeName = "CreateCheckTableType" });

//			var rowNbr = storage.Execute(cmd);
//		}
//		private Dictionary<int, List<int>> GetMembers(IEnumerable<int> groups)
//		{
//			var table = storage.GetTable("SELECT ID, GroupID FROM Members WHERE GroupID IN (" + string.Join(",", groups.Select(g => g.ToString())) + ")", null);
//			var result = new Dictionary<int, List<int>>();
//			foreach (DataRow row in table.Rows)
//			{
//				var groupID = (int)row["GroupID"];
//				var memberID = (int)row["GroupID"];

//				if (!result.ContainsKey(groupID))
//					result[groupID] = new List<int>();
//				result[groupID].Add(memberID);
//			}
//			return null;
//		}
//		private Dictionary<int, List<int>> GetAllMembers()
//		{
//			var table = storage.GetTable("SELECT ID, GroupID FROM Members", null);
//			var result = new Dictionary<int, List<int>>();
//			foreach (DataRow row in table.Rows)
//			{
//				var groupID = (int)row["GroupID"];
//				var memberID = (int)row["ID"];

//				if (!result.ContainsKey(groupID))
//					result[groupID] = new List<int>();
//				result[groupID].Add(memberID);
//			}
//			return result;
//		}

//		private static bool IsPercent(uint x)
//		{
//			return Hash.GenerateInt(1, 100) <= x;
//		}
//		private static int RandID(int maxID)
//		{
//			return (int)Hash.GenerateInt(1, (uint)maxID);
//		}
//	}
//}
