﻿using System;
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

			manager = FindObjectOfType<CircuitManager>();

			compiler = new CircuitCompiler();
			simulator = new CircuitSimulator();

			simulator.BeforeTick += OnBeforeTick;
		}

		private void Update()
		{
			if (state != ApplicationState.SIMULATING)
				return;

			float dt = Time.deltaTime;

			manager.StoreState(compilationResult.componentInstances);

			foreach (var componentInstance in compilationResult.componentInstances.All)
				componentInstance.Update(dt, simulator.CurrentState);

			simulator.Update(dt);

			manager.LoadState(simulator.CurrentState, compilationResult.symbols);
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
			manager.ClearState();

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
				componentInstance.WriteToState(nextState);
		}
	}

	public enum ApplicationState
	{ 
		DESIGNING,
		SIMULATING,
		PAUSED,
	}
}