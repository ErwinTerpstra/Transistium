using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Wire
	{
		public Junction a, b;

		public List<Vector2> vertices;

		public Wire()
		{
			vertices = new List<Vector2>();
		}


		public bool Connects(Junction a, Junction b)
		{
			return (this.a == a && this.b == b) || (this.a == b && this.b == a);
		}

	}
}
