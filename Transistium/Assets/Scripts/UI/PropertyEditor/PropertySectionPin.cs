﻿using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;

namespace Transistium.UI
{
	public class PropertySectionPin : PropertySection<Pin>
	{
		[SerializeField]
		private TMPro.TMP_InputField inputFieldName = null;

		[SerializeField]
		private TMPro.TMP_Dropdown sideDropdown = null;

		private void Awake()
		{
			inputFieldName.onValueChanged.AddListener(OnNameChanged);
			sideDropdown.onValueChanged.AddListener(OnSideChanged);
		}

		public override void Show(Pin element)
		{
			base.Show(element);

			inputFieldName.text = element.name ?? string.Empty;
			sideDropdown.value = (int)element.side;
		}

		private void OnNameChanged(string name)
		{
			element.name = !string.IsNullOrEmpty(name) ? name : null;
		}

		private void OnSideChanged(int side)
		{
			element.side = (PinSide)side;
		}
	}

}