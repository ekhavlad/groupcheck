using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools
{
	public static class UTF8
	{
		public static byte[] GetBytes(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = Encoding.UTF8.GetBytes(input);
			return result;
		}
		public static string GetString(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = Encoding.UTF8.GetString(input);
			return result;
		}
	}
}
