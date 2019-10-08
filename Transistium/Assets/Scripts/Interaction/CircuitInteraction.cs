using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class CircuitInteraction : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private static readonly Rotation[] rotations = { Rotation.NONE, Rotation.ROTATE_90, Rotation.ROTATE_180, Rotation.ROTATE_270 };

		[SerializeField]
		private Transform selectionIndicator = null;

		private CircuitManager circuitManager;

		private Circuit circuit;

		private CircuitElementBehaviour selectedElement;

		private CircuitElementBehaviour draggingElement;

		private Handle currentWire;

		private void Start()
		{
			circuitManager = CircuitManager.Instance;
			circuitManager.CircuitEntered += OnCircuitEntered;
			circuitManager.CircuitLeft += OnCircuitLeft;

			circuit = circuitManager.Circuit;

			currentWire = Handle.Invalid;

			selectionIndicator.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (selectedElement != null)
			{
				selectionIndicator.position = circuitManager.GetWorldPosition(selectedElement.Element.transform.position);
				selectionIndicator.gameObject.SetActive(true);

				HandleSelectedElementShortcuts();
			}
			else
				selectionIndicator.gameObject.SetActive(false);

			if (Input.GetKeyDown(KeyCode.T))
			{
				var transistorHandle = circuit.AddTransistor();
				var transistor = circuit.GetTransistor(transistorHandle);

				transistor.transform.position = Vector2.zero;// GetCircuitPosition(eventData);
			}
		}


		private void HandleSelectedElementShortcuts()
		{
			if (Input.GetKey(KeyCode.Delete))
			{
				var transistorBehaviour = selectedElement.GetComponent<TransistorBehaviour>();
				var junctionBehaviour = selectedElement.GetComponent<JunctionBehaviour>();

				if (transistorBehaviour != null)
					circuit.RemoveTransistor(transistorBehaviour.TransistorHandle);
				else if (junctionBehaviour != null)
					circuit.RemoveJunction(junctionBehaviour.JunctionHandle);

				selectedElement = null;
				return;
			}

			if (Input.GetKey(KeyCode.Escape))
			{
				selectedElement = null;
				return;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
				selectedElement.Element.transform.rotation = rotations[(Array.IndexOf(rotations, selectedElement.Element.transform.rotation) + 1) % rotations.Length];
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
				selectedElement.Element.transform.rotation = rotations[(rotations.Length + Array.IndexOf(rotations, selectedElement.Element.transform.rotation) - 1) % rotations.Length];

		}

		private Vector2 GetCircuitPosition(PointerEventData eventData)
		{
			return circuitManager.GetCircuitPosition(eventData.pointerCurrentRaycast.worldPosition);
		}

		private void StartWire(PointerEventData eventData, JunctionBehaviour junctionBehaviour)
		{
			currentWire = circuit.AddWire(junctionBehaviour.JunctionHandle);

			var wire = circuit.GetWire(currentWire);
			wire.vertices.Add(GetCircuitPosition(eventData));
		}

		private void ConnectWire(PointerEventData eventData, Handle junctionHandle)
		{
			var wire = circuit.GetWire(currentWire);
			wire.b = junctionHandle;
			wire.vertices.Clear();

			var junction = circuit.GetJunction(junctionHandle);
			junction.wires.Add(currentWire);

			currentWire = Handle.Invalid;
		}

		private void UpdateWire(PointerEventData eventData)
		{
			var wire = circuit.GetWire(currentWire);
			wire.vertices[0] = GetCircuitPosition(eventData);
		}

		private void UpdateElement(PointerEventData eventData)
		{
			draggingElement.Element.transform.position = GetCircuitPosition(eventData);
		}

		private void HandleLeftClick(PointerEventData eventData)
		{
			var wireBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<WireBehaviour>();

			if (wireBehaviour != null)
			{
				circuit.RemoveWire(wireBehaviour.WireHandle);
				return;
			}

			var elementBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();
			var junctionBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

			if (junctionBehaviour != null)
			{
				var junction = circuit.GetJunction(junctionBehaviour.JunctionHandle);
				if (junction.embedded)
					elementBehaviour = junctionBehaviour.transform.parent.GetComponentInParent<CircuitElementBehaviour>();
			}

			if (elementBehaviour != null)
			{
				selectedElement = elementBehaviour;
				return;
			}

			selectedElement = null;
		}

		private void HandleRightClick(PointerEventData eventData)
		{

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
					if (selectedElement == null)
					{
						var junctionBehaviour = eventData.pointerPressRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

						if (junctionBehaviour != null)
						{
							StartWire(eventData, junctionBehaviour);
							break;
						}
					}
					else
					{
						var elementBehaviour = eventData.pointerPressRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

						if (elementBehaviour == selectedElement)
							draggingElement = selectedElement;
					}

					break;
			}

		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (currentWire != Handle.Invalid)
			{
				var junctionBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();
				var wire = circuit.GetWire(currentWire);

				if (junctionBehaviour != null)
				{
					if (!circuit.AreConnected(wire.a, junctionBehaviour.JunctionHandle))
						ConnectWire(eventData, junctionBehaviour.JunctionHandle);
					else
						circuit.RemoveWire(currentWire);
				}
				else
				{
					var junctionHandle = circuit.AddJunction(false);
					var junction = circuit.GetJunction(junctionHandle);

					junction.transform.position = GetCircuitPosition(eventData);

					ConnectWire(eventData, junctionHandle);
				}

				return;
			}

			if (draggingElement != null)
			{
				draggingElement = null;
				return;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (currentWire != Handle.Invalid)
			{
				UpdateWire(eventData);
				return;
			}

			if (draggingElement != null)
			{
				UpdateElement(eventData);
				return;
			}
		}

		private void OnCircuitLeft(Circuit circuit)
		{

		}

		private void OnCircuitEntered(Circuit circuit)
		{
			this.circuit = circuit;
		}

	}

}