using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GroupCheck.Tools;
using System.Diagnostics;
using GroupCheck.Server;
using GroupCheck.WebApi;
using GroupCheck.ServerStorage;
using GroupCheck.ServerProxy;

namespace ConsoleTest
{
	class AccountImitator
	{
		private readonly string _name;
		private readonly string _email;
		private readonly string _password;
		private readonly GroupCheckAdapter server;

		private const int GROUPS_AVERAGE = 20;
		private const int MEMBERS_AVERAGE = 6;
		private const int CHECKS_AVERAGE = 1000;


		private int _checkCounter = 0;
		private int _groupCounter = 0;
		public AccountImitator(string name, string email, string password)
		{
			_name = name;
			_email = email;
			_password = password;
			server = new GroupCheckAdapter("");
		}

		public void Imitate()
		{


			try
			{
				LoginOrRegister();
			}
			catch
			{
				Console.WriteLine(_name);
				throw;
			}

			var groupsCount = RandomQuantity(GROUPS_AVERAGE, 50);
			for (var g = 1; g <= groupsCount; g++)
			{
				var membersCount = RandomQuantity(MEMBERS_AVERAGE, 50);
				var newMembers = new List<CreateGroupMemberRequest>();
				for (var i = 0; i < membersCount; i++)
					newMembers.Add(new CreateGroupMemberRequest() { Name = "Member " + i.ToString() });
				var newGroup = new CreateGroupRequest()
				{
					Name = "Group " + (++_groupCounter).ToString().PadLeft(2, '0'),
					Members = newMembers
				};
				var group = server.Groups.Create(newGroup);
				group = server.Groups.Confirm(group.ID);

				var checksCount = RandomQuantity(CHECKS_AVERAGE, 50);
				for (var c = 0; c < checksCount; c++)
				{
					var newCheck = GenerateCheck(group);
					var check = server.Checks.Create(newCheck);
					check = server.Checks.Confirm(check.GroupID, check.CheckID);
				}
			}
		}

		public void LoginOrRegister()
		{
			try
			{
				server.Login(_email, _password);
			}
			catch (Exception ex)
			{
				if (ex.Message != ResponseCode.INVALID_EMAIL)
					throw new Exception(ex.Message);

				var newAccount = new CreateAccountRequest()
				{
					Name = _name,
					Email = _email,
					Password = _password
				};
				var account = server.Accounts.Register(newAccount);
				server.Login(_email, _password);
			}
		}

		private CreateCheckRequest GenerateCheck(GroupResponse group)
		{
			var check = new CreateCheckRequest()
			{
				GroupID = group.ID,
				DateAndTime = DateTime.Now,
				Description = "Check " + (++_checkCounter).ToString().PadLeft(5, '0'),
				Creditors = $"{group.Members.First().MemberID}:{GroupCheck.Tools.Random.GenerateInt32(1000)}",
				Debitors = string.Join(",", group.Members.Select(m => $"{m.MemberID}:{GroupCheck.Tools.Random.GenerateInt32(1000)}"))
			};

			return check;
		}

		private static int RandomQuantity(int mid, int dispersion)
		{
			var min = (int)((1.0m - 0.01m * dispersion) * mid);
			var max = (int)((1.0m + 0.01m * dispersion) * mid + 1);
			var result = GroupCheck.Tools.Random.GenerateInt32(min, max);
			return result;
		}
	}
}
