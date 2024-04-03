using System;
using System.Collections;
using Transistium.Design;
using Transistium.Runtime;
using Transistium.Util;
using UnityEngine;

namespace Transistium.Interaction
{
	public class TransistiumApplication : MonoSingleton<TransistiumApplication>
	{
		public delegate void ApplicationStateEvent(ApplicationState state);

		public event ApplicationStateEvent StateChanged;

		[SerializeField]
		private int tickRate = 1000;

		private CircuitManager manager;

		private CircuitCompiler compiler;

		private CircuitSimulator simulator;

		private CompilationResult compilationResult;

		private ApplicationState state;

		public ApplicationState State => state;

		protected override void Awake()
		{
			base.Awake();

			Application.targetFrameRate = 60;

			manager = FindObjectOfType<CircuitManager>();

			compiler = new CircuitCompiler();
			simulator = new CircuitSimulator();

			simulator.BeforeTick += OnBeforeTick;

			manager.ChipEnterred += OnChipEnterred;
		}

		private void Update()
		{
			if (state == ApplicationState.SIMULATING)
			{
				float dt = Time.deltaTime;

				manager.StoreComponentState(compilationResult.componentInstances);

				foreach (var componentInstance in compilationResult.componentInstances.All)
					componentInstance.Update(dt, simulator.CurrentState);

				simulator.Update(dt);
			}

			if (state != ApplicationState.DESIGNING)
			{
				manager.LoadCircuitState(
					simulator.CurrentState,
					simulator.Metrics,
					compilationResult.symbols
				);
			}
		}

		public void Play()
		{
			if (state == ApplicationState.DESIGNING)
			{
				// Compile the circuit
				compilationResult = compiler.Compile(manager.Project);

				// Setup the simulator
				simulator.Prepare(compilationResult.circuit, tickRate);
			}

			SetState(ApplicationState.SIMULATING);
		}

		public void Pause()
		{
			SetState(ApplicationState.PAUSED);
		}

		public void Stop()
		{
			manager.ClearCircuitState();

			SetState(ApplicationState.DESIGNING);
		}

		public void Tick()
		{
			simulator.Tick();
		}

		private void SetState(ApplicationState state)
		{
			this.state = state;

			StateChanged?.Invoke(state);
		}

		private void OnBeforeTick(CircuitState currentState, CircuitState nextState)
		{
			foreach (var componentInstance in compilationResult.componentInstances.All)
			{
				componentInstance.WriteToState(currentState);
				componentInstance.WriteToState(nextState);
			}
		}
		private void OnChipEnterred(Chip chip)
		{
			if (compilationResult != null)
				manager.LoadComponentState(compilationResult.componentInstances);
		}
	}

	public enum ApplicationState
	{ 
		DESIGNING,
		SIMULATING,
		PAUSED,
	}
}