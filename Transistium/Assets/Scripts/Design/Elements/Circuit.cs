using System.Collections.Generic;

namespace Transistium.Design
{
	public class Circuit
	{
		public HandleList<Transistor> transistors;

		public HandleList<Junction> junctions;

		public HandleList<Wire> wires;

		public HandleList<ChipInstance> chipInstances;
				
		public Circuit()
		{
			transistors = new HandleList<Transistor>((byte) ElementType.TRANSISTOR);
			junctions = new HandleList<Junction>((byte) ElementType.JUNCTION);
			wires = new HandleList<Wire>((byte) ElementType.WIRE);

			chipInstances = new HandleList<ChipInstance>((byte) ElementType.CHIP_INSTANCE);
		}

		public Transistor AddTransistor(out Handle<Transistor> handle)
		{
			var transistor = new Transistor();

			var flags = CircuitElementFlags.EMBEDDED;
			AddJunction(flags, out transistor.@base);
			AddJunction(flags, out transistor.collector);
			AddJunction(flags, out transistor.emitter);

			handle = transistors.Add(transistor);
			
			return transistor;
		}

		public Junction AddJunction(CircuitElementFlags flags, out Handle<Junction> handle)
		{
			return AddJunction(Guid.Generate(), flags, out handle);
		}

		public Junction AddJunction(Guid guid, CircuitElementFlags flags, out Handle<Junction> handle)
		{
			var junction = new Junction()
			{
				flags = flags
			};

			handle = junctions.Add(guid, junction);

			return junction;
		}

		public Wire AddWire(Junction junction, out Handle<Wire> handle)
		{
			var junctionHandle = junctions.LookupHandle(junction);

			var wire = new Wire();
			wire.a = junctionHandle;
			
			handle = wires.Add(wire);

			junction.wires.Add(handle);
			
			return wire;
		}

		public ChipInstance InstantiateChip(Chip chip, Handle<Chip> chipHandle)
		{
			var chipInstance = new ChipInstance()
			{
				chipHandle = chipHandle
			};

			foreach (var pin in chip.pins)
			{
				if (!chip.ShouldInstantiatePin(chip.pins.LookupHandle(pin)))
					continue;

				InstantiatePin(pin, chip, chipInstance);
			}

			chipInstances.Add(chipInstance);

			return chipInstance;
		}

		public PinInstance InstantiatePin(Pin pin, Chip chip, ChipInstance chipInstance)
		{
			var pinInstance = new PinInstance()
			{
				pinHandle = chip.pins.LookupHandle(pin),
				flags = CircuitElementFlags.EMBEDDED,
			};

			pinInstance.transform.rotation = pin.side.ToRotation();

			AddJunction(CircuitElementFlags.EMBEDDED, out pinInstance.junctionHandle);

			chipInstance.pins.Add(pinInstance);

			return pinInstance;
		}
		
		public void RemoveTransistor(Transistor transistor)
		{
			RemoveJunction(junctions[transistor.@base]);
			RemoveJunction(junctions[transistor.collector]);
			RemoveJunction(junctions[transistor.emitter]);

			transistors.Remove(transistor);
		}

		public void RemoveJunction(Junction junction)
		{
			for (int i = junction.wires.Count - 1; i >= 0; --i)
				RemoveWire(wires[junction.wires[i]]);

			junctions.Remove(junction);
		}

		public void RemoveJunction(Handle<Junction> handle)
		{
			RemoveJunction(junctions[handle]);
		}

		public void RemoveWire(Wire wire)
		{
			var handle = wires.LookupHandle(wire);

			if (wire.a != Handle<Junction>.Invalid)
			{
				var junction = junctions[wire.a];
				junction.wires.Remove(handle);

				if (!junction.flags.Has(CircuitElementFlags.PERMANENT) && junction.wires.Count == 0)
					RemoveJunction(junction);
			}

			if (wire.b != Handle<Junction>.Invalid)
			{
				var junction = junctions[wire.b];
				junction.wires.Remove(handle);

				if (!junction.flags.Has(CircuitElementFlags.PERMANENT) && junction.wires.Count == 0)
					RemoveJunction(junction);
			}

			wires.Remove(wire);
		}

		public void RemoveChipInstance(ChipInstance chipInstance)
		{
			for (int i = chipInstance.pins.Count - 1; i >= 0; --i)
				RemovePinInstance(chipInstance, chipInstance.pins[i]);

			chipInstances.Remove(chipInstance);
		}

		public void RemovePinInstance(ChipInstance chipInstance, PinInstance pinInstance)
		{
			RemoveJunction(pinInstance.junctionHandle);

			chipInstance.pins.Remove(pinInstance);
		}

		public bool AreConnected(Junction a, Junction b)
		{
			var ha = junctions.LookupHandle(a);
			var hb = junctions.LookupHandle(b);

			foreach (var wireHandle in a.wires)
			{
				var wire = wires[wireHandle];

				if (wire.Connects(ha, hb))
					return true;
			}

			return false;
		}
		
		private Junction GetConnectedJunction(Junction junction, Wire wire)
		{
			var junctionHandle = junctions.LookupHandle(junction);

			if (wire.a == junctionHandle)
				return junctions[wire.b];

			if (wire.b == junctionHandle)
				return junctions[wire.a];

			return null;
		}

		public void CollectConnectedJunctions(Junction junction, List<Junction> connectedJunctions)
		{
			connectedJunctions.Add(junction);

			foreach (var wireHandle in junction.wires)
			{
				var connectedJunction = GetConnectedJunction(junction, wires[wireHandle]);

				if (connectedJunction != null && !connectedJunctions.Contains(connectedJunction))
					CollectConnectedJunctions(connectedJunction, connectedJunctions);
			}
		}

	}

}