using System;
using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Interaction
{
	public class Label : MonoBehaviour
	{
		[SerializeField]
		private bool keepUpright = true;

		private void LateUpdate()
		{
			if (keepUpright)
				transform.rotation = Quaternion.identity;
		}
	}
}
