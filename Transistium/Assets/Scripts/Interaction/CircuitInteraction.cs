using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitInteraction : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private static readonly Rotation[] rotations = { Rotation.NONE, Rotation.ROTATE_90, Rotation.ROTATE_180, Rotation.ROTATE_270 };

		private CircuitManager circuitManager;

		private Circuit circuit;

		private CircuitElementBehaviour currentElement;

		private WireBehaviour currentWire;

		private void Start()
		{
			circuitManager = CircuitManager.Instance;
			circuit = circuitManager.Circuit;
		}

		private Vector2 GetCircuitPosition(PointerEventData eventData)
		{
			return circuitManager.GetCircuitPosition(eventData.pointerCurrentRaycast.worldPosition);
		}
		
		private void StartWire(PointerEventData eventData, JunctionBehaviour junctionBehaviour)
		{
			var wireHandle = circuit.AddWire(junctionBehaviour.JunctionHandle);

			currentWire = circuitManager.MapWire(wireHandle);

			var wire = circuit.GetWire(currentWire.WireHandle);
			wire.vertices.Add(GetCircuitPosition(eventData));
		}

		private void EndWire(PointerEventData eventData, JunctionBehaviour junctionBehaviour)
		{
			var wire = circuit.GetWire(currentWire.WireHandle);
			wire.b = junctionBehaviour.JunctionHandle;
			wire.vertices.Clear();

			var junction = circuit.GetJunction(junctionBehaviour.JunctionHandle);
			junction.wires.Add(currentWire.WireHandle);
			
			currentWire = null;
		}

		private void UpdateWire(PointerEventData eventData)
		{
			var wire = circuit.GetWire(currentWire.WireHandle);
			wire.vertices[0] = GetCircuitPosition(eventData);
		}

		private void UpdateElement(PointerEventData eventData)
		{
			currentElement.Element.transform.position = GetCircuitPosition(eventData);
		}

		private void HandleLeftClick(PointerEventData eventData)
		{
			var wireBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<WireBehaviour>();

			if (wireBehaviour != null)
			{
				circuitManager.RemoveWire(wireBehaviour.WireHandle);
				return;
			}

			var circuitElement = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

			if (circuitElement != null)
			{
				circuitElement.Element.transform.rotation = rotations[(Array.IndexOf(rotations, circuitElement.Element.transform.rotation) + 1) % rotations.Length];
				return;
			}
		}

		private void HandleRightClick(PointerEventData eventData)
		{
			var transistorHandle = circuit.AddTransistor();
			var transistor = circuit.GetTransistor(transistorHandle);

			circuitManager.MapTransistor(transistorHandle);

			transistor.transform.position = GetCircuitPosition(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!eventData.eligibleForClick || Vector2.Distance(eventData.position, eventData.pressPosition) >= EventSystem.current.pixelDragThreshold)
				return;

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
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					var elementBehaviour = eventData.pointerPressRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

					if (elementBehaviour != null && !elementBehaviour.IsLocked)
						currentElement = elementBehaviour;

					break;

				case PointerEventData.InputButton.Right:
					var junctionBehaviour = eventData.pointerPressRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

					if (junctionBehaviour != null)
						StartWire(eventData, junctionBehaviour);

					break;
			}
			
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (currentWire != null)
			{
				var junctionBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();
				var wire = circuit.GetWire(currentWire.WireHandle);

				if (junctionBehaviour != null)
				{
					if (!circuit.AreConnected(wire.a, junctionBehaviour.JunctionHandle))
						EndWire(eventData, junctionBehaviour);
					else
						Destroy(currentWire.gameObject);
				}
				else
				{
					var junctionHandle = circuit.AddJunction();
					var junction = circuit.GetJunction(junctionHandle);

					junction.transform.position = GetCircuitPosition(eventData);

					junctionBehaviour = circuitManager.MapJunction(junctionHandle);

					EndWire(eventData, junctionBehaviour);
				}
			}
			else if (currentElement != null)
				currentElement = null;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (currentWire != null)
				UpdateWire(eventData);
			else if (currentElement != null)
				UpdateElement(eventData);
		}
	}

}