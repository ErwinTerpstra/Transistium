using System.Collections.Generic;

namespace Transistium.Design
{
	public delegate void CircuitHandleEvent(Handle handle);
	
	public class Circuit
	{
		public event CircuitHandleEvent TransistorAdded;
		public event CircuitHandleEvent TransistorRemoved;

		public event CircuitHandleEvent JunctionAdded;
		public event CircuitHandleEvent JunctionRemoved;

		public event CircuitHandleEvent WireAdded;
		public event CircuitHandleEvent WireRemoved;

		private HandleList<Transistor> transistors;

		private HandleList<Junction> junctions;

		private HandleList<Wire> wires;

		private List<ChipInstance> chipInstances;

		private Handle vcc;

		private Handle ground;
		
		public Circuit()
		{
			transistors = new HandleList<Transistor>();
			junctions = new HandleList<Junction>();
			wires = new HandleList<Wire>();

			chipInstances = new List<ChipInstance>();

			vcc = AddJunction(false);
			ground = AddJunction(false);
		}

		public Handle AddTransistor()
		{
			var transistor = new Transistor();

			transistor.gate = AddJunction(true);
			transistor.drain = AddJunction(true);
			transistor.source = AddJunction(true);

			var handle = transistors.Add(transistor);

			TransistorAdded?.Invoke(handle);

			return handle;
		}

		public Handle AddJunction(bool embedded)
		{
			var handle = junctions.Add(new Junction()
			{
				embedded = embedded
			});

			JunctionAdded?.Invoke(handle);

			return handle;
		}

		public Handle AddWire(Handle junctionHandle)
		{
			var wire = new Wire();
			wire.a = junctionHandle;
			
			var wireHandle = wires.Add(wire);

			var junction = GetJunction(junctionHandle);
			junction.wires.Add(wireHandle);

			WireAdded?.Invoke(wireHandle);

			return wireHandle;
		}

		public Transistor GetTransistor(Handle handle)
		{
			return transistors[handle];
		}

		public Junction GetJunction(Handle handle)
		{
			return junctions[handle];
		}

		public Wire GetWire(Handle handle)
		{
			return wires[handle];
		}

		public void RemoveTransistor(Handle handle)
		{
			var transistor = GetTransistor(handle);

			RemoveJunction(transistor.gate);
			RemoveJunction(transistor.drain);
			RemoveJunction(transistor.source);

			transistors.Remove(handle);

			TransistorRemoved?.Invoke(handle);
		}

		public void RemoveJunction(Handle handle)
		{
			var junction = GetJunction(handle);

			for (int i = junction.wires.Count - 1; i >= 0; --i)
				RemoveWire(junction.wires[i]);

			junctions.Remove(handle);

			JunctionRemoved?.Invoke(handle);
		}

		public void RemoveWire(Handle handle)
		{
			var wire = GetWire(handle);

			if (wire.a != Handle.Invalid)
			{
				var junction = GetJunction(wire.a);
				junction.wires.Remove(handle);

				if (!junction.embedded && junction.wires.Count == 0)
					RemoveJunction(wire.a);
			}

			if (wire.b != Handle.Invalid)
			{
				var junction = GetJunction(wire.b);
				junction.wires.Remove(handle);

				if (!junction.embedded && junction.wires.Count == 0)
					RemoveJunction(wire.b);
			}

			wires.Remove(handle);

			WireRemoved?.Invoke(handle);
		}
		
		public bool AreConnected(Handle junctionA, Handle junctionB)
		{
			Junction ja = GetJunction(junctionA);

			foreach (var wireHandle in ja.wires)
			{
				var wire = GetWire(wireHandle);

				if (wire.Connects(junctionA, junctionB))
					return true;
			}

			return false;
		}


		private Handle GetConnectedJunction(Handle junctionHandle, Handle wireHandle)
		{
			var wire = GetWire(wireHandle);

			if (wire.a == junctionHandle)
				return wire.b;

			if (wire.b == junctionHandle)
				return wire.a;

			return Handle.Invalid;
		}

		public void CollectConnectedJunctions(Handle junctionHandle, List<Handle> junctionHandles)
		{
			junctionHandles.Add(junctionHandle);

			var junction = GetJunction(junctionHandle);

			foreach (Handle wireHandle in junction.wires)
			{
				Handle connectedJunction = GetConnectedJunction(junctionHandle, wireHandle);

				if (connectedJunction != null && !junctionHandles.Contains(connectedJunction))
					CollectConnectedJunctions(connectedJunction, junctionHandles);
			}
		}

		public HandleList<Transistor> Transistors => transistors;
		public HandleList<Junction> Junctions => junctions;
		public HandleList<Wire> Wires => wires;

		public Handle Vcc => vcc;
		public Handle Ground => ground;
	}

}