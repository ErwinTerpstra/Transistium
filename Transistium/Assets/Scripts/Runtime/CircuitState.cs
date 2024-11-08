﻿using System;
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

		public readonly CurrentDirection[] resistors;

		public readonly Signal[] wires;

		public CircuitState(Circuit circuit)
		{
			this.circuit = circuit;

			transistors = new bool[circuit.transistors.Count];
			resistors = new CurrentDirection[circuit.resistors.Count];
			wires = new Signal[circuit.WireCount];

			Reset();
		}

		public CircuitState(CircuitState other)
		{
			circuit = other.circuit;

			transistors = new bool[other.transistors.Length];
			resistors = new CurrentDirection[other.resistors.Length];
			wires = new Signal[other.wires.Length];

			CopyFrom(other);
		}

		public void Reset()
		{
			for (int i = 0, c = transistors.Length; i < c; ++i)
				transistors[i] = false;

			for (int i = 0, c = resistors.Length; i < c; ++i)
				resistors[i] = CurrentDirection.NONE;

			for (int i = 0, c = wires.Length; i < c; ++i)
				wires[i] = Signal.FLOATING;

			wires[Circuit.WIRE_VCC] = Signal.HIGH;
			wires[Circuit.WIRE_GND] = Signal.LOW;
		}

		public void CopyFrom(CircuitState other)
		{
			Array.Copy(other.transistors, transistors, transistors.Length);
			Array.Copy(other.resistors, resistors, resistors.Length);
			Array.Copy(other.wires, wires, wires.Length);
		}
	}
}