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
//	internal class BigInMemoryDatabase
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

//		private readonly InMemoryStorage storage;
//		public BigInMemoryDatabase(string connectionString)
//		{
//			storage = new InMemoryStorage(new MSSQLStorage(connectionString));
//		}
//		public BigInMemoryDatabase(string host, string user, string pass, string db)
//		{
//			storage = new InMemoryStorage(new MSSQLStorage(host, user, pass, db));
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
//			var sw = new Stopwatch();
//			sw.Start();
//			var usersL = users.ToString().Length;
//			for (int i = 1; i <= users; i++)
//			{
//				var name = "User " + i.ToString().PadLeft(usersL, '0');
//				var email = "user_" + i.ToString().PadLeft(usersL, '0') + "@sample.com";
//				storage.Accounts.Create(new Account()
//				{
//					Email = email,
//					Name = name,
//					Password = new string(' ', 16),
//					Salt = new string(' ', 44),
//					Created = DateTime.UtcNow,
//					Updated = DateTime.UtcNow,
//					Deleted = IsPercent(DELETED_ACCOUNTS)
//				});

//				if (i % STEP_COUNT == 0)
//				{
//					Console.WriteLine("User created: {0} in {1}", i, sw.ElapsedMilliseconds);
//					sw.Restart();
//				}
//			}
//			sw.Stop();
//			Console.WriteLine("Users created. Backing up...");
//			Console.WriteLine("Back up finished");
//			Console.WriteLine("Creating users finished");
//		}
//		private void FillGroups(int users, int groups)
//		{
//			Console.WriteLine("Creating groups...");
//			var sw = new Stopwatch();
//			sw.Start();
//			var groupsL = groups.ToString().Length;
//			for (int i = 1; i <= groups; i++)
//			{
//				var name = "Group " + i.ToString().PadLeft(groupsL, '0');
//				storage.Groups.Create(
//					new Group()
//					{
//						Name = name,
//						Confirmed = IsPercent(CONFIRMED_GROUPS),
//						Created = DateTime.UtcNow,
//						CreatedByID = RandID(users),
//						Updated = DateTime.UtcNow,
//						UpdatedByID = RandID(users),
//						Deleted = IsPercent(DELETED_GROUPS),
//						Members = null
//					});

//				if (i % STEP_COUNT == 0)
//				{
//					Console.WriteLine("Group created: {0} in {1}", i, sw.ElapsedMilliseconds);
//					sw.Restart();
//				}
//			}
//			sw.Stop();
//			Console.WriteLine("Groups created. Backing up...");
//			Console.WriteLine("Back up finished");
//			Console.WriteLine("Creating groups finished");
//		}
//		private void FillMembers(int users, int groups, int members)
//		{
//			Console.WriteLine("Creating members...");
//			var sw = new Stopwatch();
//			sw.Start();
//			var membersL = members.ToString().Length;
//			for (int i = 1; i <= members; i++)
//			{
//				var groupID = RandID(groups);
//				var name = "Member " + i.ToString().PadLeft(membersL, '0');

//				storage.Members.Create(
//					new Member()
//					{
//						GroupID = groupID,
//						Name = name,
//						AccountID = IsPercent(MEMBERS_WITH_ACCOUNT) ? (int?)RandID(users) : null,
//						Confirmed = IsPercent(CONFIRMED_MEMBERS),
//						Created = DateTime.UtcNow,
//						CreatedByID = RandID(users),
//						Updated = DateTime.UtcNow,
//						UpdatedByID = RandID(users),
//						Deleted = IsPercent(DELETED_MEMBERS)
//					});

//				if (i % STEP_COUNT == 0)
//				{
//					Console.WriteLine("Member created: {0} in {1}", i, sw.ElapsedMilliseconds);
//					sw.Restart();
//				}
//			}
//			sw.Stop();
//			Console.WriteLine("Members created. Backing up...");
//			Console.WriteLine("Back up finished");
//			Console.WriteLine("Creating members finished");
//		}
//		private void FillChecks(int users, int groups, int checks)
//		{
//			Console.WriteLine("Creating checks...");
//			Console.WriteLine("    getting members...");
//			var members = storage.GetAllMembers();
//			Console.WriteLine("    got all members");

//			var sw = new Stopwatch();
//			sw.Start();
//			var checksL = checks.ToString().Length;
//			for (int i = 1; i <= checks; i++)
//			{
//				var groupID = RandID(groups);
//				var descr = "Check " + i.ToString().PadLeft(checksL, '0');

//				var creditors = (members.ContainsKey(groupID)) ? new Dictionary<int, int>() { { members[groupID].First(), (int)Hash.GenerateInt(0, 100000) } } : new Dictionary<int, int>();
//				var debitors = (members.ContainsKey(groupID)) ? members[groupID].ToDictionary(m => m, m => (int)Hash.GenerateInt(0, 30000)) : new Dictionary<int, int>();

//				storage.Checks.Create(
//					new Check()
//					{
//						GroupID = groupID,
//						DateAndTime = DateTime.Today,
//						Description = descr,
//						Creditors = creditors,
//						Debitors = debitors,
//						Confirmed = IsPercent(CONFIRMED_CHECKS),
//						Created = DateTime.UtcNow,
//						CreatedByID = RandID(users),
//						Updated = DateTime.UtcNow,
//						UpdatedByID = RandID(users),
//						Deleted = IsPercent(DELETED_CHECKS)
//					});

//				if (i % STEP_COUNT == 0)
//				{
//					Console.WriteLine("Check created: {0} in {1}", i, sw.ElapsedMilliseconds);
//					sw.Restart();
//				}
//			}
//			sw.Stop();
//			Console.WriteLine("Checks created. Backing up...");
//			Console.WriteLine("Back up finished");
//			Console.WriteLine("Creating checks finished");
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
