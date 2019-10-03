using System.Collections.Generic;

using Transistium.Util;

namespace Transistium.Design
{
	public class CircuitCompiler
	{
		private Runtime.Circuit compiledCircuit;

		private OneToManyMapping<int, Handle> junctionMapping;

		public CircuitCompiler()
		{
			junctionMapping = new OneToManyMapping<int, Handle>();
		}

		public void Compile(Circuit circuit)
		{
			compiledCircuit = new Runtime.Circuit();

			// Collect all junctions in the circuit
			List<Handle> allJunctions = new List<Handle>();

			allJunctions.Add(circuit.Vcc);
			allJunctions.Add(circuit.Ground);

			foreach (var transistor in circuit.Transistors)
				transistor.CollectJunctions(allJunctions);

			// Map all connected junctions to a single wire
			List<Handle> connectedJunctions = new List<Handle>();
			foreach (var junctionHandle in allJunctions)
			{
				if (junctionMapping.Contains(junctionHandle))
					continue;

				int wire = compiledCircuit.AddWire();

				// Collect connected junctions to this one
				connectedJunctions.Clear();
				circuit.CollectConnectedJunctions(junctionHandle, connectedJunctions);

				// Store the created wire mapping for all connected junctions
				junctionMapping.Map(wire, connectedJunctions);
			}

			// Create transistors
			foreach (var transistor in circuit.Transistors)
			{
				compiledCircuit.transistors.Add(new Runtime.Transistor()
				{
					gate	= junctionMapping[transistor.gate],
					drain	= junctionMapping[transistor.drain],
					source	= junctionMapping[transistor.source],
				});
			}
		}

		public Runtime.Circuit CompiledCircuit => compiledCircuit;
	}

}