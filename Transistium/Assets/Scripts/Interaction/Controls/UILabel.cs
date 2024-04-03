using System;
using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Interaction
{
	public class UILabel : MonoBehaviour
	{
		[SerializeField]
		private bool keepUpright = true;
		
		[SerializeField]
		private TMPro.TMP_Text textMesh = null;

		public string Text
		{
			get => textMesh.text;
			set => textMesh.text = value;
		}

		private void Awake()
		{
			if (!textMesh)
				textMesh = GetComponent<TMPro.TMP_Text>();
		}

		private void LateUpdate()
		{
			if (keepUpright)
				transform.rotation = Quaternion.identity;
		}
	}
}
