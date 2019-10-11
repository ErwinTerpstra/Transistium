using System.Collections.Generic;

using Transistium.Util;

namespace Transistium.Design
{
	public class CircuitCompiler
	{
		private Runtime.Circuit compiledCircuit;

		private OneToManyMapping<int, Junction> junctionMapping;

		public CircuitCompiler()
		{
			junctionMapping = new OneToManyMapping<int, Junction>();
		}

		public void Compile(Circuit circuit)
		{
			compiledCircuit = new Runtime.Circuit();

			// Collect all junctions in the circuit
			List<Handle<Junction>> allJunctions = new List<Handle<Junction>>();
			
			foreach (var transistor in circuit.transistors)
				transistor.CollectJunctions(allJunctions);

			// Map all connected junctions to a single wire
			List<Junction> connectedJunctions = new List<Junction>();
			foreach (var junctionHandle in allJunctions)
			{
				var junction = circuit.junctions[junctionHandle];

				if (junctionMapping.Contains(junction))
					continue;

				int wire = compiledCircuit.AddWire();

				// Collect connected junctions to this one
				connectedJunctions.Clear();
				circuit.CollectConnectedJunctions(junction, connectedJunctions);

				// Store the created wire mapping for all connected junctions
				junctionMapping.Map(wire, connectedJunctions);
			}

			// Create transistors
			foreach (var transistor in circuit.transistors)
			{
				compiledCircuit.transistors.Add(new Runtime.Transistor()
				{
					gate	= junctionMapping[circuit.junctions[transistor.gate]],
					drain	= junctionMapping[circuit.junctions[transistor.drain]],
					source	= junctionMapping[circuit.junctions[transistor.source]],
				});
			}
		}

		public Runtime.Circuit CompiledCircuit => compiledCircuit;
	}

}