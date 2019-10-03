using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Transistor : CircuitElement
	{
		public Handle gate;

		public Handle drain;

		public Handle source;

		public Transistor()
		{
			gate = Handle.Invalid;
			drain = Handle.Invalid;
			source = Handle.Invalid;
		}

		public void CollectJunctions(List<Handle> junctions)
		{
			junctions.Add(gate);
			junctions.Add(drain);
			junctions.Add(source);
		}


	}

}