using System.Collections.Generic;

namespace Transistium.Design
{
	public class Circuit
	{
		public List<Transistor> transistors;

		public Junction Vcc;

		public Junction Ground;
		
		public Circuit()
		{
			transistors = new List<Transistor>();

			Vcc = new Junction();
			Ground = new Junction();
		}
	}

}