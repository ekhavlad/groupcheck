using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
//using GroupCheck.Server;
using GroupCheck.ServerProxy;
using GroupCheck.ServerStorage;
using System.Threading;
using GroupCheck.WebApi;
using GroupCheck.Tools;
using GroupCheck.Tools.Extensions;
using System.Collections.Concurrent;
using GroupCheck.Server;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleTest
{
	class Program
	{
		protected const string HOST = "https://gc.com";
		protected const string EMAIL = "test.account.1@sample.com";
		protected const string NAME = "Test User 1";
		protected const string PASSWORD = "Aw34eszxc!qazwsxedc";

		static void Main(string[] args)
		{
			var api = GetHost();

			var email = $"test.{Guid.NewGuid().ToString().ToLower().Replace('-', '.')}@sample.com";
			var name = "test name";
			var pass = Guid.NewGuid().ToString();
			var pass2 = Guid.NewGuid().ToString();

			api.Accounts.ChangePassword(new ChangePasswordRequest());

			var account = api.Accounts.Register(
				new CreateAccountRequest()
				{
					Email = email,
					Name = name,
					Password = pass
				});
		}

		static int calc(int i)
		{
			return DateTime.Now.Millisecond;
		}


		static void testAPI()
		{
			var api = GetHost();
			var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50aWQiOiIxIiwiZXhwIjoxNTczNDgyNjY0fQ.jUS2IWmE5VgPyppYBrGmjIv1kWWd7u54OgCiVYa6Rqk";
			api.UseToken(token);
		}

		static void testServer()
		{
			var storage = new MSSQLStorage(".", "sa", "Aw34eszxc", "groupcheck");
			var server = new GroupCheck.Server.Core(storage);
			var account = server.Accounts.FindOrCreate(GroupCheck.Server.AuthScheme.Phone, "79166263206");
			server.SetCurrentAccount(account.ID);
		}

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

		static void BigInMemoryDb()
		{
			//var sw = new Stopwatch();
			//sw.Start();
			//var db = new BigInMemoryDatabase(".", "sa", "Aw34eszxc", "groupcheck");
			//db.Fill(100000, 100000, 500000, 1000000);
			//sw.Stop();
			//Console.WriteLine(sw.Elapsed);
			//Console.ReadLine();
		}

		static void BigDb()
		{
			//var sw = new Stopwatch();
			//sw.Start();
			//var db = new BigDatabase(".", "sa", "Aw34eszxc", "groupcheck");
			//db.Fill(10, 100, 10 * 100, 100 * 100);
			//sw.Stop();
			//Console.WriteLine(sw.Elapsed);
		}

		static void ApiAccount()
		{
			var api = new GroupCheck.ServerProxy.GroupCheckAdapter("http://localhost/gc");
			api.Accounts.Register(new GroupCheck.WebApi.CreateAccountRequest() { Email = "admin@sample.com", Name = "name", Password = "123" });
			api.Accounts.GetTokenByEmail(new GetTokenByEmailRequest() { Email = "admin@sample.com", Password = "123" });
			api.Accounts.ChangePassword(new ChangePasswordRequest() { Password = "321" });
		}

		static void ApiChecks()
		{
			var api = new GroupCheck.ServerProxy.GroupCheckAdapter("http://localhost/gc");
			var acc = api.Accounts.Register(new GroupCheck.WebApi.CreateAccountRequest() { Email = "admin@mail.ru", Password = "123" });
			api.Checks.Confirm(15, 25);
		}
	}

	class A
	{
		public int ID { get; set; }
	}
}

