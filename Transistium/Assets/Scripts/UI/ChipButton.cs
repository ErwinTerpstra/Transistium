using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Transistium.UI
{
	public delegate void ChipButtonEvent(ChipButton button);

	public class ChipButton : MonoBehaviour
	{
		public event ChipButtonEvent AddClicked;
		public event ChipButtonEvent EditClicked;
		
		[SerializeField]
		private Button buttonAdd = null;

		[SerializeField]
		private Button buttonEdit = null;

		[SerializeField]
		private TMPro.TextMeshProUGUI labelName = null;

		private void Awake()
		{
			buttonAdd.onClick.AddListener(OnAddClicked);
			buttonEdit.onClick.AddListener(OnEditClicked);
		}

		public void Configure(string name)
		{
			labelName.text = name;
		}

		private void OnAddClicked()
		{
			AddClicked?.Invoke(this);
		}

		private void OnEditClicked()
		{
			EditClicked?.Invoke(this);
		}
	}

}