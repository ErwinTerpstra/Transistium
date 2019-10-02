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
			int vertexCount = target.vertices.Count;

			if (target.a != null)
				++vertexCount;

			if (target.b != null)
				++vertexCount;

			lineRenderer.positionCount = vertexCount;

			int vertexIndex = 0;

			if (target.a != null)
			{
				lineRenderer.SetPosition(vertexIndex, CircuitManager.Instance.GetJunctionPosition(target.a));
				++vertexIndex;
			}

			foreach (var vertex in target.vertices)
			{
				lineRenderer.SetPosition(vertexIndex, CircuitManager.Instance.GetWorldPosition(vertex));
				++vertexIndex;
			}

			if (target.b != null)
			{
				lineRenderer.SetPosition(vertexIndex, CircuitManager.Instance.GetJunctionPosition(target.b));
				++vertexIndex;
			}
		}

		public Wire Target
		{
			get => target;
			set => target = value;
		}
	}

}