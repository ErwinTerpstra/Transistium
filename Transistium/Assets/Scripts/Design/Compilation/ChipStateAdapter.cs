using System.Collections;
using System.Collections.Generic;
using Transistium.Runtime;
using UnityEngine;

namespace Transistium.Design
{
	public class ChipStateAdapter
	{
		private readonly Chip chip;

		private readonly ChipMapping mapping;

		private CircuitState currentState;

		private Dictionary<int, Signal> outputSignals;

		public ChipStateAdapter(Chip chip, ChipMapping mapping)
		{
			this.chip = chip;
			this.mapping = mapping;

			outputSignals = new Dictionary<int, Signal>();
		}

		public Signal Read(Guid pinID)
		{
			Pin pin = chip.pins[pinID];
			Junction junction = chip.circuit.junctions[pin.junctionHandle];
			
			int wire = mapping.junctionMapping[junction];

			return currentState.wires[wire];
		}

		public void Write(Guid pinID, Signal signal)
		{
			Pin pin = chip.pins[pinID];
			Junction junction = chip.circuit.junctions[pin.junctionHandle];

			int wire = mapping.junctionMapping[junction];
			outputSignals[wire] = signal;
		}

		public void Reset(CircuitState state)
		{
			currentState = state;
			outputSignals.Clear();
		}

		public void WriteToState(CircuitState state)
		{
			foreach (var pair in outputSignals)
				state.wires[pair.Key] = pair.Value;
		}
	}
}