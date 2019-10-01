using System.Collections.Generic;

namespace Transistium.Runtime
{
	public class Circuit
	{
		public const int WIRE_VCC = 0;
		public const int WIRE_GND = 1;
		public const int WIRE_CLOCK = 2;

		public List<Transistor> transistors;

		public List<Wire> wires;

		public Circuit()
		{
			transistors = new List<Transistor>();
			wires = new List<Wire>();

			wires.Add(new Wire { state = WireState.PULLED_HIGH });	// Vcc
			wires.Add(new Wire { state = WireState.PULLED_LOW });	// GND
			wires.Add(new Wire { state = WireState.PULLED_LOW });	// Clock
		}

		public int AddWire()
		{
			wires.Add(new Wire()
			{
				state = WireState.UNKNOWN
			});

			return wires.Count - 1;
		}


	}

}