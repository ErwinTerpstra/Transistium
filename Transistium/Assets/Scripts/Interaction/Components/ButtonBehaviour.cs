using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Transistium.Interaction.Components
{
    public class ButtonBehaviour : ComponentBehaviour, IPointerDownHandler, IPointerUpHandler
    {
		[SerializeField]
		private Graphic graphic = null;

		[SerializeField]
		private Button button = null;

		[SerializeField]
		private Color defaultColor = Color.white;

		[SerializeField]
		private Color pressedColor = Color.red;

		private bool pressState;

		protected override void Awake()
		{
			base.Awake();

			UpdateState();
		}

		private void UpdateState()
		{
			graphic.color = pressState ? pressedColor : defaultColor;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			pressState = true;
			UpdateState();
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			pressState = false;
			UpdateState();
		}
	}

}