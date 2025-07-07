using System;
using Microsoft.IdentityModel.Tokens;

namespace GroupCheck.WebServer.Services
{
	public interface IIdentityService
	{
		string AccountIdClaim { get; }
		bool IsAuthenticated();
		int GetCurrentAccountID();
	}
}
