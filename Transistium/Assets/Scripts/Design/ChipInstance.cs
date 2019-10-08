using System.Collections.Generic;

namespace Transistium.Design
{
	public class PinInstance
	{
		public string pinGUID;

		public Handle junctionHandle;
	}

	public class ChipInstance
	{
		public string chipGUID;

		public List<PinInstance> pins;

		public ChipInstance()
		{
			pins = new List<PinInstance>();
		}
	}
}
