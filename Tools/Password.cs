using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GroupCheck.Tools
{
	public static class Password
	{
		private const string PASSWORD_CHARS = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

		/// <summary>
		/// Generates passwords of the specified length.
		/// </summary>
		/// <param name="lenght"></param>
		/// <returns></returns>
		public static string Generate(int lenght)
		{
			var chars = new char[lenght];
			var bytes = Random.GenerateBytes(lenght);
			var mod = PASSWORD_CHARS.Length;
			for (var i = 0; i < lenght; i++)
			{
				chars[i] = PASSWORD_CHARS[bytes[i] % mod];
			}
			var password = new string(chars);
			return password;
		}
	}
}
