using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools.Extensions.UnixDateTimeExt
{
	public static class UnixTimeExtension
	{
		public static int ToUnixTime(this DateTime input)
		{
			return UnixTime.ToUnixTime(input);
		}
		public static DateTime DecodeUnixTime(this int input)
		{
			return UnixTime.FromUnixTime(input);
		}
	}
}
