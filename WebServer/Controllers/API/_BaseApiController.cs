using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GroupCheck.WebApi;
using GroupCheck.Server;

namespace GroupCheck.WebServer
{
	public class BaseApiController : ControllerBase
	{
		protected IServer _server;
		public BaseApiController(IServer server)
		{
			_server = server;
		}
	}
}
