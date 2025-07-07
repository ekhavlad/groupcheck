using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.WebApi;
using GroupCheck.UnitTests;
using GroupCheck.ServerProxy;

namespace GroupCheck.UnitTests.WebApi
{
	[TestFixture]
	public class Unauthorized : _Base
	{
		[Test, Order(0)]
		public void Unauthorised()
		{
			var api = GetHost();

			// Accounts
			Assert.Throws<AuthenticationRequiredException>(() => { api.Accounts.ChangePassword(new ChangePasswordRequest()); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Accounts.GetAccount(); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Accounts.Update(new UpdateAccountRequest()); });

			// Groups
			Assert.Throws<AuthenticationRequiredException>(() => { api.Groups.Confirm(0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Groups.Create(new CreateGroupRequest()); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Groups.Exit(0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Groups.Update(new UpdateGroupRequest()); });

			// Members
			Assert.Throws<AuthenticationRequiredException>(() => { api.Members.Confirm(0, 0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Members.Create(new CreateMemberRequest()); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Members.Delete(0, 0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Members.Update(new UpdateMemberRequest()); });

			// Checks
			Assert.Throws<AuthenticationRequiredException>(() => { api.Checks.Confirm(0, 0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Checks.Create(new CreateCheckRequest()); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Checks.Delete(0, 0); });
			Assert.Throws<AuthenticationRequiredException>(() => { api.Checks.Update(new UpdateCheckRequest()); });
		}
	}
}
