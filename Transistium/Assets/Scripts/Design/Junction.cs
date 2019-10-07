using System.Collections.Generic;

namespace Transistium.Design
{
	public class Junction : CircuitElement
	{
		public bool embedded;

		public List<Handle> wires;

		public Junction()
		{
			wires = new List<Handle>();
		}
	}

}