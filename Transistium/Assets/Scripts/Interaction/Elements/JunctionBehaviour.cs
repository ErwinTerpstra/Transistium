﻿using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;
using Transistium.Runtime;
using UnityEngine.UI;

namespace Transistium.Interaction
{
	public class JunctionBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Color defaultColor = Color.white;

		[SerializeField]
		private Color activeColor = Color.white;

		[SerializeField]
		private Graphic graphic = null;

		private Junction junction;

		private Signal signal;

		public Junction Junction
		{
			get => junction;
			set => junction = value;
		}

		public Signal Signal
		{
			get => signal;
			set
			{
				signal = value;
				graphic.color = signal.ToLogicLevel() ? activeColor : defaultColor;
			}
		}
	}
}
