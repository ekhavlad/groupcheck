using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GroupCheck.Tools
{
	public static class Hash
	{
		private static readonly byte[] DEFAULT_HASH_KEY = new byte[0];

		public static string DefaultAlgorithm = Algorithm.RS256;
		public static class Algorithm
		{
			public const string RS256 = "RS256";
			public const string HS384 = "HS384";
			public const string HS512 = "HS512";
		}

		public static byte[] Calculate(byte[] value)
		{
			return Calculate(value, DefaultAlgorithm, DEFAULT_HASH_KEY);
		}
		public static byte[] Calculate(byte[] value, string algorithm)
		{
			return Calculate(value, algorithm, DEFAULT_HASH_KEY);
		}
		public static byte[] Calculate(byte[] value, string algorithm, byte[] key)
		{
			using (var alg = GetHashAlgorithm(algorithm, key))
			{
				return alg.ComputeHash(value);
			}
		}

		public static string Calculate(string value)
		{
			return Calculate(value, DefaultAlgorithm, DEFAULT_HASH_KEY);
		}
		public static string Calculate(string value, string algorithm)
		{
			return Calculate(value, algorithm, DEFAULT_HASH_KEY);
		}
		public static string Calculate(string value, string algorithm, byte[] key)
		{
			using (var alg = GetHashAlgorithm(algorithm, key))
			{
				var hash = alg.ComputeHash(UTF8.GetBytes(value));
				return Base64.ToBase64(hash);
			}
		}

		private static HashAlgorithm GetHashAlgorithm(string algorithm, byte[] key)
		{
			switch (algorithm)
			{
				case Algorithm.RS256: return new HMACSHA256(key);
				case Algorithm.HS384: return new HMACSHA384(key);
				case Algorithm.HS512: return new HMACSHA512(key);
				default: throw new UnsupportedHashAlgorithmException(algorithm);
			}
		}

		[Serializable]
		public class UnsupportedHashAlgorithmException : Exception
		{
			public UnsupportedHashAlgorithmException() { }
			public UnsupportedHashAlgorithmException(string message) : base(message) { }
			public UnsupportedHashAlgorithmException(string message, Exception innerException) : base(message, innerException) { }
		}
	}
}
