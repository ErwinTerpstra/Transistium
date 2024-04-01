using System
	;
using Transistium.Design.Components;
using UnityEngine;

namespace Transistium.Interaction.Components
{
	public abstract class ComponentBehaviour : MonoBehaviour
	{
		protected virtual void Awake()
		{

		}

		public abstract void LoadState(ComponentData data);
		public abstract void StoreState(ComponentData data);
	}

	public abstract class ComponentBehaviour<DataType> : ComponentBehaviour
		where DataType : ComponentData
	{
		public override sealed void LoadState(ComponentData data)
		{
			LoadState(data as DataType);
		}

		public override sealed void StoreState(ComponentData data)
		{
			StoreState(data as DataType);
		}

		protected virtual void LoadState(DataType data) { }
		protected virtual void StoreState(DataType data) { }
	}
}