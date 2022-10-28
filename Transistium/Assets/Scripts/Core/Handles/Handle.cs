using System;

namespace Transistium
{
	[Serializable]
	public struct Handle<T> : IEquatable<Handle<T>> where T : class
	{
		public static readonly Handle<T> Invalid = new Handle<T>(null);

		public string guid;

		public bool IsValid => !string.IsNullOrEmpty(guid);

		public Handle(string guid)
		{
			this.guid = guid;
		}

		public override int GetHashCode()
		{
			return guid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(Handle<T> other)
		{
			return guid == other.guid;
		}

		public static bool operator ==(Handle<T> a, Handle<T> b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(Handle<T> a, Handle<T> b)
		{
			return !a.Equals(b);
		}
	}

}