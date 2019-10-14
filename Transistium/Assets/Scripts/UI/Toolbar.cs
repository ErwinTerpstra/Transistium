using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Interaction;
using Transistium.Util;
using Transistium.Design;

namespace Transistium.UI
{
	public class Toolbar : MonoBehaviour
	{
		[SerializeField]
		private Button buttonAddTransistor = null;

		[SerializeField]
		private Button buttonAddPin = null;

		[SerializeField]
		private Button buttonCreateChip = null;

		[SerializeField]
		private Transform chipRoot = null;

		[SerializeField]
		private ChipButton chipButtonPrefab = null;

		private CircuitManager circuitManager;

		private Observer<Chip, ChipButton> chips;

		private void Start()
		{
			circuitManager = CircuitManager.Instance;

			chips = new Observer<Chip, ChipButton>(CreateChip, DestroyChip);
			chips.Observe(circuitManager.Project.chips);

			buttonAddTransistor.onClick.AddListener(OnAddTransistorClicked);
			buttonAddPin.onClick.AddListener(OnAddPinClicked);

			buttonCreateChip.onClick.AddListener(OnCreateChipClicked);
		}

		private void LateUpdate()
		{
			chips.DetectChanges();
		}

		private ChipButton CreateChip(Chip chip)
		{
			var chipButton = Instantiate(chipButtonPrefab, chipRoot, false);

			chipButton.Configure(chip.NameOrDefault);

			chipButton.AddClicked += OnChipAddClicked;
			chipButton.EditClicked += OnChipEditClicked;

			return chipButton;
		}

		private void DestroyChip(Chip chip, ChipButton chipButton)
		{
			Destroy(chipButton.gameObject);
		}


		private void OnAddTransistorClicked()
		{

		}

		private void OnAddPinClicked()
		{

		}

		private void OnCreateChipClicked()
		{
			var chip = circuitManager.Project.CreateChip(out _);
			circuitManager.SwitchChip(chip);
		}

		private void OnChipAddClicked(ChipButton button)
		{
		}

		private void OnChipEditClicked(ChipButton button)
		{
			var chip = chips.Mapping[button];
			circuitManager.SwitchChip(chip);
		}

	}

}