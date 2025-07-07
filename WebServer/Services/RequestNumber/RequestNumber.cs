using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace GroupCheck.WebServer.Services
{
	public class RequestNumber : IRequestNumber
	{
		private static long _currentRequestID;

		public RequestNumber(long lastRequestID)
		{
			_currentRequestID = lastRequestID;
		}

		public long GetNextRequestID()
		{
			return System.Threading.Interlocked.Increment(ref _currentRequestID);
		}
	}
}
