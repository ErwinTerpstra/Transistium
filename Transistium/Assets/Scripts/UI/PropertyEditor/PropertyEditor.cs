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

		private TransistiumApplication application;

		private CircuitManager circuitManager;

		private CircuitInteraction circuitInteraction;

		private CanvasGroup canvasGroup;

		private void Start()
		{
			application = FindObjectOfType<TransistiumApplication>();
			application.StateChanged += OnApplicationStateChanged;

			circuitManager = FindObjectOfType<CircuitManager>();
			circuitManager.ChipEnterred += OnChipEnterred;

			circuitInteraction = FindObjectOfType<CircuitInteraction>();
			circuitInteraction.SelectionChanged += OnSelectionChanged;

			canvasGroup = GetComponent<CanvasGroup>();

			OnChipEnterred(circuitManager.CurrentChip);
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

		private void OnApplicationStateChanged(ApplicationState state)
		{
			canvasGroup.interactable = state == ApplicationState.DESIGNING;
		}

	}
}
