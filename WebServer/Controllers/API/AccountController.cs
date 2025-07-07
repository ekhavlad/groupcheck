using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GroupCheck.WebApi;
using GroupCheck.Server;
using GroupCheck.WebServer.Services;
using GroupCheck.WebServer.Services.Auth;

namespace GroupCheck.WebServer
{
	public class AccountController : BaseApiController, IAccountAPI
	{
		#region ctor
		protected readonly IIdentityService _idt;
		protected readonly IJWTService _jwt;
		protected readonly IAuthByPhone _codeVerifier;
		public AccountController(
			IServer server,
			[FromServices] IIdentityService idt,
			[FromServices] IJWTService jwt,
			[FromServices] IAuthByPhone code
			) : base(server)
		{
			_idt = idt;
			_jwt = jwt;
			_codeVerifier = code;
		}
		#endregion

		[HttpPost]
		[AllowAnonymous]
		public RegisterResponse Register([FromBody] CreateAccountRequest request)
		{
			var newAccount = new NewAccount()
			{
				Email = request.Email,
				Name = request.Name,
				Password = request.Password
			};
			var account = _server.Accounts.Register(newAccount);
			var token = CreateToken(account);
			var response = new RegisterResponse()
			{
				AccountID = account.ID,
				Email = account.Email,
				Name = account.Name,
				Token = token,
				Revision = account.Revision,
			};
			return response;
		}

		[HttpPost]
		[AllowAnonymous]
		public GetTokenResponse GetTokenByEmail([FromBody] GetTokenByEmailRequest request)
		{
			var account = _server.Accounts.ValidateEmail(request.Email, request.Password);
			var token = CreateToken(account);
			var response = new GetTokenResponse()
			{
				AccountID = account.ID,
				Token = token
			};
			return response;
		}

		[HttpPost]
		[AllowAnonymous]
		public GetTokenResponse GetTokenByPhone([FromBody] GetTokenByPhoneRequest request)
		{
			var phone = request.Phone;
			var code = request.Code;

			if (!_codeVerifier.CodeIsValid(phone, code))
				throw new APIManagedException(ResponseCode.AUTH_PHONE_INVALID_CODE);

			var account = _server.Accounts.FindOrCreate(AuthScheme.Phone, phone);
			var token = CreateToken(account);
			var response = new GetTokenResponse()
			{
				AccountID = account.ID,
				Token = token
			};
			return response;
		}

		[HttpPost]
		[AllowAnonymous]
		public void RequireCodeByPhone([FromBody] RequireCodeByPhoneRequest request)
		{
			var phone = ParsePhone(request.Phone);
			_codeVerifier.RequireCode(phone);
		}

		[HttpGet]
		public AccountResponse GetAccount()
		{
			var account = _server.Accounts.GetCurrent();
			var response = new AccountResponse()
			{
				AccountID = account.ID,
				Email = account.Email,
				Name = account.Name,
				Revision = account.Revision
			};
			return response;
		}

		[HttpPost]
		public AccountResponse Update([FromBody] UpdateAccountRequest request)
		{
			var account = _server.Accounts.GetCurrent();
			account.Name = request.Name;
			var updated = _server.Accounts.Update(new UpdateAccount() { Name = request.Name });
			var response = new AccountResponse()
			{
				AccountID = updated.ID,
				Email = updated.Email,
				Name = updated.Name,
				Revision = updated.Revision
			};
			return response;
		}

		[HttpPost]
		public void ChangePassword([FromBody] ChangePasswordRequest request)
		{
			_server.Accounts.ChangePassword(request.Password);
		}


		private string CreateToken(Account account)
		{
			var claims = new Claim[]
			{
				new Claim(_idt.AccountIdClaim, account.ID.ToString()),
			};
			var token = _jwt.GenerateToken(claims);
			return token;
		}

		private string ParsePhone(string phone)
		{
			if (string.IsNullOrEmpty(phone))
				throw new ValidationException(ValidationCode.AccountInvalidPhone);

			phone = new string(phone.ToCharArray().Where(_ => char.IsDigit(_)).ToArray());

			if (phone.Length != 11)
				throw new ValidationException(ValidationCode.AccountInvalidPhone);

			return phone;
		}
	}
}
