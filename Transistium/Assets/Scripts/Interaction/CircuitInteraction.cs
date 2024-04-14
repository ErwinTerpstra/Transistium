using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;
using Transistium.Util;
using Transistium.Interaction.Components;

namespace Transistium.Interaction
{
	public delegate void CircuitElementEvent(CircuitElement element);

	public class CircuitInteraction : MonoSingleton<CircuitInteraction>, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		private static readonly Rotation[] rotations = { Rotation.NONE, Rotation.ROTATE_90, Rotation.ROTATE_180, Rotation.ROTATE_270 };

		public event CircuitElementEvent SelectionChanged;

		[SerializeField]
		private float gridSize = 0.2f;

		[SerializeField]
		private float zoomSpeed = 10.0f;

		[SerializeField]
		private float defaultZoomSize = 10.0f;

		[SerializeField]
		private float minZoomSize = 1.0f;

		[SerializeField]
		private float maxZoomSize = 20.0f;

		private Camera worldCamera;

		private TransistiumApplication application;

		private CircuitManager circuitManager;

		private Circuit circuit;

		private CircuitElementBehaviour selectedElement;

		private CircuitElementBehaviour draggingElement;

		private Wire currentWire;

		private Vector2 cameraStartPosition;

		public CircuitElement SelectedElement => selectedElement?.Element;

		protected override void Awake()
		{
			base.Awake();

			currentWire = null;

			worldCamera = Camera.main;

			application = FindObjectOfType<TransistiumApplication>();
			circuitManager = FindObjectOfType<CircuitManager>();
		}

		private void Start()
		{
			circuitManager.ChipEnterred += OnChipEnterred;
			circuitManager.ChipLeft += OnChipLeft;

			circuit = circuitManager.CurrentCircuit;
		}

		private void Update()
		{
			if (selectedElement != null && application.State == ApplicationState.DESIGNING)
				HandleSelectedElementShortcuts();

			HandleZoom();
		}

		private void SetCameraPosition(Vector2 position)
		{
			worldCamera.transform.position = new Vector3(position.x, position.y, worldCamera.transform.position.z);
		}

		private void SetCameraZoom(float size)
		{
			worldCamera.orthographicSize = Mathf.Clamp(size, minZoomSize, maxZoomSize);
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

		private void HandleZoom()
		{
			float size = worldCamera.orthographicSize;
			size -= Input.mouseScrollDelta.y * zoomSpeed;

			SetCameraZoom(size);
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
			var target = eventData.pointerCurrentRaycast.gameObject;

			var elementBehaviour = target.GetComponentInParent<CircuitElementBehaviour>();

			if (elementBehaviour != null)
			{
				var junctionBehaviour = elementBehaviour.GetComponent<JunctionBehaviour>();

				if (junctionBehaviour != null)
				{
					if (junctionBehaviour.Junction.flags.Has(CircuitElementFlags.EMBEDDED))
						elementBehaviour = junctionBehaviour.transform.parent.GetComponentInParent<CircuitElementBehaviour>();
				}

				Select(elementBehaviour);
			}
			else
			{
				Select(null);

				// Check if we clicked a wire
				var wireBehaviours = GetComponentsInChildren<WireBehaviour>();

				foreach (var wireBehaviour in wireBehaviours)
				{
					if (wireBehaviour.OverlapsPoint(eventData.pointerCurrentRaycast.worldPosition))
					{
						circuit.RemoveWire(wireBehaviour.Wire);
						return;
					}
				}
			}
		}

		private void HandleDoubleClick(PointerEventData eventData)
		{
			var target = eventData.pointerCurrentRaycast.gameObject;
			var chipInstanceBehaviour = target.GetComponentInParent<ChipInstanceBehaviour>();
			var componentBehaviour = target.GetComponentInParent<ComponentBehaviour>();

			if (chipInstanceBehaviour && !componentBehaviour)
				circuitManager.SwitchChip(chipInstanceBehaviour.ChipInstance);
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
					if (eventData.clickCount == 1)
						HandleLeftClick(eventData);
					else if (eventData.clickCount == 2)
						HandleDoubleClick(eventData);
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
					if (application.State != ApplicationState.DESIGNING)
						break;

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

				case PointerEventData.InputButton.Right: 
					cameraStartPosition = worldCamera.transform.position;
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
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					if (currentWire != null)
					{
						UpdateWire(eventData);
						break;
					}

					if (draggingElement != null)
					{
						UpdateElement(eventData);
						break;
					}

					break;

				case PointerEventData.InputButton.Right:
					Vector2 currentPosition = worldCamera.ScreenToWorldPoint(eventData.pointerCurrentRaycast.screenPosition);
					Vector2 startPosition = worldCamera.ScreenToWorldPoint(eventData.pointerPressRaycast.screenPosition);

					Vector2 pointerDelta = currentPosition - startPosition;

					SetCameraPosition(cameraStartPosition - pointerDelta);

					break;
			}
		}


		private void OnChipEnterred(Chip chip)
		{
			circuit = chip.circuit;

			SetCameraPosition(Vector2.zero);
			SetCameraZoom(defaultZoomSize);
		}

		private void OnChipLeft(Chip chip)
		{

		}


	}

}