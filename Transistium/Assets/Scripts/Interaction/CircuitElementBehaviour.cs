using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitElementBehaviour : MonoBehaviour
	{
		[SerializeField]
		private bool locked = false;

		private CircuitElement element;

		private void LateUpdate()
		{
			if (!locked)
			{
				transform.localPosition = element.transform.position;
				transform.localRotation = Quaternion.Euler(0, 0, -((int)element.transform.rotation) * 90);
			}
		}

		public bool IsLocked => locked;

		public CircuitElement Element
		{
			get => element;
			set => element = value;
		}

	}
}
