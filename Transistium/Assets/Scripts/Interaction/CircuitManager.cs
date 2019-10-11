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

		public PinBehaviour pin;
	}

	public delegate void ChipEvent(Chip chip);

	public class CircuitManager : MonoSingleton<CircuitManager>
	{
		public event ChipEvent ChipLeft;
		public event ChipEvent ChipEnterred;

		[SerializeField]
		private CircuitPrefabs prefabs = null;

		[SerializeField]
		private Transform elementRoot = null;

		[SerializeField]
		private Transform wireRoot = null;

		private Project project;

		private Chip chip;
		
		private OneToOneMapping<Transistor, TransistorBehaviour> transistorMapping;

		private OneToOneMapping<Junction, JunctionBehaviour> junctionMapping;

		private OneToOneMapping<Wire, WireBehaviour> wireMapping;

		private OneToOneMapping<Pin, PinBehaviour> pinMapping;

		protected override void Awake()
		{
			base.Awake();
			
			transistorMapping		= new OneToOneMapping<Transistor, TransistorBehaviour>();
			junctionMapping			= new OneToOneMapping<Junction, JunctionBehaviour>();
			wireMapping				= new OneToOneMapping<Wire, WireBehaviour>();
			pinMapping				= new OneToOneMapping<Pin, PinBehaviour>();

			project = new Project();
			SwitchChip(project.chips[project.rootChipHandle]);
		}

		private void Update()
		{
			DetectChanges(transistorMapping,	chip.circuit.transistors,	OnTransistorAdded,	OnTransistorRemoved);
			DetectChanges(junctionMapping,		chip.circuit.junctions,		OnJunctionAdded,	OnJunctionRemoved);
			DetectChanges(wireMapping,			chip.circuit.wires,			OnWireAdded,		OnWireRemoved);
			DetectChanges(pinMapping,			chip.pins,					OnPinAdded,			OnPinRemoved);
		}

		private void DetectChanges<A, B>(OneToOneMapping<A, B> mapping, ICollection<A> source, Action<A> addedHandler, Action<A> removedHandler)
		{
			foreach (var comparisonEntr in mapping.CompareTo(source))
			{
				switch (comparisonEntr.second)
				{
					case ComparisonResult.ADDED:
						addedHandler?.Invoke(comparisonEntr.first);
						break;

					case ComparisonResult.REMOVED:
						removedHandler?.Invoke(comparisonEntr.first);
						break;
				}
			}
		}

		public void SwitchChip(Chip chip)
		{
			if (this.chip != null)
				LeaveChip();

			this.chip = chip;

			EnterChip();
		}

		private void LeaveChip()
		{
			// Destroy all elements
			foreach (var pair in transistorMapping)
				Destroy(pair.Value);

			foreach (var pair in junctionMapping)
				Destroy(pair.Value);

			foreach (var pair in wireMapping)
				Destroy(pair.Value);

			transistorMapping.Clear();
			junctionMapping.Clear();
			wireMapping.Clear();

			ChipLeft?.Invoke(chip);
		}

		private void EnterChip()
		{
			// Handle instantiating all elements
			foreach (var handle in chip.circuit.transistors)
				OnTransistorAdded(handle);

			foreach (var handle in chip.circuit.junctions)
				OnJunctionAdded(handle);

			foreach (var handle in chip.circuit.wires)
				OnWireAdded(handle);

			ChipEnterred?.Invoke(chip);
		}

		public Vector2 GetCircuitPosition(Vector3 worldPosition)
		{
			return transform.InverseTransformPoint(worldPosition);
		}

		public Vector3 GetWorldPosition(Vector2 circuitPosition)
		{
			return transform.TransformPoint(circuitPosition);
		}

		public Vector3 GetJunctionPosition(Handle<Junction> handle)
		{
			return GetJunctionPosition(chip.circuit.junctions[handle]);
		}

		public Vector3 GetJunctionPosition(Junction junction)
		{
			JunctionBehaviour behaviour;
			if (!junctionMapping.TryGetValue(junction, out behaviour))
				return Vector3.zero;

			return behaviour.transform.position;
		}

		private void SetupJunctionBehaviour(Handle<Junction> handle, JunctionBehaviour junctionBehaviour)
		{
			var junction = chip.circuit.junctions[handle];
			junctionBehaviour.Junction = junction;

			var elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;

			junctionMapping.Map(junction, junctionBehaviour);
		}
		
		private void OnTransistorAdded(Transistor transistor)
		{
			// Instantiate the behaviour
			TransistorBehaviour transistorBehaviour = Instantiate(prefabs.transistor, elementRoot, false);
			transistorBehaviour.Transistor = transistor;

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = transistorBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = transistor;

			// Link junction behaviours with the correct junctions
			SetupJunctionBehaviour(transistor.gate, transistorBehaviour.Gate);
			SetupJunctionBehaviour(transistor.drain, transistorBehaviour.Drain);
			SetupJunctionBehaviour(transistor.source, transistorBehaviour.Source);

			// Store the behaviour in the mapping tables
			transistorMapping.Map(transistor, transistorBehaviour);
		}

		private void OnTransistorRemoved(Transistor transistor)
		{
			if (transistorMapping.TryGetValue(transistor, out TransistorBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				transistorMapping.Remove(transistor);
			}
		}

		private void OnJunctionAdded(Junction junction)
		{
			// Embedded junctions will be instantiated as part of another element (i.e. transistor)
			if (junction.flags.Has(CircuitElementFlags.EMBEDDED))
				return;

			// Instantiate a new standalone junction behaviour
			JunctionBehaviour junctionBehaviour = Instantiate(prefabs.junction, elementRoot, false);
			junctionBehaviour.Junction = junction;

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;

			// Store it in the mapping
			junctionMapping.Map(junction, junctionBehaviour);
		}

		private void OnJunctionRemoved(Junction junction)
		{
			if (junctionMapping.TryGetValue(junction, out JunctionBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				junctionMapping.Remove(junction);
			}
		}

		private void OnWireAdded(Wire wire)
		{
			WireBehaviour wireBehaviour = Instantiate(prefabs.wire, wireRoot, false);
			wireBehaviour.Wire = wire;

			wireMapping.Map(wire, wireBehaviour);
		}

		private void OnWireRemoved(Wire wire)
		{
			if (wireMapping.TryGetValue(wire, out WireBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				wireMapping.Remove(wire);
			}
		}

		private void OnPinAdded(Pin pin)
		{
			// Instantiate the pin behaviour
			PinBehaviour pinBehaviour = Instantiate(prefabs.pin, elementRoot, false);
			pinBehaviour.Pin = pin;

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = pinBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = pin;

			// Store it in the mapping
			pinMapping.Map(pin, pinBehaviour);

			SetupJunctionBehaviour(pin.junctionHandle, pinBehaviour.Junction);
		}

		private void OnPinRemoved(Pin pin)
		{
			if (pinMapping.TryGetValue(pin, out PinBehaviour behaviour))
			{
				Destroy(behaviour.gameObject);
				pinMapping.Remove(pin);
			}
		}

		public Project Project
		{
			get { return project; }
		}

		public Chip Chip
		{
			get { return chip; }
		}
	}

}