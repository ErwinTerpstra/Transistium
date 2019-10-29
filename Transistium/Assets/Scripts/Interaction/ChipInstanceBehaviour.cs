using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Util;

namespace Transistium.Interaction
{
	public delegate void PinInstanceEvent(PinInstance pinInstance, PinInstanceBehaviour behaviour);

	public class ChipInstanceBehaviour : MonoBehaviour
	{
		public event PinInstanceEvent PinInstanceCreated;
		public event PinInstanceEvent PinInstanceDestroyed;

		[SerializeField]
		private PinInstanceBehaviour pinInstancePrefab = null;

		[SerializeField]
		private Transform pinInstanceRoot = null;

		[SerializeField]
		private float pinSpacing = 1.0f;

		[SerializeField]
		private float pinOffset = 1.0f;

		[SerializeField]
		private TMPro.TMP_Text label = null;

		private Chip chip;

		private ChipInstance chipInstance;

		private Observer<PinInstance, PinInstanceBehaviour> pinInstances;

		private Dictionary<PinSide, int> pinTotalCount;
		private Dictionary<PinSide, int> pinActiveCount;

		private void Awake()
		{
			pinInstances = new Observer<PinInstance, PinInstanceBehaviour>(CreatePinInstanceBehaviour, DestroyPinInstanceBehaviour);
			pinTotalCount = new Dictionary<PinSide, int>();
			pinActiveCount = new Dictionary<PinSide, int>();
		}

		private void LateUpdate()
		{
			pinInstances.DetectChanges();

			PositionPins();

			label.text = chip.NameOrDefault;
		}

		public void Configure(Chip chip, ChipInstance chipInstance)
		{
			this.chip = chip;
			this.chipInstance = chipInstance;

			pinInstances.Observe(chipInstance.pins);
		}

		private void PositionPins()
		{
			foreach (PinSide side in Enum.GetValues(typeof(PinSide)))
			{
				pinTotalCount[side] = 0;
				pinActiveCount[side] = 0;
			}

			foreach (var pair in pinInstances.Mapping)
				++pinTotalCount[pair.Value.Pin.side];

			foreach (var pair in pinInstances.Mapping)
			{
				var pinInstance = pair.Key;

				PinSide side = pair.Value.Pin.side;

				// Calculate the base offset and direction based on the side
				Vector2 offset;
				Vector2 direction;
				SideToOffsetAndDirection(side, out offset, out direction);

				// Calculate the distance between the first pin
				float distance = (pinActiveCount[side] * pinSpacing) - ((pinTotalCount[side] - 1) / 2.0f * pinSpacing);

				pinInstance.transform.position = (offset * pinOffset) + (direction * distance);
				pinInstance.transform.rotation = side.ToRotation();

				++pinActiveCount[side];
			}
		}

		private void SideToOffsetAndDirection(PinSide side, out Vector2 offset, out Vector2 direction)
		{
			switch (side)
			{
				case PinSide.TOP:
					offset = new Vector2(0, 1);
					direction = new Vector2(1, 0);
					break;

				case PinSide.BOTTOM:
					offset = new Vector2(0, -1);
					direction = new Vector2(1, 0);
					break;

				case PinSide.LEFT:
					offset = new Vector2(-1, 0);
					direction = new Vector2(0, 1);
					break;

				case PinSide.RIGHT:
					offset = new Vector2(1, 0);
					direction = new Vector2(0, 1);
					break;

				default:
					offset = Vector2.zero;
					direction = Vector2.zero;
					break;
			}
		}
		
		private PinInstanceBehaviour CreatePinInstanceBehaviour(PinInstance pinInstance)
		{
			var pin = chip.pins[pinInstance.pinHandle];

			// Setup pin instance
			PinInstanceBehaviour behaviour = Instantiate(pinInstancePrefab, pinInstanceRoot, false);
			behaviour.Configure(pin, pinInstance);

			CircuitElementBehaviour elementBehaviour = behaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = pinInstance;

			PinInstanceCreated?.Invoke(pinInstance, behaviour);

			return behaviour;
		}

		private void DestroyPinInstanceBehaviour(PinInstance pinInstance, PinInstanceBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);

			PinInstanceDestroyed?.Invoke(pinInstance, behaviour);
		}

		public Chip Chip => chip;

		public ChipInstance ChipInstance => chipInstance;
	}

}