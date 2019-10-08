using System;
using System.Collections;
using System.Collections.Generic;

namespace Transistium.Util
{
	public class OneToOneMapping<A, B> : IEnumerable<KeyValuePair<A, B>>
	{
		private Dictionary<A, B> forward;

		private Dictionary<B, A> backward;

		public OneToOneMapping()
		{
			forward = new Dictionary<A, B>();
			backward = new Dictionary<B, A>();
		}

		public void Remove(A a)
		{
			B b;

			if (forward.TryGetValue(a, out b))
			{
				forward.Remove(a);
				backward.Remove(b);
			}
		}

		public void Remove(B b)
		{
			A a;

			if (backward.TryGetValue(b, out a))
			{
				backward.Remove(b);
				forward.Remove(a);
			}
		}


		public void Map(A a, B b)
		{
			forward.Add(a, b);
			backward.Add(b, a);
		}

		public bool Contains(A a)
		{
			return forward.ContainsKey(a);
		}

		public bool Contains(B b)
		{
			return backward.ContainsKey(b);
		}

		public bool TryGetValue(A key, out B value)
		{
			return forward.TryGetValue(key, out value);
		}

		public bool TryGetValue(B key, out A value)
		{
			return backward.TryGetValue(key, out value);
		}

		public void Clear()
		{
			forward.Clear();
			backward.Clear();
		}

		public IEnumerator<KeyValuePair<A, B>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<A, B>>)forward).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<A, B>>)forward).GetEnumerator();
		}
	}

}