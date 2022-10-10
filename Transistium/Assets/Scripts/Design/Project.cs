using System.Linq;
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
			foreach (var chip in chips)
				UpdateChipInstances(chip);
		}

		public void UpdateChipInstances(Chip parentChip)
		{
			foreach (var chipInstance in parentChip.circuit.chipInstances)
				UpdateChipInstance(parentChip, chipInstance);
		}

		public void UpdateChipInstance(Chip parentChip, ChipInstance chipInstance)
		{
			var childChip = chips[chipInstance.chipHandle];

			// Iterate all pins in the child chip
			foreach (var pin in childChip.pins)
			{
				var pinHandle = childChip.pins.LookupHandle(pin);

				// Check if this pin needs to be instantiated for a chip instance
				if (!childChip.ShouldInstantiatePin(pinHandle))
					continue;

				// Check if there already exists a pin instance for this
				if (!chipInstance.pins.Any(x => x.pinHandle == pinHandle))
					parentChip.circuit.InstantiatePin(pin, childChip, chipInstance);
			}

			// Iterate all pin instances currently present in the parent chip
			for (int i = 0; i < chipInstance.pins.Count; )
			{
				var pinInstance = chipInstance.pins[i];
				var pin = childChip.pins[pinInstance.pinHandle];

				pinInstance.transform.rotation = pin.side.ToRotation();

				// Check if this pin instance should be deleted
				if (!childChip.pins.Contains(pin))
					parentChip.circuit.RemovePinInstance(chipInstance, pinInstance);
				else
					++i;
			}
		}

		public static Project Create()
		{
			var project = new Project();

			var rootChip = project.CreateChip(out project.rootChipHandle);
			rootChip.name = "Root";

			return project;
		}
	}
}
