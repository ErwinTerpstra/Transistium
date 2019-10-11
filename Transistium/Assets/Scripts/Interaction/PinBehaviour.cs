using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class PinBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour junction = null;

		private Pin pin;

		public Pin Pin
		{
			get => pin;
			set => pin = value;
		}

		public JunctionBehaviour Junction => junction;
	}
}
