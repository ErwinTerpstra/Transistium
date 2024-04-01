using System;
using System.Collections.Generic;
using Transistium.Design.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Transistium.Interaction.Components
{
	public class SwitchBehaviour : ComponentBehaviour<Switch.Data>
    {
		[SerializeField]
		private Graphic graphic = null;

		[SerializeField]
		private UIButton button = null;

		[SerializeField]
		private Color inactiveColor = Color.white;

		[SerializeField]
		private Color activeColor = Color.red;

		private bool activationState;

		protected override void Awake()
		{
			base.Awake();

			UpdateState();

			button.Clicked += OnButtonClicked;
		}

		protected override void LoadState(Switch.Data data)
		{
			base.LoadState(data);

			activationState = data.active;

			UpdateState();
		}

		protected override void StoreState(Switch.Data data)
		{
			base.StoreState(data);

			data.active = activationState;
		}

		private void UpdateState()
		{
			graphic.color = activationState ? activeColor : inactiveColor;
		}

		private void OnButtonClicked(UIButton button)
		{
			activationState = !activationState;
			UpdateState();
		}

	}

}