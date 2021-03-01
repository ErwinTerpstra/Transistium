using System;
using System.Collections.Generic;

using UnityEngine;

namespace Transistium
{
	[ExecuteInEditMode]
	public class RectTransformScaler : MonoBehaviour
	{
		private RectTransform rectTransform;

		private SpriteRenderer spriteRenderer;

		private BoxCollider2D boxCollider;

		private void LateUpdate()
		{
			ApplySize();
		}

		private void ApplySize()
		{
			if (!rectTransform)
				rectTransform = GetComponent<RectTransform>();

			if (!spriteRenderer)
				spriteRenderer = GetComponent<SpriteRenderer>();

			if (!boxCollider)
				boxCollider = GetComponent<BoxCollider2D>();

			if (!rectTransform)
				return;

			Rect rect = rectTransform.rect;

			if (spriteRenderer)
				spriteRenderer.size = rect.size;

			if (boxCollider)
				boxCollider.size = rect.size;
		}

	}

}