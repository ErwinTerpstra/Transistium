using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class PinBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour junction = null;

		[SerializeField]
		private TMPro.TMP_Text labelName = null;

		private Pin pin;
		public Pin Pin
		{
			get => pin;
			set => pin = value;
		}

		public JunctionBehaviour Junction => junction;

		private void LateUpdate()
		{
			if (pin != null)
				labelName.text = pin.NameOrDefault;
		}

	}
}
