using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Interaction;
using System.Linq;

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

			if (circuitManager.IsEditingChipBlueprint)
			{
				levels.Add("Project");
				levels.Add(circuitManager.CurrentChip.NameOrDefault + " (Blueprint)");
			}
			else 
			{
				levels.Add("Root");

				foreach (var pair in circuitManager.ChipPath)
					levels.Add(pair.first.NameOrDefault);
			}

			breadcrumbs.SetLevels(levels);
		}

		private void OnLevelSelected(int level)
		{
			// The lowest element is already selected
			if (level == levels.Count - 1)
				return;

			if (level > 0)
			{
				var pair = circuitManager.ChipPath.ElementAt(level - 1);
				circuitManager.SwitchChip(pair.first, pair.second);
			}
			else
				circuitManager.SwitchToRoot();
		}
	}

}