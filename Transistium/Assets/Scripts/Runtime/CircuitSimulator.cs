using System;

using UnityEngine;

namespace Transistium.Runtime
{
	public class CircuitSimulator
	{
		public delegate void CircuitStateEvent(CircuitState currentState, CircuitState nextState);

		public const int METRICS_PERIOD = 100;

		public event CircuitStateEvent BeforeTick;
		public event CircuitStateEvent AfterTick;

		private Circuit circuit;

		private CircuitState currentState;

		private CircuitState previousState;

		private CircuitMetrics currentMetrics;

		private CircuitMetrics nextMetrics;

		private int tickRate;

		private float simulationTime;

		private long simulationTicks;

		public Circuit Circuit => circuit;

		public CircuitState CurrentState => currentState;

		public CircuitMetrics Metrics => currentMetrics;

		public int TickRate => tickRate;

		public CircuitSimulator()
		{

		}

		public void Prepare(Circuit circuit, int tickRate)
		{
			this.circuit = circuit;
			this.tickRate = tickRate;

			currentState = new CircuitState(circuit);
			previousState = new CircuitState(circuit);
			
			currentMetrics = new CircuitMetrics(circuit);
			nextMetrics = new CircuitMetrics(circuit);

			simulationTicks = 0;
			simulationTime = 0;
		}

		public void Update(float deltaTime)
		{
			simulationTime += deltaTime;

			long targetTicks = (long)(simulationTime * tickRate);
			while (simulationTicks < targetTicks)
			{
				Tick();

				if (simulationTicks % METRICS_PERIOD == 0)
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

			++simulationTicks;
		}

	}
}