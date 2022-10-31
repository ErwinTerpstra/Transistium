
namespace Transistium.Runtime
{
	/// <summary>
	/// Models a single semiconductor to direct the signal flow
	/// </summary>
	public struct Diode
	{
		public static readonly Signal[] LookupTable = new Signal[]
		{
			// Input (T)		// Output (T)		// Input (T+1)		// Output (T+1)
			Signal.FLOATING,    Signal.FLOATING,    Signal.FLOATING,	Signal.FLOATING,
			Signal.FLOATING,    Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.FLOATING,    Signal.HIGH,        Signal.FLOATING,	Signal.HIGH,

			Signal.LOW,         Signal.FLOATING,    Signal.LOW,			Signal.FLOATING,
			Signal.LOW,         Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.LOW,         Signal.HIGH,        Signal.LOW,			Signal.HIGH,

			Signal.HIGH,        Signal.FLOATING,    Signal.HIGH,		Signal.HIGH,
			Signal.HIGH,        Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.HIGH,        Signal.HIGH,        Signal.HIGH,		Signal.HIGH,
		};

		public int input;

		public int output;

		public void Tick(CircuitState current, CircuitState next)
		{
			Signal si = current.wires[input];
			Signal so = current.wires[output];

			const int columns = 4;
			int row = ((int)si * columns * 3) +
					  ((int)so * columns);

			next.wires[input] = LookupTable[row + 2];
			next.wires[output] = LookupTable[row + 3];
		}
	}

}