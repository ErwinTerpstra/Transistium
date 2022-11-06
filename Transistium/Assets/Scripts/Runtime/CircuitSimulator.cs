using System;

using UnityEngine;

namespace Transistium.Runtime
{
	public class CircuitSimulator
	{
		public delegate void CircuitStateEvent(CircuitState currentState, CircuitState nextState);

		public event CircuitStateEvent BeforeTick;
		public event CircuitStateEvent AfterTick;

		private Circuit circuit;

		private CircuitState currentState;

		private CircuitState previousState;

		private int tickRate;

		private float simulationTime;

		private long simulationTicks;

		public Circuit Circuit => circuit;

		public CircuitState CurrentState => currentState;

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

			simulationTicks = 0;
			simulationTime = 0;
		}

		public void Update(float deltaTime)
		{
			simulationTime += deltaTime;

			long targetTicks = (long)(simulationTime * tickRate);
			while (simulationTicks < targetTicks)
				Tick();
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