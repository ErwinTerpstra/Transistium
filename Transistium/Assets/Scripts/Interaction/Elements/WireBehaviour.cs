using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Transistium.Design;
using Transistium.Runtime;

namespace Transistium.Interaction
{
	public class WireBehaviour : Graphic
	{
		[SerializeField]
		private float width = 4.0f;

		[SerializeField]
		private Color activeColor = Color.red;

		[SerializeField]
		private UILabel label = null;

		private Wire wire;

		private List<Vector3> pathBuffer;

		private List<Vector2> vertexBuffer;

		private UIVertex[] quadBuffer;

		private Signal signal;

		private WireMetrics metrics;

		public Wire Wire
		{
			get => wire;
			set
			{
				wire = value;
				SetVerticesDirty();
			}
		}

		public override Color color 
		{
			get => Color.Lerp(base.color, activeColor, metrics.DutyCycle);// signal.ToLogicLevel() ? activeColor : base.color; 
			set => base.color = value; 
		}

		public Signal Signal
		{
			get => signal;
			set => signal = value;
		}

		public WireMetrics Metrics
		{
			get => metrics;
			set
			{
				metrics = value;
				UpdateLabel();
			}
		}

		private Vector2 FirstPosition
		{
			get
			{
				if (wire.a.IsValid)
					return CircuitManager.Instance.GetJunctionPosition(wire.a);

				if (wire.vertices.Count > 0)
					return CircuitManager.Instance.GetWorldPosition(wire.vertices[0]);

				return Vector2.zero;
			}
		}

		private Vector2 LastPosition
		{
			get
			{
				if (wire.b.IsValid)
					return CircuitManager.Instance.GetJunctionPosition(wire.b);

				if (wire.vertices.Count > 0)
					return CircuitManager.Instance.GetWorldPosition(wire.vertices[^1]);

				return Vector2.zero;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			pathBuffer = new List<Vector3>();
			vertexBuffer = new List<Vector2>();

			label.Text = "-";
			label.gameObject.SetActive(false);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			quadBuffer = new UIVertex[4];
		}

		private void Update()
		{
			if (!CircuitManager.Instance)
				return;

			Vector2 center = (FirstPosition + LastPosition) / 2;
			(label.transform as RectTransform).anchoredPosition = transform.InverseTransformPoint(center);
			SetVerticesDirty();
		}

		private void UpdateLabel()
		{
			label.Text = $"{metrics.DutyCycle * 100:0}%";
		}

		public bool OverlapsPoint(Vector2 position)
		{
			position = transform.InverseTransformPoint(position);

			for (int i = 0; i < pathBuffer.Count - 1; ++i)
			{
				Vector3 a = pathBuffer[i + 0];
				Vector3 b = pathBuffer[i + 1];

				if (VectorUtil.DistanceToLine(a, b, position, out _) < width)
					return true;
			}

			return false;
		}

		private void UpdatePathBuffer()
		{
			if (wire == null)
				return;

			pathBuffer.Clear();

			if (wire.a.IsValid)
				AddPathNode(CircuitManager.Instance.GetJunctionPosition(wire.a));

			foreach (var vertex in wire.vertices)
				AddPathNode(CircuitManager.Instance.GetWorldPosition(vertex));

			if (wire.b.IsValid)
				AddPathNode(CircuitManager.Instance.GetJunctionPosition(wire.b));
		}

		private void AddPathNode(Vector3 position)
		{
			pathBuffer.Add(transform.InverseTransformPoint(position));
		}

		private void UpdateVertexBuffer()
		{
			vertexBuffer.Clear();

			for (int i = 0; i < pathBuffer.Count; ++i)
			{
				bool hasPrevious = i > 0;
				bool hasNext = i < (pathBuffer.Count - 1);

				// Retrieve coordinates of the current vertex
				Vector2 currentVertex = pathBuffer[i];

				// Calculate a smoothed tangent vector to create a line with non-zero width
				Vector2 forward = Vector2.zero;

				if (hasPrevious)
				{
					Vector2 previousVertex = pathBuffer[i - 1];

					Vector2 previousToCurrent = (currentVertex - previousVertex).normalized;
					forward += previousToCurrent;
				}

				if (hasNext)
				{
					Vector2 nextVertex = pathBuffer[i + 1];

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