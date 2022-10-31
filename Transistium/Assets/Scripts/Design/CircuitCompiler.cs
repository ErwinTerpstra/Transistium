using System.Collections.Generic;

using Transistium.Util;
using Transistium.Runtime;

namespace Transistium.Design
{
	public class CircuitCompiler
	{
		private readonly List<Junction> junctionBuffer;

		public CircuitCompiler()
		{ 
			junctionBuffer = new List<Junction>();
		}

		public CompilationResult Compile(Project project)
		{
			var compiledCircuit = new Runtime.Circuit();

			ChipMapping chipMapping = CompileCircuit(project, project.RootChip, compiledCircuit);

			return new CompilationResult()
			{
				circuit = compiledCircuit,
				symbols = new DebugSymbols()
				{
					rootChipMapping = chipMapping,
				}
			};
		}

		private ChipMapping CompileCircuit(Project project, Chip chip, Runtime.Circuit compiledCircuit)
		{
			ChipMapping chipMapping = new ChipMapping();

			var circuit = chip.circuit;

			// Map all connected junctions to a single wire
			foreach (var junction in circuit.junctions)
			{
				if (chipMapping.junctionMapping.Contains(junction))
					continue;

				int wire = compiledCircuit.AddWire();

				// Collect connected junctions to this one
				junctionBuffer.Clear();
				circuit.CollectConnectedJunctions(junction, junctionBuffer);

				// Store the created wire mapping for all connected junctions
				chipMapping.junctionMapping.Map(wire, junctionBuffer);
			}

			// Create transistors
			foreach (var transistor in circuit.transistors)
			{
				var runtimeTransistor = new Runtime.Transistor()
				{
					@base = chipMapping.junctionMapping[circuit.junctions[transistor.@base]],
					collector = chipMapping.junctionMapping[circuit.junctions[transistor.collector]],
					emitter = chipMapping.junctionMapping[circuit.junctions[transistor.emitter]],
				};

				// Add the transistor to the circuit
				int transistorIdx = compiledCircuit.AddTransistor(runtimeTransistor);
				
				// Save a mapping from the design transistor to the index of the runtime transistor
				chipMapping.transistorMapping.Add(transistor, transistorIdx);
			}

			// Compile chip instances
			foreach (var chipInstance in circuit.chipInstances)
			{
				var childChip = project.GetChip(chipInstance.chipHandle);

				// Recursively compile the circuit for this chip instance
				ChipMapping childChipMapping = CompileCircuit(project, childChip, compiledCircuit);

				// Store the child mapping
				childChipMapping.chipInstanceMapping.Add(chipInstance, childChipMapping);

				// Connect pin instances to the pin junctions on the "inside" of the chip
				foreach (var pinInstance in chipInstance.pins)
				{
					// Retrieve the pin for this pin instance
					var pin = childChip.pins[pinInstance.pinHandle];
					
					// Retrieve the outside (instance) and inside (blueprint) junctions
					var outsideJunction = circuit.junctions[pinInstance.junctionHandle];
					var insideJunction = childChip.circuit.junctions[pin.junctionHandle];

					// Retrieve the wires those junctions are mapped to
					int outsideWire = chipMapping.junctionMapping[outsideJunction];
					int insideWire = childChipMapping.junctionMapping[insideJunction];

					// Connect inside and outside wires together in the compiled circuit
					// Diode configuration depends on the pin direction

					// In the case of bidirectional pins, we add a diode in both directions
					// This in necessary since we don't have other logic to connect two wires together
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

			// Connect VCC and ground to compiled circuit

			// First retrieve the pins ...
			var vccPin = chip.pins[chip.vccPinHandle];
			var gndPin = chip.pins[chip.groundPinHandle];

			// Then get their junctions ..
			var vccJunction = chip.circuit.junctions[vccPin.junctionHandle];
			var gndJunction = chip.circuit.junctions[gndPin.junctionHandle];

			// And finally get their wires
			var vccWire = chipMapping.junctionMapping[vccJunction];
			var gndWire = chipMapping.junctionMapping[gndJunction];

			AddInputIsolator(Runtime.Circuit.WIRE_VCC, vccWire, compiledCircuit);
			AddGroundIsolator(Runtime.Circuit.WIRE_GND, gndWire, compiledCircuit);

			return chipMapping;
		}

		private void AddInputIsolator(int from, int to, Runtime.Circuit compiledCircuit)
		{
			int connectorWire = compiledCircuit.AddWire();

			compiledCircuit.diodes.Add(new Diode()
			{
				input = from,
				output = connectorWire
			});

			compiledCircuit.resistors.Add(new Resistor()
			{
				a = connectorWire,
				b = to
			});
		}

		private void AddGroundIsolator(int from, int to, Runtime.Circuit compiledCircuit)
		{
			compiledCircuit.diodes.Add(new Diode()
			{
				input = from,
				output = to
			});
		}

	}

}