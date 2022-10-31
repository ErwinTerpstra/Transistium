using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class TransistorBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour @base = null;

		[SerializeField]
		private JunctionBehaviour collector = null;

		[SerializeField]
		private JunctionBehaviour emitter = null;

		private Transistor transistor;

		public Transistor Transistor
		{
			get { return transistor; }
			set { transistor = value; }
		}

		public JunctionBehaviour Base => @base;
		public JunctionBehaviour Collector => collector;
		public JunctionBehaviour Emitter => emitter;
	}
}
