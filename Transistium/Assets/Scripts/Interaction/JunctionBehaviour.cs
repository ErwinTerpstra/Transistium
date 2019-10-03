using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class JunctionBehaviour : MonoBehaviour
	{
		private Handle junctionHandle;

		public Handle JunctionHandle
		{
			get => junctionHandle;
			set => junctionHandle = value;
		}
	}
}
