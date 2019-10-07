using System;

namespace Transistium.Design
{
	[Serializable]
	public struct Handle : IEquatable<Handle>
	{
		public static readonly Handle Invalid = new Handle(-1);

		public int index;

		public Handle(int index)
		{
			this.index = index;
		}

		public override int GetHashCode()
		{
			return index;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(Handle other)
		{
			return index == other.index;
		}

		public static bool operator ==(Handle a, Handle b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(Handle a, Handle b)
		{
			return !a.Equals(b);
		}
	}

}