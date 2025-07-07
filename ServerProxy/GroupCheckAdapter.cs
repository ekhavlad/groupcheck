using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using GroupCheck.WebApi;
using GroupCheck.Tools.Extensions.JSONExt;

namespace GroupCheck.ServerProxy
{
	public partial class GroupCheckAdapter : IAccountAPI, IGroupAPI, IMemberAPI, ICheckAPI
	{
		#region constants
		private const string CONTENT_TYPE_JSON = "application/json";
		private const int MAX_REQUESTS_PER_SECOND = 50;
		private const int TIMEOUT_BEFORE_RETRY_REQUEST = 1000;
		private const int TIMEOUT_BEFORE_RETRY_AUTH = 10;
		private const int HTTP_TOO_MANY_REQUESTS = 429;
		private const int GATEWAY_TIMEOUT = 502;
		private const int DEFAULT_BATCH_SIZE = 50;
		#endregion

		private readonly string _host;
		private string _token;
		private int _accountID;
		public int AccountID {  get { return _accountID; } }

		public IAccountAPI Accounts { get { return this; } }
		public IGroupAPI Groups { get { return this; } }
		public IMemberAPI Members { get { return this; } }
		public ICheckAPI Checks { get { return this; } }

		public GroupCheckAdapter(string host)
		{
			_host = host;
		}
		public GroupCheckAdapter(string host, string token)
		{
			_host = host;
			_token = token;
		}

		public void UseToken(string token)
		{
			_token = token;
			var account = Accounts.GetAccount();
			_accountID = account.AccountID;
		}

		public void Login(string email, string password)
		{
			var response = Accounts.GetTokenByEmail(new GetTokenByEmailRequest() { Email = email, Password = password });
			_token = response.Token;
			_accountID = response.AccountID;
		}

		#region REST
		public T Get<T>(string path)
		{
			var response = SafeRequest("GET", path);
			var result = response.ParseJSON<T>();
			return result;
		}
		public T Post<T>(string path, object body)
		{
			var response = SafeRequest("POST", path, body.ToJSON());
			var result = response.ParseJSON<T>();
			return result;
		}
		public void Post(string path, object data)
		{
			var body = data.ToJSON();
			SafeRequest("POST", path, body);
		}
		public void Delete(string path)
		{
			var response = SafeRequest("DELETE", path);
		}

		private string SafeRequest(string method, string path, string body = null)
		{
			WaitSecondlyLimit();

			var url = $"{_host}/api/{path.TrimStart('/')}";

			var headers = new Dictionary<string, string>() { { "Authorization", $"Bearer {_token}" } };

			var response = HTTP.MakeRequest(method, url, CONTENT_TYPE_JSON, headers, body);

			if (response.ResponseCode == HttpStatusCode.OK ||
				response.ResponseCode == HttpStatusCode.NoContent ||
				response.ResponseCode == HttpStatusCode.Accepted)
				return response.Content;

			switch (response.ResponseCode)
			{
				case HttpStatusCode.OK:
					return response.Content;

				case HttpStatusCode.NoContent:
				case HttpStatusCode.Accepted:
					return null;

				case HttpStatusCode.Unauthorized:
					throw new AuthenticationRequiredException();

				case HttpStatusCode.Forbidden:
					throw new AccessDeniedException();

				case HttpStatusCode.NotFound:
					throw new NotFoundException();

				case HttpStatusCode.Gone:
					throw new DeletedException();

				case (HttpStatusCode)422:
					throw new ValidationException(response.Content);

				default:
					throw new Exception(response.Content);
			}
		}
		#endregion

		#region Adapter State
		private readonly List<DateTime> _lastRequests = new List<DateTime>();
		private void WaitSecondlyLimit()
		{
			lock (_lastRequests)
			{
				_lastRequests.RemoveAll(time => time <= DateTime.Now.AddSeconds(-1));
				if (_lastRequests.Count >= MAX_REQUESTS_PER_SECOND)
				{
					var minTime = _lastRequests.Min();
					var waitTime = (DateTime.Now - minTime).TotalMilliseconds;
					System.Threading.Thread.Sleep((int)waitTime);
				}
				_lastRequests.Add(DateTime.Now);
			}
		}
		#endregion
	}
}
