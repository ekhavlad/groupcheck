using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using GroupCheck.Tools;

namespace GroupCheck.WebServer.Services
{
	public class SMSService : ISMSService
	{
		public void SendSMS(string phone, string message)
		{
			var request = $"https://sms.ru/sms/send?api_id=E8F0C510-4F94-0067-E6C0-BE2C6D3429C8&to={phone}&msg={message}&json=1";
			var response = HTTP.MakeRequest("GET", request, "plain/text", null, null);
		}
	}
}
