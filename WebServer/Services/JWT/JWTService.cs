using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace GroupCheck.WebServer.Services
{
	public class JWTService : IJWTService
	{
		private readonly SymmetricSecurityKey _secretKey;

		public JWTService(byte[] secretKey)
		{
			_secretKey = new SymmetricSecurityKey(secretKey);
		}

		public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

		public SecurityKey EncodingKey
		{
			get
			{
				return _secretKey;
			}
		}

		public SecurityKey DecodingKey
		{
			get
			{
				return _secretKey;
			}
		}

		public TokenValidationParameters GetTokenValidationParameters()
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,

				IssuerSigningKey = DecodingKey
			};
		}

		public string GenerateToken(IEnumerable<Claim> claims)
		{
			var signCredentials = new SigningCredentials(EncodingKey, SigningAlgorithm);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddHours(5),
				signingCredentials: signCredentials
			);

			var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
			return jwtToken;
		}
	}
}
