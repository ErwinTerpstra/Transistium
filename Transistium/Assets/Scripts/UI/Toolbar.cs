using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Interaction;
using Transistium.Util;
using Transistium.Design;
using Random = UnityEngine.Random;

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
		private Button buttonSaveProject = null;

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

			buttonSaveProject.onClick.AddListener(OnSaveProjectClicked);
		}

		private void LateUpdate()
		{
			chips.DetectChanges();
		}

		private ChipButton CreateChip(Chip chip)
		{
			var chipButton = Instantiate(chipButtonPrefab, chipRoot, false);

			chipButton.Configure(chip);

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
			circuitManager.CurrentChip.circuit.AddTransistor(out _);
		}

		private void OnAddPinClicked()
		{
			var pin = circuitManager.CurrentChip.AddPin(out _);
			pin.transform.position = new Vector2()
			{
				x = Random.Range(-20.0f, 20.0f),
				y = Random.Range(-20.0f, 20.0f),
			};
		}

		private void OnCreateChipClicked()
		{
			var chip = circuitManager.Project.CreateChip(out _);
			circuitManager.SwitchChip(chip);
		}

		private void OnChipAddClicked(ChipButton button)
		{
			var chip = chips.Mapping[button];
			var project = circuitManager.Project;

			if (project.DetectCircularReferences(chip, circuitManager.CurrentChip))
			{
				Debug.LogWarning("Prevented instantiating chip that would result in a circular reference!");
				return;
			}

			circuitManager.CurrentChip.circuit.InstantiateChip(chip, project.chips.LookupHandle(chip));

		}

		private void OnChipEditClicked(ChipButton button)
		{
			var chip = chips.Mapping[button];
			circuitManager.SwitchChip(chip);
		}

		private void OnSaveProjectClicked()
		{
			circuitManager.StoreProject();
		}

	}

}