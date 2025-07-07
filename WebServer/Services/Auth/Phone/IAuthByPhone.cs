using System;

namespace GroupCheck.WebServer.Services.Auth
{
	public interface IAuthByPhone
	{
		void RequireCode(string phone);
		bool CodeIsValid(string phone, string code);
	}
}
