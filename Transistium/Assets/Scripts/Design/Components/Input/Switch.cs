using System;
using System.Collections.Generic;
using Transistium.Design;
using Transistium.Runtime;
using Guid = Transistium.Guid;

namespace Transistium.Design.Components
{
	public class Switch : Component<Switch.Data>
	{
		private const string ID = "Switch";

		public static readonly Guid GUID = Guid.Hash(ID);
		public static readonly Guid PIN_OUT = Guid.Hash(ID + "_Out");

		public override Guid Guid => GUID;

		protected override void SetupChip(Chip chip)
		{
			chip.name = "Switch";

			Pin outputPin = chip.AddPin(PIN_OUT, out _);
			outputPin.name = "OUT";
			outputPin.side = PinSide.RIGHT;
			outputPin.direction = PinDirection.OUTPUT;
		}

		protected override void Update(float dt, Data data, ChipStateAdapter adapter)
		{
			base.Update(dt, data, adapter);

			adapter.Write(PIN_OUT, data.active ? Signal.HIGH : Signal.LOW);
		}

		public class Data : ComponentData
		{
			public bool active;
		}
	}

}