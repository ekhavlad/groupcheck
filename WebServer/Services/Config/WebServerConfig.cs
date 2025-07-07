using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using GroupCheck.ServerStorage;
using GroupCheck.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GroupCheck.WebServer.Services;

namespace GroupCheck.WebServer.Configuration
{
	public class WebServerConfig : IWebServerConfig, IConnectionStringsConfig, IJWTConfig
	{
		protected readonly IConfiguration _configuration;
		public WebServerConfig(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IConnectionStringsConfig ConnectionStrings { get { return this; } }
		public IJWTConfig JWT { get { return this; } }

		string IConnectionStringsConfig.DB { get { return _configuration.GetConnectionString("db"); } }
		string IConnectionStringsConfig.Logger { get { return _configuration.GetConnectionString("logger"); } }

		byte[] IJWTConfig.Key { get { return Tools.UTF8.GetBytes(_configuration["AppSettings:oauthTokenKey"]); } }
		string IJWTConfig.AccountIdClaim { get { return _configuration["AccountIdClaim"]; } }
	}
}
