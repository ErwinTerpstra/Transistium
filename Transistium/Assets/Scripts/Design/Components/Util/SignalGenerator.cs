using System;
using System.Collections.Generic;
using Transistium.Design;
using Transistium.Runtime;
using Guid = Transistium.Guid;

namespace Transistium.Design.Components
{
	public class SignalGenerator : Component<SignalGenerator.Data>
	{
		private const string ID = "SignalGenerator";

		public static readonly Guid GUID = Guid.Hash(ID);
		public static readonly Guid PIN_OUT = Guid.Hash(ID + "_Out");

		public override Guid Guid => GUID;

		public override ComponentData CreateDefaultData()
		{
			return new Data()
			{
				currentOutput = Signal.LOW,
				halfPeriod = 50,
				ticksLeft = 0
			};
		}

		protected override void SetupChip(Chip chip)
		{
			chip.name = "Signal generator";

			Pin outputPin = chip.AddPin(PIN_OUT, out _);
			outputPin.name = "OUT";
			outputPin.side = PinSide.RIGHT;
			outputPin.direction = PinDirection.OUTPUT;
		}

		protected override void Update(ref CircuitTime clock, Data data, ChipStateAdapter adapter)
		{
			base.Update(ref clock, data, adapter);

			long ticksElapsed = clock.deltaTicks;

			while (ticksElapsed > 0)
			{
				long t = Math.Min(ticksElapsed, data.ticksLeft);
				data.ticksLeft -= t;
				ticksElapsed -= t;

				if (data.ticksLeft == 0)
				{
					data.currentOutput = data.currentOutput.Invert();
					data.ticksLeft = data.halfPeriod;
				}	
			}

			adapter.Write(PIN_OUT, data.currentOutput);
		}

		public class Data : ComponentData
		{
			public Signal currentOutput;

			public long halfPeriod;

			public long ticksLeft;
		}
	}

}