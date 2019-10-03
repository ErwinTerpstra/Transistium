using System;
using System.Collections.Generic;

using UnityEngine;

namespace Transistium
{
	public static class VectorUtil
	{
		public static Vector2 RotateCW(Vector2 v)
		{
			return new Vector2(v.y, -v.x);
		}

		public static Vector2 RotateCCW(Vector2 v)
		{
			return new Vector2(-v.y, v.x);
		}

		public static float Cross(Vector2 a, Vector2 b)
		{
			return (a.x * b.y) - (a.y * b.x);
		}

	}
}
