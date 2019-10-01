using UnityEngine;
using UnityEngine.EventSystems;

namespace Transistium.Interaction
{
	public class CircuitInteraction : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
	{
		private WireBehaviour currentConnection;

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Right:

					break;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			var junction = eventData.selectedObject.GetComponent<JunctionBehaviour>();

			if (junction != null)
			{

			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{

		}
	}

}