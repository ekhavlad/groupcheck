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
using System.Linq;
using GroupCheck.WebServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;

namespace GroupCheck.WebServer.Filters
{
	public class RequireInputDataFilter : Attribute, IActionFilter
	{
		private readonly IWebHostEnvironment _env;
		public RequireInputDataFilter(IWebHostEnvironment env)
		{
			_env = env;
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ModelState.IsValid)
				return;

			var errors = new Dictionary<string, string>();
			foreach (var par in context.ActionDescriptor.Parameters)
			{
				var name = par.Name;
				var state = context.ModelState[name] ?? context.ModelState[string.Empty];
				if (state.Errors.Any())
				{
					errors.Add(name, state.Errors.First().ErrorMessage);

				}
			}
			if (errors.Any())
			{
				context.Result = new ContentResult()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ContentType = System.Net.Mime.MediaTypeNames.Text.Plain,
					Content = ResponseCode.INVALID_REQUEST_DATA + "\r\n" + string.Join("\r\n", errors.Select(_ => $"{_.Key}: {_.Value}"))
				};
			}
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}
	}
}
