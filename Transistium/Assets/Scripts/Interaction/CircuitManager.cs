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

		public WireBehaviour wire;
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

		public Vector2 GetCircuitPosition(Vector3 worldPosition)
		{
			return transform.InverseTransformPoint(worldPosition);
		}

		public Vector3 GetWorldPosition(Vector2 circuitPosition)
		{
			return transform.TransformPoint(circuitPosition);
		}

		public Vector3 GetJunctionPosition(Junction junction)
		{
			JunctionBehaviour behaviour;
			if (!junctionMapping.TryGetValue(junction, out behaviour))
				return Vector3.zero;

			return behaviour.transform.position;
		}

		public Transistor AddTransistor()
		{
			var transistor = new Transistor();
			circuit.transistors.Add(transistor);

			MapTransistor(transistor);

			return transistor;
		}

		public WireBehaviour CreateTemporaryWire(Junction junction)
		{
			WireBehaviour wireBehaviour = Instantiate(prefabs.wire, transform, false);
			wireBehaviour.Target = new Wire() { a = junction, b = null };

			return wireBehaviour;
		}

		public void StoreTemporaryWire(WireBehaviour wireBehaviour)
		{
			var wire = wireBehaviour.Target;

			wire.a.wires.Add(wire);
			wire.b.wires.Add(wire);

			wireMapping.Map(wire, wireBehaviour);
		}

		private TransistorBehaviour MapTransistor(Transistor transistor)
		{
			TransistorBehaviour transistorBehaviour = Instantiate(prefabs.transistor, transform, false);
			transistorBehaviour.Target = transistor;

			CircuitElementBehaviour elementBehaviour = transistorBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Target = transistor;

			transistorMapping.Map(transistor, transistorBehaviour);
			junctionMapping.Map(transistor.gate, transistorBehaviour.Gate);
			junctionMapping.Map(transistor.drain, transistorBehaviour.Drain);
			junctionMapping.Map(transistor.source, transistorBehaviour.Source);

			return transistorBehaviour;
		}
	}

}