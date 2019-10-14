using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Interaction;

namespace Transistium.UI
{
	public class ChipBreadcrumbs : MonoBehaviour
	{
		private Breadcrumbs breadcrumbs;

		private CircuitManager circuitManager;

		private List<string> levels;

		private void Awake()
		{
			breadcrumbs = GetComponent<Breadcrumbs>();
			breadcrumbs.LevelSelected += OnLevelSelected;

			levels = new List<string>();
		}

		private void Start()
		{
			circuitManager = CircuitManager.Instance;
		}

		private void LateUpdate()
		{
			levels.Clear();

			levels.Add("Root");

			if (circuitManager.Chip != circuitManager.Project.RootChip)
				levels.Add(circuitManager.Chip.NameOrDefault);

			breadcrumbs.SetLevels(levels);
		}

		private void OnLevelSelected(int level)
		{
			// The lowest element is already selected
			if (level == levels.Count - 1)
				return;

			circuitManager.SwitchChip(circuitManager.Project.RootChip);
		}
	}

}