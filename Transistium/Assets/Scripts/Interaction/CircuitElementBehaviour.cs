using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitElementBehaviour : MonoBehaviour
	{
		private CircuitElement target;

		private void LateUpdate()
		{
			transform.localPosition = target.transform.position;
			transform.localRotation = Quaternion.Euler(0, 0, -((int)target.transform.rotation) * 90);
		}

		public CircuitElement Target
		{
			get => target;
			set => target = value;
		}

	}
}
