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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GroupCheck.WebServer
{
	[Serializable]
	public class APIManagedException : Exception
	{
		public APIManagedException() : base() { }
		public APIManagedException(string message) : base(message) { }
		public APIManagedException(string message, Exception inner) : base(message, inner) { }
		protected APIManagedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
