using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using GroupCheck.Server;
using GroupCheck.WebApi;
using System.Net;
using GroupCheck.WebServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GroupCheck.WebServer
{
	public class UnmanagedExceptionsMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IWebHostEnvironment _env;

		public UnmanagedExceptionsMiddleware(RequestDelegate next, IWebHostEnvironment env)
		{
			_next = next;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context, ILogger logger)
		{
			try
			{
				if (_next != null)
					await _next.Invoke(context);
			}
			catch(AccountBlockedException)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync(ResponseCode.ACCOUNT_BANNED);
			}
			catch (Exception exception)
			{
				logger.Error(exception);
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				if (_env.IsDevelopment())
					await context.Response.WriteAsync(exception.ToString());
			}
		}
	}

	public static class ManagedExceptionsExtensions
	{
		public static IApplicationBuilder UseUnmanagedExceptions(this IApplicationBuilder app)
		{
			return app.UseMiddleware<UnmanagedExceptionsMiddleware>();
		}
	}
}
