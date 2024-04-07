using System;

namespace Transistium.Runtime
{
	public class CircuitSimulator
	{
		public delegate void CircuitUpdateEvent(CircuitState currentState, CircuitTime time);
		public delegate void CircuitStateEvent(CircuitState currentState, CircuitState nextState);

		public const int METRICS_PERIOD = 100;

		public event CircuitUpdateEvent BeforeUpdate;
		public event CircuitUpdateEvent AfterUpdate;

		public event CircuitStateEvent BeforeTick;
		public event CircuitStateEvent AfterTick;

		private Circuit circuit;

		private CircuitClock clock;

		private CircuitState currentState;

		private CircuitState previousState;

		private CircuitMetrics currentMetrics;

		private CircuitMetrics nextMetrics;

		public Circuit Circuit => circuit;

		public CircuitState CurrentState => currentState;

		public CircuitMetrics Metrics => currentMetrics;

		public int TickRate => clock.TickRate;

		public CircuitSimulator()
		{

		}

		public void Prepare(Circuit circuit, int tickRate)
		{
			this.circuit = circuit;

			clock = new CircuitClock(tickRate);

			currentState = new CircuitState(circuit);
			previousState = new CircuitState(circuit);
			
			currentMetrics = new CircuitMetrics(circuit);
			nextMetrics = new CircuitMetrics(circuit);
		}

		public void Update(float deltaTime)
		{
			CircuitTime time = clock.Update(deltaTime);

			BeforeUpdate?.Invoke(currentState, time);

			for (long t = 0; t < time.deltaTicks; ++t)
			{
				Tick();

				if (clock.Ticks % METRICS_PERIOD == 0)
				{
					// Swap metrics buffers
					var tmp = currentMetrics;
					currentMetrics = nextMetrics;
					nextMetrics = tmp;

					// Reset new metrics buffer
					nextMetrics.Reset();
				}

				// Record the state in the metrics buffer
				nextMetrics.Record(currentState);
			}

			AfterUpdate?.Invoke(currentState, time);
		}

		public void Tick()
		{
			if (circuit == null)
				return;

			// Prepare our "previous state" to be overwritten as the next state
			CircuitState nextState = previousState;
			nextState.Reset();

			BeforeTick?.Invoke(currentState, nextState);

			circuit.Tick(currentState, nextState);

			AfterTick?.Invoke(currentState, nextState);

			// Swap states
			previousState = currentState;
			currentState = nextState;

			clock.NextTick();
		}

	}
}