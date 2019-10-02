using UnityEngine;

namespace Transistium.Design
{
	public enum Rotation
	{
		NONE,
		ROTATE_90,
		ROTATE_180,
		ROTATE_270
	}

	public struct Transformation
	{
		public Vector2 position;

		public Rotation rotation;
	}
}
