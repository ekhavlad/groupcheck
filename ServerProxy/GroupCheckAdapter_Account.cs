using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using GroupCheck.WebApi;
using GroupCheck.Tools.Extensions;

namespace GroupCheck.ServerProxy
{
	public partial class GroupCheckAdapter : IAccountAPI
	{
		RegisterResponse IAccountAPI.Register(CreateAccountRequest request)
		{
			var response = Post<RegisterResponse>("Account/Register", request);
			_token = response.Token;
			return response;
		}
		AccountResponse IAccountAPI.GetAccount()
		{
			var response = Get<AccountResponse>("Account/GetAccount");
			return response;
		}
		GetTokenResponse IAccountAPI.GetTokenByEmail(GetTokenByEmailRequest request)
		{
			var response = Post<GetTokenResponse>("Account/GetTokenByEmail", request);
			return response;
		}
		GetTokenResponse IAccountAPI.GetTokenByPhone(GetTokenByPhoneRequest request)
		{
			var response = Post<GetTokenResponse>("Account/GetTokenByPhone", request);
			return response;
		}
		void IAccountAPI.RequireCodeByPhone(RequireCodeByPhoneRequest request)
		{
			Post("Account/RequireCodeByPhone", request);
		}
		AccountResponse IAccountAPI.Update(UpdateAccountRequest request)
		{
			var response = Post<AccountResponse>("Account/Update", request);
			return response;
		}
		void IAccountAPI.ChangePassword(ChangePasswordRequest request)
		{
			Post("Account/ChangePassword", request);
		}
	}
}
