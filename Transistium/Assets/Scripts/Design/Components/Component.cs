using System;
using Transistium.Runtime;

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

		public virtual void Update(float dt, ComponentData data, ChipStateAdapter adapter)
		{

		}

		protected abstract void SetupChip(Chip chip);

		public abstract ComponentData CreateDefaultData();
	}

	public abstract class Component<DataType> : Component
		where DataType : ComponentData, new()
	{
		public override ComponentData CreateDefaultData()
		{
			return new DataType();
		}

		public override sealed void Update(float dt, ComponentData data, ChipStateAdapter adapter)
		{
			base.Update(dt, data, adapter);

			Update(dt, data as DataType, adapter);
		}

		protected virtual void Update(float dt, DataType data, ChipStateAdapter adapter) { }
	}

	public class ComponentData
	{

	}
}