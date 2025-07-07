using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace GroupCheck.WebServer.Services.Auth
{
	public class AuthByPhone : IAuthByPhone
	{
		private const int CODE_VALID_MINUTES = 2;
		private const int MAX_VALIDATE_COUNTER = 5;

		private readonly ISMSService _sms;
		private readonly ConcurrentDictionary<string, CodeInfo> _codes = new ConcurrentDictionary<string, CodeInfo>();

		public AuthByPhone(ISMSService sms)
		{
			_sms = sms;
		}

		public void RequireCode(string phone)
		{
			if (_codes.TryGetValue(phone, out CodeInfo info))
			{
				if (IsPeriodValid(info.Created))
					throw new APIManagedException(WebApi.ResponseCode.AUTH_PHONE_WAIT_FOR_RECREATE);
			}

			var code = Tools.Random.GenerateUInt32(10000);
			info = new CodeInfo() { Code = code, TryCounter = 0, Created = DateTime.Now };
			_codes[phone] = info;

			_sms.SendSMS(phone, code.ToString());
		}

		public bool CodeIsValid(string phone, string code)
		{
			if (!_codes.TryGetValue(phone, out CodeInfo info))
				return false;

			info.TryCounter++;
			_codes[phone] = info;

			if (info.Code.ToString() != code)
				return false;

			if (!IsPeriodValid(info.Created) || info.TryCounter > MAX_VALIDATE_COUNTER)
			{
				_codes.TryRemove(phone, out CodeInfo removed);
				return false;
			}

			return true;
		}

		private static bool IsPeriodValid(DateTime created)
		{
			return created > DateTime.Now.AddMinutes(-CODE_VALID_MINUTES);
		}

		private struct CodeInfo
		{
			public uint Code;
			public int TryCounter;
			public DateTime Created;
		}
	}
}
