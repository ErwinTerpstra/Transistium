using System;

namespace Transistium.Design
{
	[Flags]
	public enum CircuitElementFlags
	{
		NONE = 0,
		STATIC = 1,			// Static elements can't be moved
		PERMANENT = 2,      // Permanent elements can't be removed

		// Embedded elements are directly managed by another part and shouldn't be created/destroyed separate from their embedder
		EMBEDDED = STATIC | PERMANENT
	}

	public class CircuitElement
	{
		public CircuitElementFlags flags;

		public Transformation transform;

		public CircuitElement()
		{

		}

	}

	public static class CircuitElementExtensions
	{
		public static bool Has(this CircuitElementFlags flags, CircuitElementFlags flag)
		{
			return (flags & flag) != 0;
		}
	}

}