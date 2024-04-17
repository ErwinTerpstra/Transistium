using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Util;
using UnityEngine.UI;

namespace Transistium.Interaction
{
	public delegate void PinInstanceEvent(PinInstance pinInstance, PinInstanceBehaviour behaviour);

	public class ChipInstanceBehaviour : MonoBehaviour
	{
		public static readonly (int, int)[] FontSizes = 
		{
			(6, 18),
			(10, 16),
			(int.MaxValue, 14)
		};

		public event PinInstanceEvent PinInstanceCreated;
		public event PinInstanceEvent PinInstanceDestroyed;

		[SerializeField]
		private PinInstanceBehaviour pinInstancePrefab = null;

		[SerializeField]
		private LayoutGroup[] pinInstanceRoots = null;

		[SerializeField]
		private TMPro.TMP_Text label = null;

		[SerializeField]
		private int pinGroupPadding = 45;

		private Chip chip;

		private ChipInstance chipInstance;

		private Observer<PinInstance, PinInstanceBehaviour> pinInstances;

		public Chip Chip => chip;

		public ChipInstance ChipInstance => chipInstance;

		private void Awake()
		{
			pinInstances = new Observer<PinInstance, PinInstanceBehaviour>(CreatePinInstanceBehaviour, DestroyPinInstanceBehaviour);

			UpdatePinLayouts();
		}

		private void LateUpdate()
		{
			pinInstances.DetectChanges();

			label.text = chip.NameOrDefault;
			label.fontSize = DetermineFontSize(label.text.Length);
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
			PinInstanceBehaviour behaviour = Instantiate(pinInstancePrefab, root.transform, false);
			behaviour.Configure(pin, pinInstance);

			CircuitElementBehaviour elementBehaviour = behaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = pinInstance;

			UpdatePinLayouts();

			PinInstanceCreated?.Invoke(pinInstance, behaviour);

			return behaviour;
		}

		private void DestroyPinInstanceBehaviour(PinInstance pinInstance, PinInstanceBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);

			UpdatePinLayouts();

			PinInstanceDestroyed?.Invoke(pinInstance, behaviour);
		}

		private void UpdatePinLayouts()
		{
			UpdatePinLayout(pinInstanceRoots[(int) PinSide.TOP], true);
			UpdatePinLayout(pinInstanceRoots[(int) PinSide.BOTTOM], true);

			UpdatePinLayout(pinInstanceRoots[(int) PinSide.LEFT], false);
			UpdatePinLayout(pinInstanceRoots[(int) PinSide.RIGHT], false);
		}

		private void UpdatePinLayout(LayoutGroup group, bool horizontal)
		{
			RectOffset offset = group.padding;
			int padding = group.transform.childCount > 0 ? pinGroupPadding : 0;

			if (horizontal)
			{
				group.padding.left = padding;
				group.padding.right = padding;
			}
			else
			{
				group.padding.top = padding;
				group.padding.bottom = padding;
			}
		}

		private int DetermineFontSize(int textLength)
		{
			foreach ((int lengthTreshold, int fontSize) in FontSizes)
			{
				if (textLength <= lengthTreshold)
					return fontSize;
			}

			return 0;
		}
	}

}