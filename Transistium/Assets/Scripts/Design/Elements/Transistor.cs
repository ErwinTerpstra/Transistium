using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Transistor : CircuitElement
	{
		public Handle<Junction> gate;

		public Handle<Junction> drain;

		public Handle<Junction> source;

		public Transistor()
		{
			gate = Handle<Junction>.Invalid;
			drain = Handle<Junction>.Invalid;
			source = Handle<Junction>.Invalid;
		}

		public void CollectJunctions(List<Handle<Junction>> junctions)
		{
			junctions.Add(gate);
			junctions.Add(drain);
			junctions.Add(source);
		}


	}

}