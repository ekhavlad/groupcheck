using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools.Extensions.Base64Ext
{
	public static class Base64Extension
	{
		public static string ToBase64(this byte[] input)
		{
			return Base64.ToBase64(input);
		}
		public static byte[] DecodeBase64(this string input)
		{
			return Base64.Decode(input);
		}
		public static string ToUrlBase64(this byte[] input)
		{
			return Base64.ToUrlBase64(input);
		}
		public static byte[] DecodeUrlBase64(this string input)
		{
			return Base64.DecodeUrl(input);
		}
	}
}
