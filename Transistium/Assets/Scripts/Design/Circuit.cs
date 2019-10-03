using System.Collections.Generic;

namespace Transistium.Design
{
	public class Circuit
	{
		private List<Transistor> transistors;

		private List<Junction> junctions;

		private List<Wire> wires;

		private Handle vcc;

		private Handle ground;
		
		public Circuit()
		{
			transistors = new List<Transistor>();
			junctions = new List<Junction>();
			wires = new List<Wire>();

			vcc = AddJunction();
			ground = AddJunction();
		}

		public Handle AddTransistor()
		{
			var transistor = new Transistor();

			transistor.gate = AddJunction();
			transistor.drain = AddJunction();
			transistor.source = AddJunction();

			transistors.Add(transistor);

			return new Handle(transistors.Count - 1);
		}

		public Handle AddJunction()
		{
			junctions.Add(new Junction());

			return new Handle(junctions.Count - 1);
		}

		public Handle AddWire(Handle junctionHandle)
		{
			var wire = new Wire();
			wire.a = junctionHandle;

			wires.Add(wire);

			var wireHandle = new Handle(wires.Count - 1);

			var junction = GetJunction(junctionHandle);
			junction.wires.Add(wireHandle);

			return wireHandle;
		}

		public Transistor GetTransistor(Handle handle)
		{
			return transistors[handle.index];
		}

		public Junction GetJunction(Handle handle)
		{
			return junctions[handle.index];
		}

		public Wire GetWire(Handle handle)
		{
			return wires[handle.index];
		}

		public void RemoveTransistor(Handle transistorHandle)
		{
			var transistor = GetTransistor(transistorHandle);

			RemoveJunction(transistor.gate);
			RemoveJunction(transistor.drain);
			RemoveJunction(transistor.source);

			transistors.RemoveAt(transistorHandle.index);
		}

		public void RemoveJunction(Handle junctionHandle)
		{
			var junction = GetJunction(junctionHandle);

			foreach (var wireHandle in junction.wires)
				RemoveWire(wireHandle);

			junctions.RemoveAt(junctionHandle.index);
		}

		public void RemoveWire(Handle wireHandle)
		{
			var wire = GetWire(wireHandle);

			if (wire.a != Handle.Invalid)
			{
				var junction = GetJunction(wire.a);
				junction.wires.Remove(wireHandle);

				if (junction.wires.Count == 0)
					RemoveJunction(wire.a);
			}

			if (wire.b != Handle.Invalid)
			{
				var junction = GetJunction(wire.b);
				junction.wires.Remove(wireHandle);

				if (junction.wires.Count == 0)
					RemoveJunction(wire.b);
			}

			wires.RemoveAt(wireHandle.index);
		}


		public void Connect(Handle junctionA, Handle junctionB)
		{
			if (AreConnected(junctionA, junctionB))
				return;

			wires.Add(new Wire()
			{
				a = junctionA,
				b = junctionB,
			});

			Handle handle = new Handle(wires.Count - 1);

			Junction ja = GetJunction(junctionA);
			Junction jb = GetJunction(junctionB);

			ja.wires.Add(handle);
			jb.wires.Add(handle);
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

		public List<Transistor> Transistors => transistors;
		public List<Junction> Junctions => junctions;
		public List<Wire> Wires => wires;

		public Handle Vcc => vcc;
		public Handle Ground => ground;
	}

}