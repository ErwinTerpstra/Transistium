using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Design;
using Transistium.Util;

namespace Transistium.Interaction
{

	[Serializable]
	public class CircuitPrefabs
	{
		public TransistorBehaviour transistor;
	}

	public class CircuitManager : MonoSingleton<CircuitManager>
	{
		[SerializeField]
		private CircuitPrefabs prefabs = null;

		private Circuit circuit;
		
		private OneToOneMapping<Transistor, TransistorBehaviour> transistorMapping;

		private OneToOneMapping<Junction, JunctionBehaviour> junctionMapping;

		private OneToOneMapping<Wire, WireBehaviour> wireMapping;

		protected override void Awake()
		{
			base.Awake();

			circuit = new Circuit();

			transistorMapping = new OneToOneMapping<Transistor, TransistorBehaviour>();
			junctionMapping = new OneToOneMapping<Junction, JunctionBehaviour>();
			wireMapping = new OneToOneMapping<Wire, WireBehaviour>();
		}

		public Transistor AddTransistor()
		{
			var transistor = new Transistor();
			circuit.transistors.Add(transistor);

			return transistor;
		}
	}

}