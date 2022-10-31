using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public class Transistor : CircuitElement
	{
		public Handle<Junction> @base;

		public Handle<Junction> collector;

		public Handle<Junction> emitter;

		public Transistor()
		{
			@base = Handle<Junction>.Invalid;
			collector = Handle<Junction>.Invalid;
			emitter = Handle<Junction>.Invalid;
		}

		public void CollectJunctions(List<Handle<Junction>> junctions)
		{
			junctions.Add(@base);
			junctions.Add(collector);
			junctions.Add(emitter);
		}


	}

}