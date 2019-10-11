using System;
using System.Collections.Generic;

namespace Transistium.Util
{
	[Serializable]
	public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>
	{
		public readonly TFirst first;

		public readonly TSecond second;

		public Pair(TFirst first, TSecond second)
		{
			this.first = first;
			this.second = second;
		}

		public override int GetHashCode()
		{
			return first.GetHashCode() << 13 + second.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Pair<TFirst, TSecond>)
				return Equals((Pair<TFirst, TSecond>)obj);

			return base.Equals(obj);
		}

		public bool Equals(Pair<TFirst, TSecond> other)
		{
			return other.first.Equals(first) && other.second.Equals(second);
		}

		public static bool operator ==(Pair<TFirst, TSecond> lhs, Pair<TFirst, TSecond> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Pair<TFirst, TSecond> lhs, Pair<TFirst, TSecond> rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
