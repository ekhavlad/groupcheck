using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.WebApi;
using GroupCheck.ServerProxy;
using GroupCheck.UnitTests;

namespace GroupCheck.UnitTests.WebApi
{
	public class _Base
	{
		protected const string HOST = "https://gc.com";
		protected const string EMAIL = "test.account.1@sample.com";
		protected const string NAME = "Test User 1";
		protected const string PASSWORD = "Aw34eszxc!qazwsxedc";

		protected readonly GroupCheckAdapter api = GetHost();

		protected static GroupCheckAdapter GetHost()
		{
			System.Net.ServicePointManager.ServerCertificateValidationCallback =
			delegate (
				object s,
				System.Security.Cryptography.X509Certificates.X509Certificate certificate,
				System.Security.Cryptography.X509Certificates.X509Chain chain,
				System.Net.Security.SslPolicyErrors sslPolicyErrors
			)
			{
				return true;
			};

			return new GroupCheckAdapter(HOST);
		}

		[SetUp]
		public void DerivedSetUp()
		{
			try
			{
				api.Login(EMAIL, PASSWORD);
			}
			catch (NotFoundException)
			{
				api.Accounts.Register(new CreateAccountRequest() { Email = EMAIL, Name = NAME, Password = PASSWORD });
			}
		}

		[TearDown]
		public void DerivedTearDown()
		{
		}
	}
}
