using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
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
using GroupCheck.WebServer.Services.Auth;
using GroupCheck.WebServer.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GroupCheck.WebServer.Filters;

namespace GroupCheck.WebServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Config = new WebServerConfig(configuration);
		}

		public readonly WebServerConfig Config;

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<IISServerOptions>(options =>
			{
				options.AutomaticAuthentication = false;
			});

			var jwt = new JWTService(Config.JWT.Key);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IRequestNumber>(new RequestNumber(SqlLogger.GetLastRequestNbr(Config.ConnectionStrings.Logger)));
			services.AddSingleton<IServerStorage>(new MSSQLStorage(Config.ConnectionStrings.DB));
			services.AddSingleton<IJWTService>(jwt);
			services.AddSingleton<IIdentityService, IdentityService>();
			services.AddSingleton<ISMSService, SMSService>();
			services.AddSingleton<IAuthByPhone, AuthByPhone>();
			services.AddScoped<ILogger>(provider =>
			{
				return new SqlLogger(Config.ConnectionStrings.Logger, provider.GetService<IRequestNumber>().GetNextRequestID());
			});
			services.AddScoped<IServer>(provider =>
			{
				var identity = provider.GetService<IIdentityService>();
				var server = new Core(provider.GetService<IServerStorage>());
				if (identity.IsAuthenticated())
					server.SetCurrentAccount(identity.GetCurrentAccountID());
				else
					server.WorkAsAnonymous();

				return server;
			});

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options => { options.TokenValidationParameters = jwt.GetTokenValidationParameters(); });

			services.AddMvc(provider =>
			{
				var policy = new AuthorizationPolicyBuilder()
								 .RequireAuthenticatedUser()
								 .Build();
				provider.Filters.Add(new AuthorizeFilter(policy));
				provider.Filters.Add(new Microsoft.AspNetCore.Mvc.TypeFilterAttribute(typeof(RequireInputDataFilter)));
				provider.Filters.Add(new Microsoft.AspNetCore.Mvc.TypeFilterAttribute(typeof(ManagedExceptionFilter)));
				provider.EnableEndpointRouting = true;
			})
				.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
				.AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
		}

		public void Configure(IApplicationBuilder app)
		{
			app.Map("/api", ConfigureAPI);

			app.UseStaticFiles();
			app.UseUnmanagedExceptions();
			app.UseAuthentication();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
			});
		}

		private static void ConfigureAPI(IApplicationBuilder app)
		{
			app.UseUnmanagedExceptions();
			app.UseRequestLogger();
			app.UseAuthentication();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
			});
		}
	}
}
