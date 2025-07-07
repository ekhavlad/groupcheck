using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace GroupCheck.WebServer.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly IHttpContextAccessor _ctx;
		private readonly IJWTService _jwt;

		public IdentityService(IHttpContextAccessor ctx, IJWTService jwt)
		{
			_ctx = ctx;
			_jwt = jwt;
		}

		public string AccountIdClaim { get { return "accountid"; } }

		public bool IsAuthenticated()
		{
			return _ctx.HttpContext.User?.Identity?.IsAuthenticated == true;
		}

		public int GetCurrentAccountID()
		{
			if (int.TryParse(_ctx.HttpContext.User?.FindFirst(AccountIdClaim)?.Value, out int accountID))
				return accountID;
			else
				throw new System.Security.Authentication.AuthenticationException();
		}
	}
}
