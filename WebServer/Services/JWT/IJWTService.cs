using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace GroupCheck.WebServer.Services
{
	public interface IJWTService
	{
		string SigningAlgorithm { get; }
		SecurityKey EncodingKey { get; }
		SecurityKey DecodingKey { get; }

		string GenerateToken(IEnumerable<Claim> claims);
		TokenValidationParameters GetTokenValidationParameters();
	}
}
