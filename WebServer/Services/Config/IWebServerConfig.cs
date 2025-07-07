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
	public interface IWebServerConfig
	{
		IConnectionStringsConfig ConnectionStrings { get; }
		IJWTConfig JWT { get; }
	}

	public interface IConnectionStringsConfig
	{
		string DB { get; }
		string Logger { get; }
	}

	public interface IJWTConfig
	{
		byte[] Key { get; }
		string AccountIdClaim { get; }
	}
}
