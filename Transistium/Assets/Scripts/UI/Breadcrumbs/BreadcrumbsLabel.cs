using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Transistium.UI
{
	public class BreadcrumbsLabel : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
	{
		public event BreadcrumbsEvent Clicked;

		[SerializeField]
		private TMPro.TextMeshProUGUI label = null;

		private int level;

		public void Configure(int level, string text)
		{
			this.level = level;

			label.text = text;
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Clicked?.Invoke(level);
		}
	}

}