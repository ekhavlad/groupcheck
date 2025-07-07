using System;

namespace GroupCheck.WebServer.Services
{
	public abstract class BaseLogger : ILogger
	{
		public LogLevel LogLevel { get; set; }

		protected readonly long _requestID;
		public long RequestID { get { return _requestID; } }

		public BaseLogger()
		{
		}

		public BaseLogger(long requestID)
		{
			_requestID = requestID;
		}

		public void Debug(string msg)
		{
			if (LogLevel <= LogLevel.Debug)
				Log(LogLevel.Debug, msg);
		}
		public void Debug(Exception ex)
		{
			if (LogLevel <= LogLevel.Debug)
				Log(LogLevel.Debug, ex);
		}
		public void Info(string msg)
		{
			if (LogLevel <= LogLevel.Info)
				Log(LogLevel.Info, msg);
		}
		public void Info(Exception ex)
		{
			if (LogLevel <= LogLevel.Info)
				Log(LogLevel.Info, ex);
		}
		public void Warning(string msg)
		{
			if (LogLevel <= LogLevel.Warn)
				Log(LogLevel.Warn, msg);
		}
		public void Warning(Exception ex)
		{
			if (LogLevel <= LogLevel.Warn)
				Log(LogLevel.Warn, ex);
		}
		public void Error(string msg)
		{
			if (LogLevel <= LogLevel.Error)
				Log(LogLevel.Error, msg);
		}
		public void Error(Exception ex)
		{
			if (LogLevel <= LogLevel.Error)
				Log(LogLevel.Error, ex);
		}
		public void Fatal(string msg)
		{
			if (LogLevel <= LogLevel.Fatal)
				Log(LogLevel.Fatal, msg);
		}
		public void Fatal(Exception ex)
		{
			if (LogLevel <= LogLevel.Fatal)
				Log(LogLevel.Fatal, ex);
		}

		public abstract void Log(LogLevel logLevel, string msg);
		public abstract void Log(LogLevel logLevel, Exception ex);

		public abstract void LogRequest(DateTime requestTime, string method, short responseCode, int duration, string url, string request, string response);
	}
}
