using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools
{
	public static class UnixTime
	{
		/// <summary>
		/// Initial DateTime point, that corresponds to 0 in Unix timestamp format.
		/// </summary>
		public static readonly DateTime START_POINT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Converts DateTime value to Unix timestamp format.
		/// </summary>
		public static int ToUnixTime(DateTime input)
		{
			var utc = input.ToUniversalTime();
			var timestamp = (int)(utc - START_POINT).TotalSeconds;
			return timestamp;
		}

		public static DateTime FromUnixTime(int input)
		{
			var date = START_POINT.AddSeconds(input);
			return date;
		}
	}
}
