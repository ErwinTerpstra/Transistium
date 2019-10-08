using System.Collections.Generic;

namespace Transistium.Design
{
	public class Pin
	{
		public string guid;

		public string name;

		public Handle junctionHandle;
	}

	public class Chip
	{
		public string guid;

		public string name;

		public Circuit circuit;

		public List<Pin> pins;

		public Chip()
		{
			circuit = new Circuit();
			pins = new List<Pin>();
		}

	}
}
