using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupCheck.WebApi;
using GroupCheck.ServerStorage;
using GroupCheck.Server;
using GroupCheck.Tools;
using GroupCheck.Tools.Extensions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GroupCheck.WebServer
{
	public class BaseController : Controller
	{
		protected IServer _server;
		public BaseController(IServer server)
		{
			_server = server;
		}
	}
}
