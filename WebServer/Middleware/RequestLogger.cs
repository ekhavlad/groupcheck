using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using GroupCheck.WebServer.Services;

namespace GroupCheck.WebServer
{
	public class RequestLoggerMiddleware
	{
		private readonly RequestDelegate _next;

		public RequestLoggerMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger logger)
		{
			if (context == null) throw new Exception("FATAL!");

			context.Request.EnableBuffering(50 * 1024 * 1040);
			var origResponseBody = context.Response.Body;
			var responseBody = (string)null;
			var sw = new Stopwatch();
			var requestTime = DateTime.Now.ToUniversalTime();
			sw.Start();
			try
			{
				using (var memStream = new MemoryStream())
				{
					context.Response.Body = memStream;

					if (_next != null)
						await _next.Invoke(context);

					memStream.Position = 0;
					using (var reader = new StreamReader(memStream))
					{
						responseBody = reader.ReadToEnd();
						memStream.Position = 0;
						await memStream.CopyToAsync(origResponseBody);
					}
				}
			}
			catch (Exception ex)
			{
				logger.Fatal(ex);
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			finally
			{
				sw.Stop();

				context.Response.Body = origResponseBody;

				var method = context.Request.Method;
				var statusCode = (short)context.Response.StatusCode;
				var requestBody = (string)null;
				var path = context.Request.Path;
				var queryString = context.Request.QueryString;
				var url = path + queryString;

				if (method == "POST" || method == "PUT")
				{
					try
					{
						requestBody = ReadStream(context.Request.Body);
					}
					catch (Exception ex)
					{
						requestBody = ex.Message;
					}
				}

				var requestExecutionTime = (int)sw.ElapsedMilliseconds;

				logger.LogRequest(
					requestTime,
					method,
					statusCode,
					requestExecutionTime,
					url,
					requestBody ?? string.Empty,
					responseBody ?? string.Empty);
			}
		}

		private string ReadStream(Stream stream)
		{
			using (var mem = new MemoryStream())
			using (var reader = new StreamReader(mem))
			{
				var pos = stream.Position;
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(mem);
				stream.Seek(pos, SeekOrigin.Begin);
				mem.Seek(0, SeekOrigin.Begin);
				var data = reader.ReadToEnd();
				return data;
			}
		}
	}

	public static class RequestLoggerExtensions
	{
		public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder app)
		{
			return app.UseMiddleware<RequestLoggerMiddleware>();
		}
	}
}
