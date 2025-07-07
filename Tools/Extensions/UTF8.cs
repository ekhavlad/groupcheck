using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools.Extensions.UTF8Ext
{
	public static class UTF8Extension
	{
		public static byte[] GetUTF8Bytes(this string input)
		{
			return UTF8.GetBytes(input);
		}
		public static string GetUTF8String(this byte[] input)
		{
			return UTF8.GetString(input);
		}
	}
}
