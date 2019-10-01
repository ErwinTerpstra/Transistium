using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class WireBehaviour : MonoBehaviour
	{
		private Wire target;

		private LineRenderer lineRenderer;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		private void LateUpdate()
		{
			// TODO:
			// - Lookup Interaction.Junctions that match target junctions
			// - Update line renderer vertices
		}

		public Wire Target
		{
			get { return target; }
			set { target = value; }
		}
	}

}