using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Transistium.Interaction
{
	public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public delegate void ButtonEvent(UIButton button);

		public event ButtonEvent Pressed;
		public event ButtonEvent Released;
		public event ButtonEvent Clicked;

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			Pressed?.Invoke(this);
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			Released?.Invoke(this);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			Clicked?.Invoke(this);
		}
	}
}