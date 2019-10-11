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

		private Transistor transistor;

		private void LateUpdate()
		{
			UpdateJunction(gate);
			UpdateJunction(drain);
			UpdateJunction(source);
		}

		private void UpdateJunction(JunctionBehaviour junctionBehaviour)
		{
			junctionBehaviour.Junction.transform.position = CircuitManager.Instance.GetCircuitPosition(junctionBehaviour.transform.position);
		}

		public Transistor Transistor
		{
			get { return transistor; }
			set { transistor = value; }
		}

		public JunctionBehaviour Gate => gate;
		public JunctionBehaviour Drain => drain;
		public JunctionBehaviour Source => source;
	}
}
