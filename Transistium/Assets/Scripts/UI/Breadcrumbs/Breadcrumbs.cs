using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Transistium.UI
{
	public delegate void BreadcrumbsEvent(int level);

	public class Breadcrumbs : MonoBehaviour
	{
		public event BreadcrumbsEvent LevelSelected;

		[SerializeField]
		private BreadcrumbsLabel labelPrefab = null;

		[SerializeField]
		private GameObject separatorPrefab = null;

		private List<BreadcrumbsLabel> labels;

		private List<GameObject> separators;

		private void Awake()
		{
			labels = new List<BreadcrumbsLabel>();
			separators = new List<GameObject>();
		}

		public void SetLevels(List<string> levels)
		{
			// Handle all levels
			for (int level = 0; level < levels.Count; ++level)
			{
				// Check if we need to create a new label
				if (level >= labels.Count)
				{
					if (level > 0)
					{
						var separator = Instantiate(separatorPrefab, transform, false);
						separators.Add(separator);
					}

					var label = Instantiate(labelPrefab, transform, false);
					label.Clicked += OnLabelClicked;
					labels.Add(label);
				}

				// Configure the label
				labels[level].Configure(level, levels[level]);
			}

			// Destroy excessive labels
			while (labels.Count > levels.Count)
			{
				Destroy(labels[labels.Count - 1].gameObject);
				labels.RemoveAt(labels.Count - 1);
			}

			// Destroy excessive separators
			while (separators.Count > levels.Count - 1)
			{
				Destroy(separators[separators.Count - 1].gameObject);
				separators.RemoveAt(separators.Count - 1);
			}

		}

		private void OnLabelClicked(int level)
		{
			LevelSelected?.Invoke(level);
		}
	}

}