using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Interaction;
using Transistium.Util;
using Transistium.Design;
using System.Linq;

using Random = UnityEngine.Random;
using Component = Transistium.Design.Components.Component;

namespace Transistium.UI
{
	public struct ToolbarElement
	{
		public Chip chip;

		public Handle<Chip> chipHandle;

		public Component component;
	}

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

		private Observer<ToolbarElement, ChipButton> elements;

		private void Start()
		{
			circuitManager = CircuitManager.Instance;

			var project = circuitManager.Project;

			var options = project.AllChips.Select(handle => new ToolbarElement()
			{
				chip = project.GetChip(handle),
				chipHandle = handle,
				component = project.componentLibrary.FindComponent(handle)
			});

			elements = new Observer<ToolbarElement, ChipButton>(CreateOptionButton, DestroyOptionButton);
			elements.Observe(options);

			buttonAddTransistor.onClick.AddListener(OnAddTransistorClicked);
			buttonAddPin.onClick.AddListener(OnAddPinClicked);

			buttonCreateChip.onClick.AddListener(OnCreateChipClicked);

			buttonSaveProject.onClick.AddListener(OnSaveProjectClicked);
		}

		private void LateUpdate()
		{
			elements.DetectChanges();
		}

		private ChipButton CreateOptionButton(ToolbarElement option)
		{
			var chipButton = Instantiate(chipButtonPrefab, chipRoot, false);

			chipButton.Configure(option.chip, option.component == null);

			chipButton.AddClicked += OnChipInstantiateClicked;
			chipButton.EditClicked += OnChipEditClicked;

			return chipButton;
		}

		private void DestroyOptionButton(ToolbarElement option, ChipButton chipButton)
		{
			Destroy(chipButton.gameObject);
		}


		private void OnAddTransistorClicked()
		{
			circuitManager.CurrentCircuit.AddTransistor(out _);
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

		private void OnChipInstantiateClicked(ChipButton button)
		{
			var element = elements.Mapping[button];
			var project = circuitManager.Project;

			if (project.DetectCircularReferences(element.chip, circuitManager.CurrentChip))
			{
				Debug.LogWarning("Prevented instantiating chip that would result in a circular reference!");
				return;
			}

			circuitManager.CurrentChip.circuit.InstantiateChip(element.chip, element.chipHandle);
		}

		private void OnChipEditClicked(ChipButton button)
		{
			var element = elements.Mapping[button];

			if (element.component != null)
				return;

			circuitManager.SwitchChip(element.chip);
		}

		private void OnSaveProjectClicked()
		{
			circuitManager.StoreProject();
		}

	}

}