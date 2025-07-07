using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GroupCheck.WebServer.Services
{
	public class SqlLogger : BaseLogger
	{
		private readonly string _connectionString;
		public SqlLogger(string connectionString) : base()
		{
			_connectionString = connectionString;
		}
		public SqlLogger(string connectionString, long requestID) : base(requestID)
		{
			_connectionString = connectionString;
		}

		public override void Log(LogLevel logLevel, string msg)
		{
			if (logLevel < LogLevel) return;

			using (var conn = new SqlConnection(_connectionString))
			{
				using (var cmd = new SqlCommand("LogMessage", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@RequestID", RequestID);
					cmd.Parameters.AddWithValue("@DateAndTime", DateTime.UtcNow);
					cmd.Parameters.AddWithValue("@LogLevel", logLevel);
					cmd.Parameters.AddWithValue("@Message", msg);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}
		public override void Log(LogLevel logLevel, Exception ex)
		{
			if (logLevel < LogLevel || ex == null) return;
			Log(logLevel, $"{ex.Message} TRACE: {ex.StackTrace}");
		}

		public override void LogRequest(DateTime requestTime, string method, short responseCode, int duration, string url, string request, string response)
		{
			using (var conn = new SqlConnection(_connectionString))
			{
				using (var cmd = new SqlCommand("LogRequest", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@ID", RequestID);
					cmd.Parameters.AddWithValue("@DateAndTime", requestTime);
					cmd.Parameters.AddWithValue("@Method", method.Substring(0, 3));
					cmd.Parameters.AddWithValue("@ResponseCode", responseCode);
					cmd.Parameters.AddWithValue("@Duration", duration);
					cmd.Parameters.AddWithValue("@Url", url);
					cmd.Parameters.AddWithValue("@Request", (object)request ?? DBNull.Value);
					cmd.Parameters.AddWithValue("@Response", (object)response ?? DBNull.Value);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		public static long GetLastRequestNbr(string connectionString)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				using (var cmd = new SqlCommand("SELECT MAX(ID) FROM Requests", conn))
				{
					conn.Open();
					var obj = cmd.ExecuteScalar();
					var max = (obj == DBNull.Value) ? 0 : (long)obj;
					return max;
				}
			}
		}
	}
}
