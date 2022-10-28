using System.Collections.Generic;

using Transistium.Util;

namespace Transistium.Design
{
	public class CircuitCompiler
	{
		private readonly Project project;

		private readonly List<Junction> junctionBuffer;

		private readonly OneToManyMapping<int, Junction> junctionMapping;

		private Runtime.Circuit compiledCircuit;

		public CircuitCompiler(Project project)
		{
			this.project = project;

			junctionBuffer = new List<Junction>();
			junctionMapping = new OneToManyMapping<int, Junction>();
		}

		public Runtime.Circuit Compile(Project project)
		{
			junctionMapping.Clear();

			compiledCircuit = new Runtime.Circuit();

			CompileCircuit(project.RootChip.circuit);

			return compiledCircuit;
		}

		private void CompileCircuit(Chip chip)
		{
			var circuit = chip.circuit;

			// Map all connected junctions to a single wire
			foreach (var junction in circuit.junctions)
			{
				if (junctionMapping.Contains(junction))
					continue;

				int wire = compiledCircuit.AddWire();

				// Collect connected junctions to this one
				junctionBuffer.Clear();
				circuit.CollectConnectedJunctions(junction, junctionBuffer);

				// Store the created wire mapping for all connected junctions
				junctionMapping.Map(wire, junctionBuffer);
			}

			// Create transistors
			foreach (var transistor in circuit.transistors)
			{
				compiledCircuit.transistors.Add(new Runtime.Transistor()
				{
					gate = junctionMapping[circuit.junctions[transistor.gate]],
					drain = junctionMapping[circuit.junctions[transistor.drain]],
					source = junctionMapping[circuit.junctions[transistor.source]],
				});
			}

			// Compile chip instances
			foreach (var chipInstance in circuit.chipInstances)
			{
				var childChip = project.GetChip(chipInstance.chipHandle);

				// Recursively compile the circuit for this chip instance
				CompileCircuit(childChip);

				// Connect pin instances to the pin junctions on the "inside" of the chip
				foreach (var pinInstance in chipInstance.pins)
				{
					// Retrieve the pin for this pin instance
					var pin = childChip.pins[pinInstance.pinHandle];
					
					// Retrieve the outside (instance) and inside (blueprint) junctions
					var outsideJunction = circuit.junctions[pinInstance.junctionHandle];
					var insideJunction = childChip.circuit.junctions[pin.junctionHandle];

					// Retrieve the wires those junctions are mapped to
					int outsideWire = junctionMapping[outsideJunction];
					int insideWire = junctionMapping[insideJunction];

					// Connect inside and outside wires together in the compiled circuit
					// Diode configuration depends on the pin direction
					// In the case of bidirectional pins, we add a diode in both directions

					if (pin.direction == PinDirection.INPUT || pin.direction == PinDirection.BIDIRECTIONAL)
					{
						compiledCircuit.diodes.Add(new Runtime.Diode()
						{
							input = outsideWire,
							output = insideWire
						});
					}

					if (pin.direction == PinDirection.OUTPUT || pin.direction == PinDirection.BIDIRECTIONAL)
					{
						compiledCircuit.diodes.Add(new Runtime.Diode()
						{
							input = insideWire,
							output = outsideWire,
						});
					}
				}
			}

			// TODO: connect VCC and ground to compiled circuit
		}

	}

}