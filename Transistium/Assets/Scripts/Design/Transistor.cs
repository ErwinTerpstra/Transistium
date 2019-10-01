
using System.Collections.Generic;

namespace Transistium.Design
{
	public class Transistor
	{
		public Junction gate;

		public Junction drain;

		public Junction source;

		public Transistor()
		{
			gate = new Junction();
			drain = new Junction();
			source = new Junction();
		}

		public void CollectJunctions(List<Junction> junctions)
		{
			junctions.Add(gate);
			junctions.Add(drain);
			junctions.Add(source);
		}


	}

}