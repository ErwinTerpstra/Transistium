
using System;

namespace Transistium.Runtime
{
	/// <summary>
	/// Models a single valueless resistor.
	/// Can be used to prevent signals from being pulled low when connected to both Vcc and ground
	/// </summary>
	public struct Resistor
	{
		public int a, b;

		public CurrentDirection Tick(CurrentDirection previousDirection, CircuitState current, CircuitState next)
		{
			// Prepare our output variables
			CurrentDirection nextDirection;
			Signal outputA = Signal.FLOATING;
			Signal outputB = Signal.FLOATING;

			Signal sa = current.wires[a];
			Signal sb = current.wires[b];

			// Resistor behaviour can be modelled by ignoring low signals and considering them as floating
			// This simplifies further logic
			bool la = sa.ToLogicLevel();
			bool lb = sb.ToLogicLevel();

			if (la && lb)
			{
				// If both are high, it depends on our previous current direction
				// If we have no previous direction, two "High" pulses arrived at the same time, so no current flow happens either
				if (previousDirection == CurrentDirection.FORWARD)
					outputB = Signal.HIGH;
				else if (previousDirection == CurrentDirection.REVERSE)
					outputA = Signal.HIGH;

				// In any case, current direction stays the same
				nextDirection = previousDirection;
			}
			else if (la)
			{
				outputB = Signal.HIGH;
				nextDirection = CurrentDirection.FORWARD;
			}
			else if (lb)
			{
				outputA = Signal.HIGH;
				nextDirection = CurrentDirection.REVERSE;
			}
			else
			{
				// If both are low, no current flow happens
				nextDirection = CurrentDirection.NONE;
			}

			SignalUtil.Merge(outputA, ref next.wires[a]);
			SignalUtil.Merge(outputB, ref next.wires[b]);

			return nextDirection;
		}
	}

}