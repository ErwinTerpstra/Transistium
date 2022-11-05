using System;
using UnityEngine;

namespace Transistium.Runtime
{
	/// <summary>
	/// Represents the state of all components and signals in a compiled ciruit
	/// </summary>
	public class CircuitState
	{
		public readonly Circuit circuit;

		public readonly bool[] transistors;

		public readonly Signal[] wires;

		public CircuitState(Circuit circuit)
		{
			this.circuit = circuit;

			transistors = new bool[circuit.transistors.Count];
			wires = new Signal[circuit.WireCount];

			Reset();
		}

		public CircuitState(CircuitState other)
		{
			circuit = other.circuit;

			transistors = new bool[other.transistors.Length];
			wires = new Signal[other.wires.Length];

			Array.Copy(other.transistors, transistors, transistors.Length);
			Array.Copy(other.wires, wires, wires.Length);
		}

		public void Reset()
		{
			for (int i = 0, c = transistors.Length; i < c; ++i)
				transistors[i] = false;

			for (int i = 0, c = wires.Length; i < c; ++i)
				wires[i] = Signal.FLOATING;

			wires[Circuit.WIRE_VCC] = Signal.HIGH;
			wires[Circuit.WIRE_GND] = Signal.LOW;
		}

	}
}