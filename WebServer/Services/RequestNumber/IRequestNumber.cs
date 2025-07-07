using System;
using Microsoft.IdentityModel.Tokens;

namespace GroupCheck.WebServer.Services
{
	public interface IRequestNumber
	{
		long GetNextRequestID();
	}
}
