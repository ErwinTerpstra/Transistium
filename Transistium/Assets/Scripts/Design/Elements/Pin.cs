
namespace Transistium.Design
{
	public enum PinSide
	{
		TOP,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public enum PinDirection
	{
		BIDIRECTIONAL,
		INPUT,
		OUTPUT,
	}

	public class Pin : CircuitElement
	{
		public string name;

		public PinSide side;

		public PinDirection direction;

		public Handle<Junction> junctionHandle;

		public string NameOrDefault => name ?? "???";
		
	}

	public static class PinExtensions
	{
		public static Rotation ToRotation(this PinSide side)
		{
			switch (side)
			{
				default:
				case PinSide.TOP:
					return Rotation.NONE;

				case PinSide.BOTTOM:
					return Rotation.ROTATE_180;

				case PinSide.RIGHT:
					return Rotation.ROTATE_90;

				case PinSide.LEFT:
					return Rotation.ROTATE_270;
			}
		}
	}
}
