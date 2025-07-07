using System;

namespace GroupCheck.WebServer.Services
{
	public interface ISMSService
	{
		void SendSMS(string phone, string message);
	}
}
