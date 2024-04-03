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

		public static float DistanceToLine(Vector2 a, Vector2 b, Vector2 p, out Vector2 pointOnLine)
		{
			Vector2 ab = b - a;
			Vector2 ap = p - a;

			float length = ab.magnitude;
			Vector2 direction = ab / length;

			float dot = Mathf.Clamp(Vector3.Dot(direction, ap), 0.0f, length);
			pointOnLine = a + direction * dot;

			return Vector2.Distance(pointOnLine, p);
		}

	}
}
