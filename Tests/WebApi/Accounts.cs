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
	public class Accounts : _Base
	{
		[Test, Order(0)]
		public void CRUD()
		{
			var api = GetHost();
			var STEP = "";

			var email = $"test.{Guid.NewGuid().ToString().ToLower().Replace('-', '.')}@sample.com";
			var name = "test name";
			var pass = Guid.NewGuid().ToString();
			var pass2 = Guid.NewGuid().ToString();

			//==================================================================
			STEP = "REGISTER Account";
			var account = api.Accounts.Register(
				new CreateAccountRequest()
				{
					Email = email,
					Name = name,
					Password = pass
				});
			Assert.IsNotNull(account, Messages.NOT_CREATED);
			Assert.Greater(account.AccountID, 0, "Created ID == 0");
			Assert.Greater(account.Revision, 0, "Created Revision == 0");
			Assert.AreEqual(email, account.Email, STEP);
			Assert.AreEqual(name, account.Name, STEP);
			Assert.IsNotNull(name, account.Token);

			api.Login(email, pass);

			//==================================================================
			STEP = "GET Current Account";
			var current = api.Accounts.GetAccount();
			Assert.AreEqual(account.AccountID, current.AccountID, STEP);
			Assert.AreEqual(account.Email, current.Email, STEP);
			Assert.AreEqual(account.Name, current.Name, STEP);
			Assert.AreEqual(account.Revision, current.Revision, STEP);

			//==================================================================
			STEP = "UPDATE Account";
			api.Accounts.ChangePassword(new ChangePasswordRequest() { Password = pass2 });

			api.Login(email, pass2);

			api.Accounts.GetTokenByEmail(new GetTokenByEmailRequest() { Email = email, Password = pass2 });
		}
	}
}
