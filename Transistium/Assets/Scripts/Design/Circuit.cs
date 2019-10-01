using System.Collections.Generic;

namespace Transistium.Design
{
	public class Circuit
	{
		public List<Transistor> transistors;

		public Junction vcc;

		public Junction ground;
		
		public Circuit()
		{
			transistors = new List<Transistor>();

			vcc = new Junction();
			ground = new Junction();
		}
	}

}