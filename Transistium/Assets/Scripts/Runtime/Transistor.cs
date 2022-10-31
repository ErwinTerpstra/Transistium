
namespace Transistium.Runtime
{
	public struct Transistor
	{
		public static readonly Signal[] LookupTable = new Signal[]
		{
			// Base (T)			// Collector (T)	// Emitter (T)		// Collector (T+1)  // Emitter (T+1)
			Signal.FLOATING,    Signal.FLOATING,    Signal.FLOATING,    Signal.FLOATING,	Signal.FLOATING,
			Signal.FLOATING,    Signal.FLOATING,    Signal.LOW,         Signal.FLOATING,	Signal.LOW,
			Signal.FLOATING,    Signal.FLOATING,    Signal.HIGH,        Signal.FLOATING,	Signal.HIGH,
			
			Signal.FLOATING,    Signal.LOW,         Signal.FLOATING,    Signal.LOW,			Signal.FLOATING,
			Signal.FLOATING,    Signal.LOW,         Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.FLOATING,    Signal.LOW,         Signal.HIGH,        Signal.LOW,			Signal.HIGH,
			
			Signal.FLOATING,    Signal.HIGH,        Signal.FLOATING,    Signal.HIGH,		Signal.FLOATING,
			Signal.FLOATING,    Signal.HIGH,        Signal.LOW,         Signal.HIGH,		Signal.LOW,
			Signal.FLOATING,    Signal.HIGH,        Signal.HIGH,        Signal.HIGH,		Signal.HIGH,
			
			// Base (T)			// Collector (T)	// Emitter (T)		// Collector (T+1)  // Emitter (T+1)
			Signal.LOW,			Signal.FLOATING,    Signal.FLOATING,    Signal.FLOATING,	Signal.FLOATING,
			Signal.LOW,			Signal.FLOATING,    Signal.LOW,         Signal.FLOATING,	Signal.LOW,
			Signal.LOW,			Signal.FLOATING,    Signal.HIGH,        Signal.FLOATING,	Signal.HIGH,
			
			Signal.LOW,			Signal.LOW,         Signal.FLOATING,    Signal.LOW,			Signal.FLOATING,
			Signal.LOW,			Signal.LOW,         Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.LOW,			Signal.LOW,         Signal.HIGH,        Signal.LOW,			Signal.HIGH,
			
			Signal.LOW,			Signal.HIGH,        Signal.FLOATING,    Signal.HIGH,		Signal.FLOATING,
			Signal.LOW,			Signal.HIGH,        Signal.LOW,         Signal.HIGH,		Signal.LOW,
			Signal.LOW,			Signal.HIGH,        Signal.HIGH,        Signal.HIGH,		Signal.HIGH,
			
			// Base	(T)			// Collector (T)	// Emitter (T)		// Collector (T+1)  // Emitter (T+1)
			Signal.HIGH,		Signal.FLOATING,    Signal.FLOATING,    Signal.FLOATING,	Signal.FLOATING,
			Signal.HIGH,		Signal.FLOATING,    Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.HIGH,		Signal.FLOATING,    Signal.HIGH,        Signal.FLOATING,	Signal.HIGH,

			Signal.HIGH,		Signal.LOW,         Signal.FLOATING,    Signal.LOW,			Signal.FLOATING,
			Signal.HIGH,		Signal.LOW,         Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.HIGH,		Signal.LOW,         Signal.HIGH,        Signal.LOW,			Signal.HIGH,

			Signal.HIGH,		Signal.HIGH,        Signal.FLOATING,    Signal.HIGH,		Signal.HIGH,
			Signal.HIGH,		Signal.HIGH,        Signal.LOW,         Signal.LOW,			Signal.LOW,
			Signal.HIGH,		Signal.HIGH,        Signal.HIGH,        Signal.HIGH,		Signal.HIGH,

		};

		public int @base;

		public int collector;

		public int emitter;

		public bool Tick(CircuitState current, CircuitState next)
		{
			Signal sb = current.wires[@base];
			Signal sc = current.wires[collector];
			Signal se = current.wires[emitter];

			const int columns = 5;
			int row = ((int)sb * columns * 3 * columns * 3) +
					  ((int)sc * columns * 3) +
					  ((int)se * columns);

			next.wires[collector] = LookupTable[row + 3];
			next.wires[emitter] = LookupTable[row + 4];

			return sb.ToLogicLevel();
		}
	}

}