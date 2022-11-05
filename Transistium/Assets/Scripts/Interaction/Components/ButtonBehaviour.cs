using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Transistium.Interaction.Components
{
	using Button = Design.Components.Button;

	public class ButtonBehaviour : ComponentBehaviour<Button.Data>
    {
		[SerializeField]
		private Graphic graphic = null;

		[SerializeField]
		private UIButton button = null;

		[SerializeField]
		private Color defaultColor = Color.white;

		[SerializeField]
		private Color pressedColor = Color.red;

		private bool pressState;

		protected override void Awake()
		{
			base.Awake();

			UpdateState();

			button.Pressed += OnButtonPressed;
			button.Released += OnButtonReleased;
		}

		protected override void StoreState(Button.Data data)
		{
			base.StoreState(data);

			data.pressed = pressState;
		}

		private void UpdateState()
		{
			graphic.color = pressState ? pressedColor : defaultColor;
		}

		private void OnButtonPressed(UIButton button)
		{
			pressState = true;
			UpdateState();
		}

		private void OnButtonReleased(UIButton button)
		{
			pressState = false;
			UpdateState();
		}
	}

}