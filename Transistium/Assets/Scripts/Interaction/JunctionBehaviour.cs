using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class JunctionBehaviour : MonoBehaviour
	{
		private Junction target;

		public Junction Target
		{
			get => target;
			set => target = value;
		}
	}
}
