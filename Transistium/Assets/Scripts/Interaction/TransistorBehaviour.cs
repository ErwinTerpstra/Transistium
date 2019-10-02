using UnityEngine;

using Transistium.Design;

namespace Transistium.Interaction
{
	public class TransistorBehaviour : MonoBehaviour
	{
		[SerializeField]
		private JunctionBehaviour gate = null;

		[SerializeField]
		private JunctionBehaviour drain = null;

		[SerializeField]
		private JunctionBehaviour source = null;

		private Transistor target;
		
		public Transistor Target
		{
			get { return target; }
			set
			{
				target = value;

				gate.Target = target.gate;
				drain.Target = target.drain;
				source.Target = target.source;
			}
		}

		public JunctionBehaviour Gate => gate;
		public JunctionBehaviour Drain => drain;
		public JunctionBehaviour Source => source;
	}
}
