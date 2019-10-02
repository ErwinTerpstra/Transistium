using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitInteraction : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private static readonly Rotation[] rotations = { Rotation.NONE, Rotation.ROTATE_90, Rotation.ROTATE_180, Rotation.ROTATE_270 };

		private CircuitElementBehaviour currentElement;

		private WireBehaviour currentConnection;

		private Vector2 GetCircuitPosition(PointerEventData eventData)
		{
			return CircuitManager.Instance.GetCircuitPosition(eventData.pointerCurrentRaycast.worldPosition);
		}

		private void StartConnection(PointerEventData eventData, JunctionBehaviour junction)
		{
			currentConnection = CircuitManager.Instance.CreateTemporaryWire(junction.Target);
			currentConnection.Target.vertices.Add(GetCircuitPosition(eventData));
		}

		private void EndConnection(PointerEventData eventData, JunctionBehaviour junction)
		{
			currentConnection.Target.b = junction.Target;
			currentConnection.Target.vertices.Clear();

			CircuitManager.Instance.StoreTemporaryWire(currentConnection);

			currentConnection = null;
		}

		private void UpdateConnection(PointerEventData eventData)
		{
			currentConnection.Target.vertices[0] = GetCircuitPosition(eventData);
		}

		private void UpdateElement(PointerEventData eventData)
		{
			currentElement.transform.position = GetCircuitPosition(eventData);
		}

		private void HandleLeftClick(PointerEventData eventData)
		{

		}

		private void HandleRightClick(PointerEventData eventData)
		{
			var circuitElement = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

			if (circuitElement != null)
			{
				circuitElement.Target.transform.rotation = rotations[(Array.IndexOf(rotations, circuitElement.Target.transform.rotation) + 1) % rotations.Length];
			}
			else
			{
				var transistor = CircuitManager.Instance.AddTransistor();
				transistor.transform.position = GetCircuitPosition(eventData);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					HandleLeftClick(eventData);
					break;

				case PointerEventData.InputButton.Right:
					HandleRightClick(eventData);
					break;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			var junction = eventData.pointerPressRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

			if (junction != null)
			{
				StartConnection(eventData, junction);
				return;
			}

			var element = eventData.pointerPressRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

			if (element != null)
			{
				currentElement = element;
				return;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (currentConnection != null)
			{
				var junction = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

				if (junction != null)
					EndConnection(eventData, junction);
				else
					Destroy(currentConnection.gameObject);
			}
			else if (currentElement != null)
				currentElement = null;
		}
		public void OnDrag(PointerEventData eventData)
		{
			if (currentConnection != null)
				UpdateConnection(eventData);
			else if (currentElement != null)
				UpdateElement(eventData);
		}
	}

}