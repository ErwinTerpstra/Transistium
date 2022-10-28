using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Transistium
{
	public unsafe struct Guid : IEquatable<Guid>
	{
		private static RandomNumberGenerator rng = RandomNumberGenerator.Create();

		public const int LENGTH = 16;

		private fixed byte bytes[LENGTH];

		public byte this[int index]
		{
			get => bytes[index];
			set => bytes[index] = value;
		}

		public bool Equals(Guid other)
		{
			bool equal = true;

			for (int i = 0; i < LENGTH; ++i)
				equal &= bytes[i] == other.bytes[i];

			return equal;
		}

		public override bool Equals(object obj)
		{
			if (obj is Guid guid)
				return Equals(guid);

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hash = 17;

			for (int i = 0; i < LENGTH; ++i)
				hash = (hash * 13) ^ bytes[i];

			return hash;
		}

		public string ToShortString(int length = 3)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < length; ++i)
				builder.AppendFormat("{0:x2}", bytes[i]);

			return builder.ToString();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < LENGTH; ++i)
			{
				builder.AppendFormat("{0:x2}", bytes[i]);

				if (i == 3 || i == 5 || i == 7 || i == 9)
					builder.Append('-');
			}

			return builder.ToString();
		}

		public static bool operator ==(Guid lhs, Guid rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Guid lhs, Guid rhs)
		{
			return !lhs.Equals(rhs);
		}

		public static Guid Generate()
		{
			Guid guid = new Guid();

			byte[] buffer = new byte[LENGTH];
			rng.GetBytes(buffer);

			for (int i = 0; i < LENGTH; ++i)
				guid.bytes[i] = buffer[i];

			return guid;
		}

		public static Guid Hash(string input)
		{
			Guid guid = new Guid();
			Random random = new Random(input.GetStableHashCode());

			byte[] buffer = new byte[LENGTH];
			random.NextBytes(buffer);

			for (int i = 0; i < LENGTH; ++i)
				guid.bytes[i] = buffer[i];

			return guid;
		}

		public static Guid FromString(string input)
		{
			Guid guid = new Guid();

			input = input.Trim();
			input = input.Replace("-", "");
			input = input.Replace("_", "");

			if (input.Length != LENGTH * 2)
				throw new FormatException("Not a valid GUID format");

			for (int i = 0; i < LENGTH; ++i)
				guid.bytes[i] = (byte)int.Parse(input.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

			return guid;
		}
	}
}
