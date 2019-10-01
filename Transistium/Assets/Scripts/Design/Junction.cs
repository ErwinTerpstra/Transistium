using System.Collections.Generic;

namespace Transistium.Design
{
	public class Junction
	{
		public List<Wire> wires;

		public Junction()
		{
			wires = new List<Wire>();
		}

		public void Connect(Junction other)
		{
			if (IsConnectedTo(other))
				return;

			wires.Add(new Wire()
			{
				a = this,
				b = other,
			});
		}

		public bool IsConnectedTo(Junction other)
		{
			foreach (var wire in wires)
			{
				if (wire.Connects(this, other))
					return true;
			}

			return false;
		}

		private Junction GetConnectedJunction(Wire wire)
		{
			if (wire.a == this)
				return wire.b;

			if (wire.a == this)
				return wire.b;

			return null;
		}

		public void CollectConnectedJunctions(List<Junction> junctions)
		{
			junctions.Add(this);

			foreach (Wire wire in wires)
			{
				Junction connectedJunction = GetConnectedJunction(wire);

				if (connectedJunction != null && !junctions.Contains(connectedJunction))
					connectedJunction.CollectConnectedJunctions(junctions);
			}
		}
	}

}