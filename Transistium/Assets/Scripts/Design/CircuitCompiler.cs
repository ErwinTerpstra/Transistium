using System.Collections.Generic;

using Transistium.Util;

namespace Transistium.Design
{
	public class CircuitCompiler
	{
		private Runtime.Circuit compiledCircuit;

		private OneToManyMapping<int, Junction> junctionMapping;

		public void Compile(Circuit circuit)
		{
			compiledCircuit = new Runtime.Circuit();

			// Collect all junctions in the circuit
			List<Junction> allJunctions = new List<Junction>();

			allJunctions.Add(circuit.Vcc);
			allJunctions.Add(circuit.Ground);

			foreach (var transistor in circuit.transistors)
				transistor.CollectJunctions(allJunctions);

			// Map all connected junctions to a single wire
			List<Junction> connectedJunctions = new List<Junction>();
			foreach (var junction in allJunctions)
			{
				if (junctionMapping.Contains(junction))
					continue;

				int wire = compiledCircuit.AddWire();

				// Collect connected junctions to this one
				connectedJunctions.Clear();
				junction.CollectConnectedJunctions(connectedJunctions);

				// Store the created wire mapping for all connected junctions
				junctionMapping.Map(wire, connectedJunctions);
			}

			// Create transistors
			foreach (var transistor in circuit.transistors)
			{
				compiledCircuit.transistors.Add(new Runtime.Transistor()
				{
					gate = junctionMapping[transistor.gate],
					drain = junctionMapping[transistor.drain],
					source = junctionMapping[transistor.source],
				});
			}
		}

		public Runtime.Circuit CompiledCircuit => compiledCircuit;
	}

}