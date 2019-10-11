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
			transistors = new HandleList<Transistor>();
			junctions = new HandleList<Junction>();
			wires = new HandleList<Wire>();

			chipInstances = new HandleList<ChipInstance>();
		}

		public Transistor AddTransistor(out Handle<Transistor> handle)
		{
			var transistor = new Transistor();

			var flags = CircuitElementFlags.EMBEDDED;
			AddJunction(flags, out transistor.gate);
			AddJunction(flags, out transistor.drain);
			AddJunction(flags, out transistor.source);

			handle = transistors.Add(transistor);
			
			return transistor;
		}

		public Junction AddJunction(CircuitElementFlags flags, out Handle<Junction> handle)
		{
			var junction = new Junction()
			{
				flags = flags
			};

			handle = junctions.Add(junction);
			
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

			foreach (var pinHandle in chip.pins)
			{
				var pinInstance = new PinInstance();

				AddJunction(CircuitElementFlags.EMBEDDED | CircuitElementFlags.PERMANENT | CircuitElementFlags.STATIC, out pinInstance.junctionHandle);

				chipInstance.pins.Add(pinInstance);
			}

			chipInstances.Add(chipInstance);

			return chipInstance;
		}
		
		public void RemoveTransistor(Transistor transistor)
		{
			RemoveJunction(junctions[transistor.gate]);
			RemoveJunction(junctions[transistor.drain]);
			RemoveJunction(junctions[transistor.source]);

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