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
		private Transform[] pinInstanceRoots = null;

		[SerializeField]
		private TMPro.TMP_Text label = null;

		private Chip chip;

		private ChipInstance chipInstance;

		private Observer<PinInstance, PinInstanceBehaviour> pinInstances;

		public Chip Chip => chip;

		public ChipInstance ChipInstance => chipInstance;

		private void Awake()
		{
			pinInstances = new Observer<PinInstance, PinInstanceBehaviour>(CreatePinInstanceBehaviour, DestroyPinInstanceBehaviour);
		}

		private void LateUpdate()
		{
			pinInstances.DetectChanges();

			label.text = chip.NameOrDefault;
		}

		public void Configure(Chip chip, ChipInstance chipInstance)
		{
			this.chip = chip;
			this.chipInstance = chipInstance;

			pinInstances.Observe(chipInstance.pins);
		}

		private PinInstanceBehaviour CreatePinInstanceBehaviour(PinInstance pinInstance)
		{
			var pin = chip.pins[pinInstance.pinHandle];
			var root = pinInstanceRoots[(int)pin.side];

			// Setup pin instance
			PinInstanceBehaviour behaviour = Instantiate(pinInstancePrefab, root, false);
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
	}

}