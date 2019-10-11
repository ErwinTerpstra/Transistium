using System.Collections.Generic;

namespace Transistium.Design
{
	public class PinInstance
	{
		public Handle<Pin> pinHandle;

		public Handle<Junction> junctionHandle;
	}

	public class ChipInstance
	{
		public Handle<Chip> chipHandle;

		public List<PinInstance> pins;

		public ChipInstance()
		{
			pins = new List<PinInstance>();
		}
	}
}
