using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class PinInstanceBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour junction = null;

		[SerializeField]
		private TMPro.TMP_Text labelName = null;

		private Pin pin;

		private PinInstance pinInstance;

		public Pin Pin => pin;

		public PinInstance PinInstance => pinInstance;

		public JunctionBehaviour Junction => junction;

		private void LateUpdate()
		{
			if (pin != null)
				labelName.text = pin.NameOrDefault;
		}

		public void Configure(Pin pin, PinInstance pinInstance)
		{
			this.pin = pin;
			this.pinInstance = pinInstance;
		}
	}
}
