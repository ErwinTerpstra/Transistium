using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Interaction;

namespace Transistium.UI
{
	public class PropertyEditor : MonoBehaviour
	{
		[SerializeField]
		private PropertySectionChip propertySectionChip = null;

		[SerializeField]
		private PropertySectionPin propertySectionPin = null;

		private CircuitManager circuitManager;

		private CircuitInteraction circuitInteraction;
		
		private void Start()
		{
			circuitManager = CircuitManager.Instance;
			circuitManager.ChipEnterred += OnChipEnterred;

			circuitInteraction = CircuitInteraction.Instance;
			circuitInteraction.SelectionChanged += OnSelectionChanged;

			OnChipEnterred(circuitManager.Chip);
			OnSelectionChanged(circuitInteraction.SelectedElement);
		}

		private void ShowSections(CircuitElement element)
		{
			if (element != null && element is Pin)
				propertySectionPin.Show(element as Pin);
			else
				propertySectionPin.Hide();
		}

		private void OnChipEnterred(Chip chip)
		{
			propertySectionChip.Show(chip);
		}

		private void OnSelectionChanged(CircuitElement element)
		{
			ShowSections(element);
		}

	}
}
