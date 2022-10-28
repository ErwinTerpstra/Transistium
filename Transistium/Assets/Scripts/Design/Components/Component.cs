using System;

namespace Transistium.Design.Components
{
	public abstract class Component
	{
		private readonly Chip chip;

		public Chip Chip => chip;

		public abstract Guid Guid { get; }

		public Component()
		{
			chip = Chip.Create();

			SetupChip(chip);
		}

		protected abstract void SetupChip(Chip chip);
	}
}