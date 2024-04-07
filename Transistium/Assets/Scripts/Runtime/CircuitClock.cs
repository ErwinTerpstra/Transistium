namespace Transistium.Runtime
{
	public class CircuitClock
	{
		private float time;

		private long ticks;

		private int tickRate;

		public float Time => time;

		public long Ticks => ticks;

		public int TickRate
		{
			get => tickRate;
			set
			{
				tickRate = value;
			}
		}

		public CircuitClock(int tickRate)
		{
			this.tickRate = tickRate;
		}

		public CircuitTime Update(float deltaTime)
		{
			time += deltaTime;

			long targetTicks = (long)(time * tickRate);

			return new CircuitTime()
			{
				ticks = targetTicks,
				time = time,
				deltaTicks = targetTicks - ticks,
				deltaTime = deltaTime,
			};
		}

		public void NextTick()
		{
			++ticks;
		}
	}
}