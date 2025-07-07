using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GroupCheck.Tools.Extensions.HashExt
{
	public static class HashExtension
	{
		public static byte[] CalculateHash(this byte[] value)
		{
			return Hash.Calculate(value);
		}
		public static byte[] CalculateHash(this byte[] value, string algorithm)
		{
			return Hash.Calculate(value, algorithm);
		}
		public static byte[] CalculateHash(this byte[] value, string algorithm, byte[] key)
		{
			return Hash.Calculate(value, algorithm, key);
		}
	}
}
