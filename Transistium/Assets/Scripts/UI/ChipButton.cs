using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Design;

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

		private Chip chip;

		private void Awake()
		{
			buttonAdd.onClick.AddListener(OnAddClicked);
			buttonEdit.onClick.AddListener(OnEditClicked);
		}

		private void LateUpdate()
		{
			UpdateName();
		}

		public void Configure(Chip chip, bool canEdit)
		{
			this.chip = chip;

			buttonEdit.interactable = canEdit;

			UpdateName();
		}

		private void UpdateName()
		{
			labelName.text = chip.NameOrDefault;
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