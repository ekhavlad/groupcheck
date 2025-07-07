using System;
using System.Collections.Generic;
using System.Text;

namespace GroupCheck.Tools
{
	public class IntEncryptor
	{
		const ulong s0 = 0x4A92D80E6B1C7F53;
		const ulong s1 = 0xEB4C6DFA23810759;
		const ulong s2 = 0x581DA342EFC7609B;
		const ulong s3 = 0x7DA1089FE46CB253;
		const ulong s4 = 0x6C715FD84A9E03B2;
		const ulong s5 = 0x4BA0721D36859CFE;
		const ulong s6 = 0xDB413F590AE7682C;
		const ulong s7 = 0x105723469FDEAB8C;

		private const int ROUNDS = 5;
		private const int SHIFT = 11;
		private const int SHIFT_X = 31 - SHIFT;
		private const int INT_SIGN = unchecked((int)0x80000000);
		private const int INT_BODY = 0x7FFFFFFF;

		private static byte[] direct0;
		private static byte[] revert0;
		private static byte[] direct1;
		private static byte[] revert1;
		private static byte[] direct2;
		private static byte[] revert2;
		private static byte[] direct3;
		private static byte[] revert3;

		private int[] keys;

		static IntEncryptor()
		{
			var s = new ulong[] { s0, s1, s2, s3, s4, s5, s6, s7 };
			direct0 = new byte[256];
			revert0 = new byte[256];
			for (int i1 = 0; i1 < 16; i1++)
				for (int i0 = 0; i0 < 16; i0++)
				{
					var pos = (byte)(i1 * 16 + i0);
					var val = (byte)(
						((s1 >> (60 - i1 * 4)) % 16) * 16 +
						((s0 >> (60 - i0 * 4)) % 16)
						);
					direct0[pos] = val;
					revert0[val] = pos;
				}

			direct1 = new byte[256];
			revert1 = new byte[256];
			for (int i3 = 0; i3 < 16; i3++)
				for (int i2 = 0; i2 < 16; i2++)
				{
					var pos = (byte)(i3 * 16 + i2);
					var val = (byte)(
						((s3 >> (60 - i3 * 4)) % 16) * 16 +
						((s2 >> (60 - i2 * 4)) % 16)
						);
					direct1[pos] = val;
					revert1[val] = pos;
				}

			direct2 = new byte[256];
			revert2 = new byte[256];
			for (int i5 = 0; i5 < 16; i5++)
				for (int i4 = 0; i4 < 16; i4++)
				{
					var pos = (byte)(i5 * 16 + i4);
					var val = (byte)(
						((s5 >> (60 - i5 * 4)) % 16) * 16 +
						((s4 >> (60 - i4 * 4)) % 16)
						);
					direct2[pos] = val;
					revert2[val] = pos;
				}

			direct3 = new byte[128];
			revert3 = new byte[128];
			for (int i7 = 0; i7 < 8; i7++)
				for (int i6 = 0; i6 < 16; i6++)
				{
					var pos = (byte)(i7 * 16 + i6);
					var val = (byte)(
						((s7 >> (60 - i7 * 4)) % 16) * 16 +
						((s6 >> (60 - i6 * 4)) % 16)
						);
					direct3[pos] = val;
					revert3[val] = pos;
				}
		}

		public IntEncryptor(byte[] key)
		{
			if (key.Length % 4 != 0)
				throw new Exception("Invalid key length - must be divisible by 4.");

			if (key.Length / 4 < ROUNDS)
				throw new Exception($"Invalid key length - must be at least {ROUNDS} * 4.");

			keys = new int[ROUNDS];
			for (int i = 0; i < ROUNDS; i++)
			{
				keys[i] = BitConverter.ToInt32(key, i * 4) & INT_BODY;
			}
		}
		public IntEncryptor(string key) : this(ParseKey(key)) { }
		private static byte[] ParseKey(string value)
		{
			var key = new byte[value.Length / 2];
			for (int i = 0; i < value.Length / 2; i++)
			{
				key[i] = byte.Parse(value.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
			}
			return key;
		}
		public int Encrypt(int a)
		{
			var mask = a & INT_SIGN;
			a = a & INT_BODY;
			for (int i = 0; i < ROUNDS; i++)
			{
				a = a ^ keys[i];
				var byte0 = direct0[a & 0xFF];
				var byte1 = direct1[(a >> 8) & 0xFF];
				var byte2 = direct2[(a >> 16) & 0xFF];
				var byte3 = direct3[(a >> 24) & 0xFF];
				a = (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
				a = ((a << SHIFT) & INT_BODY) | (a >> SHIFT_X);
			}
			a = a | mask;
			return a;
		}
		public int Decrypt(int a)
		{
			var mask = a & INT_SIGN;
			a = a & INT_BODY;

			for (int i = ROUNDS - 1; i >= 0; i--)
			{
				a = ((a & INT_BODY) >> SHIFT) | ((a << SHIFT_X)) & INT_BODY;
				var byte0 = revert0[a & 0xFF];
				var byte1 = revert1[(a >> 8) & 0xFF];
				var byte2 = revert2[(a >> 16) & 0xFF];
				var byte3 = revert3[(a >> 24) & 0xFF];
				a = (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
				a = a ^ keys[i % keys.Length];
			}
			a = a | mask;
			return a;
		}
	}
}
