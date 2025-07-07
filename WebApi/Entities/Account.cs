using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.WebApi
{
	public interface IAccountAPI
	{
		RegisterResponse Register(CreateAccountRequest request);
		AccountResponse GetAccount();
		AccountResponse Update(UpdateAccountRequest request);
		void ChangePassword(ChangePasswordRequest request);
		GetTokenResponse GetTokenByEmail(GetTokenByEmailRequest request);
		GetTokenResponse GetTokenByPhone(GetTokenByPhoneRequest request);
		void RequireCodeByPhone(RequireCodeByPhoneRequest request);
	}

	public class CreateAccountRequest
	{
		public string Email { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
	}

	public class RegisterResponse
	{
		public int AccountID { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string Token { get; set; }
		public long Revision { get; set; }
	}

	public class AccountResponse
	{
		public int AccountID { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public long Revision { get; set; }
	}

	public class GetTokenByEmailRequest
	{
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class GetTokenByPhoneRequest
	{
		public string Phone { get; set; }
		public string Code { get; set; }
	}

	public class RequireCodeByPhoneRequest
	{
		public string Phone { get; set; }
	}

	public class GetTokenResponse
	{
		public int AccountID { get; set; }
		public string Token { get; set; }
	}

	public class UpdateAccountRequest
	{
		public string Name { get; set; }
	}

	public class ChangePasswordRequest
	{
		public string Password { get; set; }
	}
}
