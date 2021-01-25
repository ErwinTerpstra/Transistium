using System.Collections.Generic;

namespace Transistium.Design
{
	public class Project
	{
		public HandleList<Chip> chips;

		public Handle<Chip> rootChipHandle;

		public Chip RootChip => chips[rootChipHandle]; 

		public Project()
		{
			chips = new HandleList<Chip>();
		}

		public Chip CreateChip(out Handle<Chip> handle)
		{
			var chip = Chip.Create();

			handle = chips.Add(chip);

			return chip;
		}

		public bool DetectCircularReferences(Chip haystack, Chip needle)
		{
			if (haystack == needle)
				return true;

			foreach (var instance in haystack.circuit.chipInstances)
			{
				var chip = chips[instance.chipHandle];

				if (DetectCircularReferences(chip, needle))
					return true;
			}

			return false;
		}

		public void UpdateChipInstances()
		{

		}

		public void UpdateChipInstance()
		{

		}

		public static Project Create()
		{
			var project = new Project();

			project.CreateChip(out project.rootChipHandle);

			return project;
		}
	}
}
