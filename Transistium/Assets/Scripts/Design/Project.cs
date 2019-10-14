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

		public Chip RootChip
		{
			get { return chips[rootChipHandle]; }
		}

	}
}
