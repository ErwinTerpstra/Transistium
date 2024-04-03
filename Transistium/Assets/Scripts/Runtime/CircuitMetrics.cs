
namespace Transistium.Runtime
{
	public class CircuitMetrics
	{
		private readonly Circuit circuit;

		private readonly WireMetrics[] wires;

		public CircuitMetrics(Circuit circuit)
		{
			this.circuit = circuit;

			wires = new WireMetrics[circuit.WireCount];
		}

		public void Reset()
		{
			for (int i = 0; i < wires.Length; ++i)
				wires[i] = default;
		}

		public void Record(CircuitState state)
		{
			for (int i = 0; i < wires.Length; ++i)
			{
				ref WireMetrics wireMetrics = ref wires[i];
				
				switch (state.wires[i])
				{
					case Signal.LOW:
						++wireMetrics.samplesLow;
						break;

					case Signal.HIGH:
						++wireMetrics.samplesHigh;
						break;

					case Signal.FLOATING:
						++wireMetrics.samplesFloating;
						break;
				}
			}
		}

		public WireMetrics GetWireMetrics(int wireIndex) => wires[wireIndex];
	}

	public struct WireMetrics
	{
		public int samplesLow;

		public int samplesHigh;

		public int samplesFloating;

		public int SampleCount => samplesLow + samplesHigh + samplesFloating;

		public float DutyCycle
		{
			get
			{
				int sampleCount = SampleCount;
				if (sampleCount > 0)
					return samplesHigh / (float) sampleCount;
				else
					return 0.0f;
			}
		}
	}
}