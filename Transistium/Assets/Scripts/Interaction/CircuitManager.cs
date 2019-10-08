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

	public delegate void CircuitEvent(Circuit circuit);

	public class CircuitManager : MonoSingleton<CircuitManager>
	{
		public event CircuitEvent CircuitLeft;
		public event CircuitEvent CircuitEntered;

		[SerializeField]
		private CircuitPrefabs prefabs = null;

		[SerializeField]
		private Transform elementRoot = null;

		[SerializeField]
		private Transform wireRoot = null;

		private Project project;

		private Circuit circuit;
		
		private OneToOneMapping<Handle, TransistorBehaviour> transistorMapping;

		private OneToOneMapping<Handle, JunctionBehaviour> junctionMapping;

		private OneToOneMapping<Handle, WireBehaviour> wireMapping;

		protected override void Awake()
		{
			base.Awake();
			
			transistorMapping = new OneToOneMapping<Handle, TransistorBehaviour>();
			junctionMapping = new OneToOneMapping<Handle, JunctionBehaviour>();
			wireMapping = new OneToOneMapping<Handle, WireBehaviour>();

			project = new Project();
			SwitchCircuit(project.rootCircuit);
		}

		private void SwitchCircuit(Circuit circuit)
		{
			if (this.circuit != null)
				LeaveCircuit();

			this.circuit = circuit;

			EnterCircuit();
		}

		private void LeaveCircuit()
		{
			circuit.TransistorAdded -= OnTransistorAdded;
			circuit.TransistorRemoved -= OnTransistorRemoved;

			circuit.WireAdded -= OnWireAdded;
			circuit.WireRemoved -= OnWireRemoved;

			circuit.JunctionAdded -= OnJunctionAdded;
			circuit.JunctionRemoved -= OnJunctionRemoved;

			foreach (var pair in transistorMapping)
				Destroy(pair.Value);

			foreach (var pair in junctionMapping)
				Destroy(pair.Value);

			foreach (var pair in wireMapping)
				Destroy(pair.Value);

			transistorMapping.Clear();
			junctionMapping.Clear();
			wireMapping.Clear();

			CircuitLeft?.Invoke(circuit);
		}

		private void EnterCircuit()
		{
			circuit.TransistorAdded += OnTransistorAdded;
			circuit.TransistorRemoved += OnTransistorRemoved;

			circuit.WireAdded += OnWireAdded;
			circuit.WireRemoved += OnWireRemoved;

			circuit.JunctionAdded += OnJunctionAdded;
			circuit.JunctionRemoved += OnJunctionRemoved;

			foreach (var handle in circuit.Transistors)
				OnTransistorAdded(handle);

			foreach (var handle in circuit.Junctions)
				OnJunctionAdded(handle);

			foreach (var handle in circuit.Wires)
				OnWireAdded(handle);

			CircuitEntered?.Invoke(circuit);
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
		
		private void OnTransistorAdded(Handle handle)
		{
			TransistorBehaviour transistorBehaviour = Instantiate(prefabs.transistor, elementRoot, false);
			transistorBehaviour.TransistorHandle = handle;

			Transistor transistor = circuit.GetTransistor(handle);
			transistorBehaviour.Gate.JunctionHandle = transistor.gate;
			transistorBehaviour.Drain.JunctionHandle = transistor.drain;
			transistorBehaviour.Source.JunctionHandle = transistor.source;

			transistorMapping.Map(handle, transistorBehaviour);

			junctionMapping.Map(transistor.gate, transistorBehaviour.Gate);
			junctionMapping.Map(transistor.drain, transistorBehaviour.Drain);
			junctionMapping.Map(transistor.source, transistorBehaviour.Source);

			CircuitElementBehaviour elementBehaviour = transistorBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = transistor;
		}

		private void OnTransistorRemoved(Handle handle)
		{
			var transistor = circuit.GetTransistor(handle);

			if (transistorMapping.TryGetValue(handle, out TransistorBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				transistorMapping.Remove(handle);
			}
		}

		private void OnJunctionAdded(Handle handle)
		{
			var junction = circuit.GetJunction(handle);

			if (junction.embedded)
				return;

			JunctionBehaviour junctionBehaviour = Instantiate(prefabs.junction, elementRoot, false);
			junctionBehaviour.JunctionHandle = handle;

			junctionMapping.Map(handle, junctionBehaviour);

			CircuitElementBehaviour elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;
		}

		private void OnJunctionRemoved(Handle handle)
		{
			if (junctionMapping.TryGetValue(handle, out JunctionBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				junctionMapping.Remove(handle);
			}
		}

		private void OnWireAdded(Handle handle)
		{
			WireBehaviour wireBehaviour = Instantiate(prefabs.wire, wireRoot, false);
			wireBehaviour.WireHandle = handle;

			wireMapping.Map(handle, wireBehaviour);
		}

		private void OnWireRemoved(Handle handle)
		{
			if (wireMapping.TryGetValue(handle, out WireBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				wireMapping.Remove(handle);
			}
		}

		public Circuit Circuit
		{
			get { return circuit; }
		}
	}

}