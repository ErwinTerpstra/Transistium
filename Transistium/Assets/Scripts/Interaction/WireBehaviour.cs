using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class WireBehaviour : MonoBehaviour
	{
		private Wire wire;

		private LineRenderer lineRenderer;

		private new PolygonCollider2D collider;

		private List<Vector3> vertexBuffer;

		private List<Vector2> colliderBuffer;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
			collider = GetComponent<PolygonCollider2D>();

			vertexBuffer = new List<Vector3>();
			colliderBuffer = new List<Vector2>();
		}

		private void LateUpdate()
		{
			UpdateVertexBuffer();
			UpdateLineRenderer();
			UpdateCollider();
		}

		private void UpdateVertexBuffer()
		{
			vertexBuffer.Clear();

			if (wire.a != Handle<Junction>.Invalid)
				vertexBuffer.Add(CircuitManager.Instance.GetJunctionPosition(wire.a));

			foreach (var vertex in wire.vertices)
				vertexBuffer.Add(CircuitManager.Instance.GetWorldPosition(vertex));

			if (wire.b != Handle<Junction>.Invalid)
				vertexBuffer.Add(CircuitManager.Instance.GetJunctionPosition(wire.b));
		}

		private void UpdateLineRenderer()
		{
			lineRenderer.positionCount = vertexBuffer.Count;

			for (int i = 0; i < vertexBuffer.Count; ++i)
				lineRenderer.SetPosition(i, vertexBuffer[i]);
		}

		private void UpdateCollider()
		{
			colliderBuffer.Clear();

			for (int i = 0; i < vertexBuffer.Count * 2; ++i)
				colliderBuffer.Add(Vector2.zero);

			for (int i = 0; i < vertexBuffer.Count; ++i)
			{
				bool hasPrevious = i > 0;
				bool hasNext = i < (vertexBuffer.Count - 1);

				// Retrieve coordinates of the current vertex
				Vector2 currentVertex = transform.InverseTransformPoint(vertexBuffer[i]);

				// Calculate a smoothed tangent vector to create a line with non-zero width
				Vector2 forward = Vector2.zero;

				if (hasPrevious)
				{
					Vector2 previousVertex = transform.InverseTransformPoint(vertexBuffer[i - 1]);

					Vector2 previousToCurrent = (currentVertex - previousVertex).normalized;
					forward += previousToCurrent;
				}

				if (hasNext)
				{
					Vector2 nextVertex = transform.InverseTransformPoint(vertexBuffer[i + 1]);

					Vector2 currentToNext = (nextVertex - currentVertex).normalized;
					forward += currentToNext;
				}

				forward.Normalize();

				Vector2 tangent = VectorUtil.RotateCW(forward);

				// Calculate the line width at this vertex
				float t = i / (vertexBuffer.Count - 1);
				float width = lineRenderer.widthCurve.Evaluate(t);

				// Calculate the tangent positions
				Vector2 left = currentVertex - tangent * width * 0.5f;
				Vector2 right = currentVertex + tangent * width * 0.5f;

				// Add vertices to the list
				colliderBuffer[i] = left;
				colliderBuffer[colliderBuffer.Count - 1 - i] = right;
			}


			collider.pathCount = 1;
			collider.SetPath(0, colliderBuffer);
		}


		public Wire Wire
		{
			get => wire;
			set => wire = value;
		}
	}

}