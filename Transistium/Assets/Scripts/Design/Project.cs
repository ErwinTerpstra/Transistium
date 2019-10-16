using System.Collections.Generic;

namespace Transistium.Design
{
	public class Project
	{
		public HandleList<Chip> chips;

		public Handle<Chip> rootChipHandle;
		
		public Project()
		{
			chips = new HandleList<Chip>();

			CreateChip(out rootChipHandle);
		}

		public Chip CreateChip(out Handle<Chip> handle)
		{
			var chip = new Chip();

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

		public Chip RootChip
		{
			get { return chips[rootChipHandle]; }
		}

	}
}
