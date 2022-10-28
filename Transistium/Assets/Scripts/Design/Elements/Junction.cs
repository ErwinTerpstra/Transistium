using System;
using System.Collections.Generic;

namespace Transistium.Design
{
	public class Junction : CircuitElement
	{
		public List<Handle<Wire>> wires;

		public Junction()
		{
			wires = new List<Handle<Wire>>();
		}
	}

}