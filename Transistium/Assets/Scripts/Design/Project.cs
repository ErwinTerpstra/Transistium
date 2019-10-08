using System.Collections.Generic;

namespace Transistium.Design
{
	public class Project
	{
		public Circuit rootCircuit;

		public List<Chip> chips;

		public Project()
		{
			rootCircuit = new Circuit();
			chips = new List<Chip>();
		}

		public Chip FindChip(string guid)
		{
			foreach (Chip chip in chips)
			{
				if (chip.guid == guid)
					return chip;
			}

			return null;
		}
	}
}
