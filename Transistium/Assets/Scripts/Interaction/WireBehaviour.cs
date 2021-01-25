using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class WireBehaviour : Graphic
	{
		[SerializeField]
		private float width = 4.0f;

		private Wire wire;

		private List<Vector3> pathBuffer;

		private List<Vector2> vertexBuffer;

		private UIVertex[] quadBuffer;

		public Wire Wire
		{
			get => wire;
			set
			{
				wire = value;
				SetVerticesDirty();
			}
		}

		protected override void Awake()
		{
			base.Awake();

			pathBuffer = new List<Vector3>();
			vertexBuffer = new List<Vector2>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			quadBuffer = new UIVertex[4];
		}

		private void Update()
		{
			SetVerticesDirty();
		}

		private void UpdatePathBuffer()
		{
			if (wire == null)
				return;

			pathBuffer.Clear();

			if (wire.a != Handle<Junction>.Invalid)
				pathBuffer.Add(CircuitManager.Instance.GetJunctionPosition(wire.a));

			foreach (var vertex in wire.vertices)
				pathBuffer.Add(CircuitManager.Instance.GetWorldPosition(vertex));

			if (wire.b != Handle<Junction>.Invalid)
				pathBuffer.Add(CircuitManager.Instance.GetJunctionPosition(wire.b));
		}

		private void UpdateVertexBuffer()
		{
			vertexBuffer.Clear();

			for (int i = 0; i < pathBuffer.Count; ++i)
			{
				bool hasPrevious = i > 0;
				bool hasNext = i < (pathBuffer.Count - 1);

				// Retrieve coordinates of the current vertex
				Vector2 currentVertex = transform.InverseTransformPoint(pathBuffer[i]);

				// Calculate a smoothed tangent vector to create a line with non-zero width
				Vector2 forward = Vector2.zero;

				if (hasPrevious)
				{
					Vector2 previousVertex = transform.InverseTransformPoint(pathBuffer[i - 1]);

					Vector2 previousToCurrent = (currentVertex - previousVertex).normalized;
					forward += previousToCurrent;
				}

				if (hasNext)
				{
					Vector2 nextVertex = transform.InverseTransformPoint(pathBuffer[i + 1]);

					Vector2 currentToNext = (nextVertex - currentVertex).normalized;
					forward += currentToNext;
				}

				forward.Normalize();

				Vector2 tangent = VectorUtil.RotateCW(forward);

				// Calculate the tangent positions
				Vector2 left = currentVertex - tangent * width * 0.5f;
				Vector2 right = currentVertex + tangent * width * 0.5f;

				// Add vertices to the list
				vertexBuffer.Add(left);
				vertexBuffer.Add(right);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			UpdatePathBuffer();
			UpdateVertexBuffer();

			vh.Clear();

			for (int i = 0; i < 4; ++i)
			{
				ref UIVertex vertex = ref quadBuffer[i];
				vertex.color = color;
			}

			for (int i = 0; i < vertexBuffer.Count - 3; i += 2)
			{
				quadBuffer[0].position = vertexBuffer[i + 0];
				quadBuffer[1].position = vertexBuffer[i + 1];
				quadBuffer[2].position = vertexBuffer[i + 3];
				quadBuffer[3].position = vertexBuffer[i + 2];

				vh.AddUIVertexQuad(quadBuffer);
			}

		}

	}

}