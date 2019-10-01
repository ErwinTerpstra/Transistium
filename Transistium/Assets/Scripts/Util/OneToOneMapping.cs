using System;
using System.Collections.Generic;

namespace Transistium.Util
{
	public class OneToOneMapping<A, B>
	{
		private Dictionary<A, B> forward;

		private Dictionary<B, A> backward;

		public OneToOneMapping()
		{
			forward = new Dictionary<A, B>();
			backward = new Dictionary<B, A>();
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
	}

}