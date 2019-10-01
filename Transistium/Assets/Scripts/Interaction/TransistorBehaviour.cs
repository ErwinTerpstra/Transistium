using UnityEngine;

namespace Transistium.Interaction
{
	public class TransistorBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour gate = null;

		[SerializeField]
		private JunctionBehaviour drain = null;

		[SerializeField]
		private JunctionBehaviour source = null;

	}
}
