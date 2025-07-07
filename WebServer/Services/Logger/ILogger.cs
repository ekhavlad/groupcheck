using System;

namespace GroupCheck.WebServer.Services
{
	public interface ILogger
	{
		LogLevel LogLevel { get; set; }
		long RequestID { get; }
		void Debug(string msg);
		void Debug(Exception ex);
		void Info(string msg);
		void Info(Exception ex);
		void Warning(string msg);
		void Warning(Exception ex);
		void Error(string msg);
		void Error(Exception ex);
		void Fatal(string msg);
		void Fatal(Exception ex);
		void Log(LogLevel logLevel, string msg);
		void Log(LogLevel logLevel, Exception ex);
		void LogRequest(DateTime requestTime, string method, short responseCode, int duration, string url, string request, string response);
	}

	public enum LogLevel
	{
		Debug = 0,
		Info = 1,
		Warn = 2,
		Error = 3,
		Fatal = 4
	}
}
