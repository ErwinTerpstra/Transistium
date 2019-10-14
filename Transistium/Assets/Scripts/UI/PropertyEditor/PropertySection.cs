using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;

namespace Transistium.UI
{
	public class PropertySection<T> : MonoBehaviour where T : class
	{
		protected T element;

		public virtual void Show(T element)
		{
			this.element = element;

			gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		public T Element
		{
			get { return element; }
		}
	}

}