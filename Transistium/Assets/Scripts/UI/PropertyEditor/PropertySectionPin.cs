using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;

namespace Transistium.UI
{
	public class PropertySectionPin : PropertySection<Pin>
	{
		[SerializeField]
		private TMPro.TMP_InputField inputFieldName = null;

		private void Awake()
		{
			inputFieldName.onValueChanged.AddListener(OnNameChanged);
		}

		public override void Show(Pin element)
		{
			base.Show(element);

			inputFieldName.text = element.name ?? string.Empty;
		}

		private void OnNameChanged(string name)
		{
			element.name = !string.IsNullOrEmpty(name) ? name : null;
		}
	}

}