
namespace Transistium.Runtime
{
	/// <summary>
	/// Models a single valueless resistor.
	/// Can be used to prevent signals from being pulled low when connected to both Vcc and ground
	/// </summary>
	public struct Resistor
	{
		public static readonly Signal[] LookupTable = new Signal[]
		{
			// A (T)			// B (T)			// A (T+1)			// B (T+1)
			Signal.FLOATING,	Signal.FLOATING,	Signal.FLOATING,	Signal.FLOATING,
			Signal.FLOATING,    Signal.LOW,			Signal.LOW,			Signal.LOW,
			Signal.FLOATING,    Signal.HIGH,		Signal.HIGH,		Signal.HIGH,

			Signal.LOW,         Signal.FLOATING,	Signal.LOW,			Signal.FLOATING,
			Signal.LOW,         Signal.LOW,			Signal.LOW,			Signal.LOW,
			Signal.LOW,         Signal.HIGH,		Signal.LOW,			Signal.HIGH,

			Signal.HIGH,        Signal.FLOATING,	Signal.HIGH,		Signal.HIGH,
			Signal.HIGH,        Signal.LOW,			Signal.HIGH,		Signal.LOW,
			Signal.HIGH,        Signal.HIGH,		Signal.HIGH,		Signal.HIGH,
		};

		public int a, b;

		public void Tick(CircuitState current, CircuitState next)
		{
			Signal sa = current.wires[a];
			Signal sb = current.wires[b];

			const int columns = 4;
			int row = ((int)sa * columns * 3) +
					  ((int)sb * columns);

			next.wires[a] = LookupTable[row + 2];
			next.wires[b] = LookupTable[row + 3];
		}
	}

}