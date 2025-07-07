using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools
{
	public static class Base64
	{
		/// <summary>
		/// RFC 3548: The Base 64 Alphabet.
		/// </summary>
		public static string ToBase64(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = Convert.ToBase64String(input);
			return result;
		}
		/// <summary>
		/// RFC 3548: The Base 64 Alphabet.
		/// </summary>
		public static byte[] Decode(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = Convert.FromBase64String(input);
			return result;
		}
		/// <summary>
		/// RFC 3548, 4: The "URL and Filename safe" Base 64 Alphabet.
		/// </summary>
		public static string ToUrlBase64(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = ToBase64(input);
			result = result.TrimEnd('=');
			result = result.Replace('+', '-');
			result = result.Replace('/', '_');
			return result;
		}
		/// <summary>
		/// RFC 3548, 4: The "URL and Filename safe" Base 64 Alphabet.
		/// </summary>
		public static byte[] DecodeUrl(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			input = input.Replace('-', '+');
			input = input.Replace('_', '/');

			if (input.Length % 4 == 2) input += "==";
			else if (input.Length % 4 == 3) input += "=";

			var result = Decode(input);
			return result;
		}
	}
}
