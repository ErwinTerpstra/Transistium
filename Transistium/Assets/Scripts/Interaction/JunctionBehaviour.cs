using UnityEngine;
using UnityEngine.EventSystems;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class JunctionBehaviour : MonoBehaviour
	{
		private Junction junction;

		public Junction Junction
		{
			get => junction;
			set => junction = value;
		}
	}
}
