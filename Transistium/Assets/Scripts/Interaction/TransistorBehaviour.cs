using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class TransistorBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour gate = null;

		[SerializeField]
		private JunctionBehaviour drain = null;

		[SerializeField]
		private JunctionBehaviour source = null;

		private Handle transitorHandle;

		private void LateUpdate()
		{
			UpdateJunction(gate);
			UpdateJunction(drain);
			UpdateJunction(source);
		}

		private void UpdateJunction(JunctionBehaviour junctionBehaviour)
		{
			var junction = CircuitManager.Instance.Circuit.GetJunction(junctionBehaviour.JunctionHandle);
			junction.transform.position = CircuitManager.Instance.GetCircuitPosition(junctionBehaviour.transform.position);
		}

		public Handle TransistorHandle
		{
			get { return transitorHandle; }
			set { transitorHandle = value; }
		}

		public JunctionBehaviour Gate => gate;
		public JunctionBehaviour Drain => drain;
		public JunctionBehaviour Source => source;
	}
}
