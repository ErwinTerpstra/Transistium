using System.Collections.Generic;

namespace Transistium.Runtime
{
	public class Circuit
	{
		public const int WIRE_VCC = 0;
		public const int WIRE_GND = 1;
		public const int BUILT_IN_WIRE_COUNT = 2;

		public readonly List<Transistor> transistors;

		public readonly List<Resistor> resistors;

		public readonly List<Diode> diodes;

		private int wireCount;

		public int WireCount => wireCount;

		public Circuit()
		{
			transistors = new List<Transistor>();
			resistors = new List<Resistor>();
			diodes = new List<Diode>();

			wireCount = BUILT_IN_WIRE_COUNT;
		}

		public int AddWire() => wireCount++;

		public int AddTransistor(Transistor transistor)
		{
			transistors.Add(transistor);
			return transistors.Count - 1;
		}

		public void Tick(CircuitState current, CircuitState next)
		{
			// Tick diodes
			for (int i = 0, c = diodes.Count; i < c; ++i)
				diodes[i].Tick(current, next);

			// Tick resistors
			for (int i = 0, c = resistors.Count; i < c; ++i)
				next.resistors[i] = resistors[i].Tick(current.resistors[i], current, next);

			// Tick transistors
			for (int i = 0, c = transistors.Count; i < c; ++i)
				next.transistors[i] = transistors[i].Tick(current, next);
		}
	}

}