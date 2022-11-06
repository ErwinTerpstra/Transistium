using System.Collections;
using Transistium.Runtime;
using UnityEngine;

namespace Transistium.Design.Components
{
	public class ComponentInstance
	{
		private readonly ChipInstancePath path;

		private readonly Component component;

		private readonly ComponentData data;

		private readonly ChipStateAdapter stateAdapter;

		public ChipInstancePath Path => path;

		public ComponentData Data => data;

		public ComponentInstance(ChipInstancePath path, ChipMapping chipMapping, Component component)
		{
			this.path = path;
			this.component = component;

			data = component.CreateDefaultData();

			stateAdapter = new ChipStateAdapter(component.Chip, chipMapping);
		}

		public void Update(float dt, CircuitState state)
		{
			stateAdapter.Reset(state);

			component.Update(dt, data, stateAdapter);
		}

		public void WriteToState(CircuitState state)
		{
			stateAdapter.WriteToState(state);
		}
	}
}