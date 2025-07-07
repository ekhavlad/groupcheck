using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GroupCheck.Tools
{
	public static class JWT
	{
		public static string DefaultHashAlgorithm = Hash.Algorithm.RS256;

		public static string Encode(object payload, byte[] key)
		{
			return Encode(payload, DefaultHashAlgorithm, key);
		}
		public static string Encode(object payload, string algorithm, byte[] key)
		{
			var header = new { alg = algorithm, typ = "JWT" };
			var serializedHeader = JSON.Serialize(header);
			var headerBytes = UTF8.GetBytes(serializedHeader);
			var headerPrepared = Base64.ToUrlBase64(headerBytes);

			var serializedPayload = JSON.Serialize(payload);
			var payloadBytes = UTF8.GetBytes(serializedPayload);
			var payloadPrepared = Base64.ToUrlBase64(payloadBytes);

			var signature = CalculateSign(headerPrepared, payloadPrepared, key, algorithm);

			var token = headerPrepared + "." + payloadPrepared + "." + signature;

			return token;
		}
		public static string Decode(string token, byte[] key, bool verify)
		{
			var parts = token.Split('.');
			var header = parts[0];
			var payload = parts[1];
			var sign = parts[2];

			var payloadJson = UTF8.GetString(Base64.DecodeUrl(payload));

			if (verify)
			{
				var headerJson = UTF8.GetString(Base64.DecodeUrl(header));
				var headerData = JSON.Deserialize<Dictionary<string,string>>(headerJson);
				var algorithm = (string)headerData["alg"];
				var calculatedSign = CalculateSign(header, payload, key, algorithm);
				if (calculatedSign != sign)
				{
					throw new InvalidJWTSignatureException();
				}
			}

			return payloadJson;
		}

		private static string CalculateSign(string header, string payload, byte[] key, string algorithm)
		{
			var bytesToSign = UTF8.GetBytes(header + "." + payload);
			var calculatedSign = Hash.Calculate(bytesToSign, algorithm, key);
			var calculatedSignBase64 = Base64.ToUrlBase64(calculatedSign);
			return calculatedSignBase64;
		}

		[Serializable]
		private class InvalidJWTSignatureException : Exception
		{
			public InvalidJWTSignatureException() { }
			public InvalidJWTSignatureException(string message) : base(message) { }
			public InvalidJWTSignatureException(string message, Exception innerException) : base(message, innerException) { }
		}
	}
}
