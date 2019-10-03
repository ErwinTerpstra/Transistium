using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Wire
	{
		public Handle a, b;

		public List<Vector2> vertices;

		public Wire()
		{
			a = Handle.Invalid;
			b = Handle.Invalid;

			vertices = new List<Vector2>();
		}

		public bool Connects(Handle a, Handle b)
		{
			return (this.a == a && this.b == b) || (this.a == b && this.b == a);
		}

	}
}
