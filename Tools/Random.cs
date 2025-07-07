using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GroupCheck.Tools
{
	public static class Random
	{
		/// <summary>
		/// Generates array of random bytes of the corresponded length.
		/// </summary>
		/// <param name="lenght">Array size.</param>
		/// <returns></returns>
		public static byte[] GenerateBytes(int lenght)
		{
			var result = new byte[lenght];
			using (var rnd = new RNGCryptoServiceProvider())
			{
				rnd.GetBytes(result);
			}
			return result;
		}
		/// <summary>
		/// Generates random UInt32 value.
		/// </summary>
		public static uint GenerateUInt32()
		{
			var bytes = GenerateBytes(4);
			return BitConverter.ToUInt32(bytes, 0);
		}
		/// <summary>
		/// Generates random UInt32 value: 0 &#8804; value &#60; <paramref name="max"/>.
		/// </summary>
		public static uint GenerateUInt32(uint max)
		{
			var tmp = GenerateUInt32();
			return tmp % max;
		}
		/// <summary>
		/// Generates random Int32 value.
		/// </summary>
		public static int GenerateInt32()
		{
			var bytes = GenerateBytes(4);
			return BitConverter.ToInt32(bytes, 0);
		}
		/// <summary>
		/// Generates random Int32 value: 0 &#8804; value &#60; <paramref name="max"/>.
		/// </summary>
		public static int GenerateInt32(int max)
		{
			var tmp = GenerateUInt32();
			return (int)tmp % max;
		}
		/// <summary>
		/// Generates random UInt32 value: <paramref name="min"/> &#8804; value &#60; <paramref name="max"/>.
		/// </summary>
		public static int GenerateInt32(int min, int max)
		{
			var diff = GenerateInt32(max - min);
			return min + diff;
		}
	}
}
