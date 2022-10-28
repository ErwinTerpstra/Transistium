using System.Collections.Generic;

namespace Transistium.Runtime
{
	public class Circuit
	{
		public const int WIRE_VCC = 0;
		public const int WIRE_GND = 1;

		public readonly List<Transistor> transistors;

		public readonly List<Diode> diodes;

		public readonly List<Wire> wires;

		public Circuit()
		{
			transistors = new List<Transistor>();
			diodes = new List<Diode>();
			wires = new List<Wire>();

			wires.Add(new Wire { state = WireState.HIGH });	// Vcc
			wires.Add(new Wire { state = WireState.LOW });	// GND
		}

		public int AddWire()
		{
			wires.Add(new Wire()
			{
				state = WireState.FLOATING
			});

			return wires.Count - 1;
		}


	}

}