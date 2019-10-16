using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitElementBehaviour : MonoBehaviour
	{
		private CircuitElement element;

		private void LateUpdate()
		{
			transform.localPosition = element.transform.position;
			transform.localRotation = Quaternion.Euler(0, 0, -((int)element.transform.rotation) * 90);
		}

		public CircuitElement Element
		{
			get => element;
			set => element = value;
		}

	}
}
