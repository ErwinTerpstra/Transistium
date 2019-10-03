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

		public JunctionBehaviour junction;

		public WireBehaviour wire;
	}

	public class CircuitManager : MonoSingleton<CircuitManager>
	{
		[SerializeField]
		private CircuitPrefabs prefabs = null;

		[SerializeField]
		private Transform elementRoot = null;

		[SerializeField]
		private Transform wireRoot = null;

		private Circuit circuit;
		
		private OneToOneMapping<Handle, TransistorBehaviour> transistorMapping;

		private OneToOneMapping<Handle, JunctionBehaviour> junctionMapping;

		private OneToOneMapping<Handle, WireBehaviour> wireMapping;

		protected override void Awake()
		{
			base.Awake();

			circuit = new Circuit();

			transistorMapping = new OneToOneMapping<Handle, TransistorBehaviour>();
			junctionMapping = new OneToOneMapping<Handle, JunctionBehaviour>();
			wireMapping = new OneToOneMapping<Handle, WireBehaviour>();
		}

		public Vector2 GetCircuitPosition(Vector3 worldPosition)
		{
			return transform.InverseTransformPoint(worldPosition);
		}

		public Vector3 GetWorldPosition(Vector2 circuitPosition)
		{
			return transform.TransformPoint(circuitPosition);
		}

		public Vector3 GetJunctionPosition(Handle junctionHandle)
		{
			JunctionBehaviour behaviour;
			if (!junctionMapping.TryGetValue(junctionHandle, out behaviour))
				return Vector3.zero;

			return behaviour.transform.position;
		}
		

		public void RemoveWire(Handle wireHandle)
		{
			circuit.RemoveWire(wireHandle);

			WireBehaviour behaviour;
			if (wireMapping.TryGetValue(wireHandle, out behaviour))
			{
				Destroy(behaviour.gameObject);
				wireMapping.Remove(wireHandle);
			}
		}

		public TransistorBehaviour MapTransistor(Handle transistorHandle)
		{
			TransistorBehaviour transistorBehaviour = Instantiate(prefabs.transistor, elementRoot, false);
			transistorBehaviour.TransistorHandle = transistorHandle;

			Transistor transistor = circuit.GetTransistor(transistorHandle);
			transistorBehaviour.Gate.JunctionHandle = transistor.gate;
			transistorBehaviour.Drain.JunctionHandle = transistor.drain;
			transistorBehaviour.Source.JunctionHandle = transistor.source;

			transistorMapping.Map(transistorHandle, transistorBehaviour);

			CircuitElementBehaviour elementBehaviour = transistorBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = transistor;

			MapJunction(transistorBehaviour.Gate, transistor.gate);
			MapJunction(transistorBehaviour.Drain, transistor.drain);
			MapJunction(transistorBehaviour.Source, transistor.source);

			return transistorBehaviour;
		}

		public JunctionBehaviour MapJunction(Handle junctionHandle)
		{
			JunctionBehaviour junctionBehaviour = Instantiate(prefabs.junction, elementRoot, false);

			MapJunction(junctionBehaviour, junctionHandle);

			return junctionBehaviour;
		}
		
		public void MapJunction(JunctionBehaviour junctionBehaviour, Handle junctionHandle)
		{
			junctionBehaviour.JunctionHandle = junctionHandle;

			junctionMapping.Map(junctionHandle, junctionBehaviour);

			CircuitElementBehaviour elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = circuit.GetJunction(junctionHandle);
		}


		public WireBehaviour MapWire(Handle wireHandle)
		{
			WireBehaviour wireBehaviour = Instantiate(prefabs.wire, wireRoot, false);
			wireBehaviour.WireHandle = wireHandle;

			wireMapping.Map(wireBehaviour.WireHandle, wireBehaviour);

			return wireBehaviour;
		}

		public Circuit Circuit
		{
			get { return circuit; }
		}
	}

}