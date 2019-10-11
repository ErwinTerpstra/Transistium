using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Interaction;

namespace Transistium.UI
{
	public class Toolbar : MonoBehaviour
	{
		[SerializeField]
		private Button createChipButton = null;

		private CircuitManager circuitManager;

		private void Start()
		{
			circuitManager = CircuitManager.Instance;

			createChipButton.onClick.AddListener(OnCreateChipClicked);
		}

		private void OnCreateChipClicked()
		{
			var chip = circuitManager.Project.CreateChip(out _);
			circuitManager.SwitchChip(chip);
		}

	}

}