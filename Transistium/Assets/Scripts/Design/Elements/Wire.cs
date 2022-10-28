using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Wire
	{
		public Handle<Junction> a, b;

		public List<Vector2> vertices;

		public Wire()
		{
			a = Handle<Junction>.Invalid;
			b = Handle<Junction>.Invalid;

			vertices = new List<Vector2>();
		}

		public bool Connects(Handle<Junction> a, Handle<Junction> b)
		{
			return (this.a == a && this.b == b) || (this.a == b && this.b == a);
		}

	}
}
