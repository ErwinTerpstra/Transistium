using System;
using System.Collections.Generic;
using Transistium.Design;
using Guid = Transistium.Guid;

namespace Transistium.Design.Components
{
	public class Button : Component
	{
		private const string ID = "Button";

		public static readonly Guid GUID = Guid.Hash(ID);
		public static readonly Guid PIN_OUT = Guid.Hash(ID + "_Out");

		public override Guid Guid => GUID;

		protected override void SetupChip(Chip chip)
		{
			chip.name = "Button";

			Pin outputPin = chip.AddPin(PIN_OUT, out _);
			outputPin.name = "OUT";
			outputPin.side = PinSide.RIGHT;
			outputPin.direction = PinDirection.OUTPUT;
		}
	}

}