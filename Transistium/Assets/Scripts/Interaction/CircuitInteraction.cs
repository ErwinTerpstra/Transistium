﻿using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;
using Transistium.Util;

namespace Transistium.Interaction
{
	public delegate void CircuitElementEvent(CircuitElement element);

	public class CircuitInteraction : MonoSingleton<CircuitInteraction>, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private static readonly Rotation[] rotations = { Rotation.NONE, Rotation.ROTATE_90, Rotation.ROTATE_180, Rotation.ROTATE_270 };

		public event CircuitElementEvent SelectionChanged;

		[SerializeField]
		private float gridSize = 0.2f;

		private CircuitManager circuitManager;

		private Circuit circuit;

		private CircuitElementBehaviour selectedElement;

		private CircuitElementBehaviour draggingElement;

		private Wire currentWire;

		protected override void Awake()
		{
			base.Awake();

			currentWire = null;
		}

		private void Start()
		{
			circuitManager = CircuitManager.Instance;
			circuitManager.ChipEnterred += OnChipEnterred;
			circuitManager.ChipLeft += OnChipLeft;

			circuit = circuitManager.CurrentChip.circuit;
		}

		private void Update()
		{
			if (selectedElement != null)
				HandleSelectedElementShortcuts();
		}


		private void HandleSelectedElementShortcuts()
		{
			if (Input.GetKey(KeyCode.Delete))
			{
				if (!selectedElement.Element.flags.Has(CircuitElementFlags.PERMANENT))
				{
					// Deduce what kind of element was selected
					var transistorBehaviour = selectedElement.GetComponent<TransistorBehaviour>();
					var junctionBehaviour = selectedElement.GetComponent<JunctionBehaviour>();
					var pinBehaviour = selectedElement.GetComponent<PinBehaviour>(); ;
					var chipInstanceBehaviour = selectedElement.GetComponent<ChipInstanceBehaviour>();

					// Remove from circuit accordingly
					if (transistorBehaviour != null)
						circuit.RemoveTransistor(transistorBehaviour.Transistor);
					else if (pinBehaviour != null)
						circuitManager.CurrentChip.RemovePin(pinBehaviour.Pin);
					else if (chipInstanceBehaviour != null)
						circuit.RemoveChipInstance(chipInstanceBehaviour.ChipInstance);
					else if (junctionBehaviour != null)
						circuit.RemoveJunction(junctionBehaviour.Junction);

					Select(null);
					return;
				}
			}

			if (Input.GetKey(KeyCode.Escape))
			{
				Select(null);
				return;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
				selectedElement.Element.transform.rotation = rotations[(Array.IndexOf(rotations, selectedElement.Element.transform.rotation) + 1) % rotations.Length];
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
				selectedElement.Element.transform.rotation = rotations[(rotations.Length + Array.IndexOf(rotations, selectedElement.Element.transform.rotation) - 1) % rotations.Length];

		}

		private void Select(CircuitElementBehaviour elementBehaviour)
		{
			if (selectedElement && selectedElement.SelectionIndicator)
				selectedElement.SelectionIndicator.SetActive(false);

			selectedElement = elementBehaviour;

			if (selectedElement && selectedElement.SelectionIndicator)
				selectedElement.SelectionIndicator.SetActive(true);

			SelectionChanged?.Invoke(elementBehaviour?.Element);
		}

		private Vector2 GetCircuitPosition(PointerEventData eventData)
		{
			return circuitManager.GetCircuitPosition(eventData.pointerCurrentRaycast.worldPosition);
		}

		private Vector2 SnapPosition(Vector2 position)
		{
			return new Vector2(Mathf.Round(position.x / gridSize) * gridSize, Mathf.Round(position.y / gridSize) * gridSize);
		}

		private void StartWire(PointerEventData eventData, JunctionBehaviour junctionBehaviour)
		{
			currentWire = circuit.AddWire(junctionBehaviour.Junction, out _);
			currentWire.vertices.Add(GetCircuitPosition(eventData));
		}

		private void ConnectWire(PointerEventData eventData, Junction junction)
		{
			currentWire.b = circuit.junctions.LookupHandle(junction);
			currentWire.vertices.Clear();

			junction.wires.Add(circuit.wires.LookupHandle(currentWire));

			currentWire = null;
		}

		private void UpdateWire(PointerEventData eventData)
		{
			currentWire.vertices[0] = GetCircuitPosition(eventData);
		}

		private void UpdateElement(PointerEventData eventData)
		{
			draggingElement.Element.transform.position = SnapPosition(GetCircuitPosition(eventData));
		}

		private void HandleLeftClick(PointerEventData eventData)
		{
			var wireBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<WireBehaviour>();

			if (wireBehaviour != null)
			{
				circuit.RemoveWire(wireBehaviour.Wire);
				return;
			}

			var elementBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CircuitElementBehaviour>();

			if (elementBehaviour != null)
			{
				var junctionBehaviour = elementBehaviour.GetComponent<JunctionBehaviour>();

				if (junctionBehaviour != null)
				{
					if (junctionBehaviour.Junction.flags.Has(CircuitElementFlags.EMBEDDED))
						elementBehaviour = junctionBehaviour.transform.parent.GetComponentInParent<CircuitElementBehaviour>();
				}

				Select(elementBehaviour);
				return;
			}

			Select(null);
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

						if (elementBehaviour != null)
						{
							var junctionBehaviour = elementBehaviour.GetComponent<JunctionBehaviour>();

							if (junctionBehaviour != null)
							{
								if (junctionBehaviour.Junction.flags.Has(CircuitElementFlags.EMBEDDED))
									elementBehaviour = junctionBehaviour.transform.parent.GetComponentInParent<CircuitElementBehaviour>();
							}

							if (elementBehaviour == selectedElement)
								draggingElement = selectedElement;
						}
					}

					break;
			}

		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (currentWire != null)
			{
				var junctionBehaviour = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<JunctionBehaviour>();

				if (junctionBehaviour != null)
				{
					if (!circuit.AreConnected(circuit.junctions[currentWire.a], junctionBehaviour.Junction))
						ConnectWire(eventData, junctionBehaviour.Junction);
					else
						circuit.RemoveWire(currentWire);
				}
				else
				{
					var junction = circuit.AddJunction(CircuitElementFlags.NONE, out _);
					junction.transform.position = SnapPosition(GetCircuitPosition(eventData));

					ConnectWire(eventData, junction);
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
			if (currentWire != null)
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


		private void OnChipEnterred(Chip chip)
		{
			circuit = chip.circuit;
		}

		private void OnChipLeft(Chip chip)
		{

		}

		public CircuitElement SelectedElement
		{
			get { return selectedElement?.Element; }
		}

	}

}