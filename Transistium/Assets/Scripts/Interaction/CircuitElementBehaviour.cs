using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitElementBehaviour : MonoBehaviour
	{
		[SerializeField]
		private GameObject selectionIndicator = null;

		private CircuitElement element;

		private RectTransform rectTransform;

		public CircuitElement Element
		{
			get => element;
			set => element = value;
		}

		public GameObject SelectionIndicator => selectionIndicator;

		private void Awake()
		{
			rectTransform = transform as RectTransform;

			if (selectionIndicator)
				selectionIndicator.SetActive(false);
		}

		private void LateUpdate()
		{
			if (!element.flags.Has(CircuitElementFlags.STATIC))
				rectTransform.anchoredPosition = element.transform.position;

			rectTransform.localRotation = Quaternion.Euler(0, 0, -((int)element.transform.rotation) * 90);
		}

	}
}
