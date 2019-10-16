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

		public ChipInstanceBehaviour chipInstance;
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

		private Chip currentChip;
		
		private Observer<Transistor, TransistorBehaviour> transistors;

		private Observer<Junction, JunctionBehaviour> junctions;

		private Observer<Wire, WireBehaviour> wires;

		private Observer<Pin, PinBehaviour> pins;

		private Observer<ChipInstance, ChipInstanceBehaviour> chipInstances;

		protected override void Awake()
		{
			base.Awake();

			chipInstances	= new Observer<ChipInstance,	ChipInstanceBehaviour>(CreateChipInstance, DestroyChipInstance);
			transistors		= new Observer<Transistor,		TransistorBehaviour>(CreateTransistor, DestroyTransistor);
			junctions		= new Observer<Junction,		JunctionBehaviour>(CreateJunction, DestroyJunction);
			wires			= new Observer<Wire,			WireBehaviour>(CreateWire, DestroyWire);
			pins			= new Observer<Pin,				PinBehaviour>(CreatePin, DestroyPin);

			project = new Project();
			SwitchChip(project.chips[project.rootChipHandle]);
		}

		private void Update()
		{
			chipInstances.DetectChanges();
			transistors.DetectChanges();
			junctions.DetectChanges();
			wires.DetectChanges();
			pins.DetectChanges();
		}

		public void SwitchChip(Chip chip)
		{
			if (this.currentChip != null)
				ChipLeft?.Invoke(chip);

			this.currentChip = chip;

			chipInstances.Observe(chip.circuit.chipInstances);
			transistors.Observe(chip.circuit.transistors);
			junctions.Observe(chip.circuit.junctions);
			wires.Observe(chip.circuit.wires);
			pins.Observe(chip.pins);

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
			return GetJunctionPosition(currentChip.circuit.junctions[handle]);
		}

		public Vector3 GetJunctionPosition(Junction junction)
		{
			JunctionBehaviour behaviour;
			if (!junctions.Mapping.TryGetValue(junction, out behaviour))
				return Vector3.zero;

			return behaviour.transform.position;
		}

		private void SetupJunctionBehaviour(Handle<Junction> handle, JunctionBehaviour junctionBehaviour)
		{
			var junction = currentChip.circuit.junctions[handle];
			junctionBehaviour.Junction = junction;

			var elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;

			junctions.Mapping.Map(junction, junctionBehaviour);
		}
		
		private ChipInstanceBehaviour CreateChipInstance(ChipInstance chipInstance)
		{
			var chip = project.chips[chipInstance.chipHandle];

			// Instantiate the behaviour
			ChipInstanceBehaviour chipInstanceBehaviour = Instantiate(prefabs.chipInstance, elementRoot, false);
			chipInstanceBehaviour.Configure(chip, chipInstance);

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = chipInstanceBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = chipInstance;

			return chipInstanceBehaviour;
		}

		private void DestroyChipInstance(ChipInstance chipInstance, ChipInstanceBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);
		}

		private TransistorBehaviour CreateTransistor(Transistor transistor)
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

			return transistorBehaviour;
		}

		private void DestroyTransistor(Transistor transistor, TransistorBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);
		}

		private JunctionBehaviour CreateJunction(Junction junction)
		{
			// Embedded junctions will be instantiated as part of another element (i.e. transistor)
			if (junction.flags.Has(CircuitElementFlags.EMBEDDED))
				return null;

			// Instantiate a new standalone junction behaviour
			JunctionBehaviour junctionBehaviour = Instantiate(prefabs.junction, elementRoot, false);
			junctionBehaviour.Junction = junction;

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;

			return junctionBehaviour;
		}

		private void DestroyJunction(Junction junction, JunctionBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);
		}

		private WireBehaviour CreateWire(Wire wire)
		{
			WireBehaviour wireBehaviour = Instantiate(prefabs.wire, wireRoot, false);
			wireBehaviour.Wire = wire;

			return wireBehaviour;
		}

		private void DestroyWire(Wire wire, WireBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);
		}

		private PinBehaviour CreatePin(Pin pin)
		{
			// Instantiate the pin behaviour
			PinBehaviour pinBehaviour = Instantiate(prefabs.pin, elementRoot, false);
			pinBehaviour.Pin = pin;

			// Link the element behaviour
			CircuitElementBehaviour elementBehaviour = pinBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = pin;

			SetupJunctionBehaviour(pin.junctionHandle, pinBehaviour.Junction);

			return pinBehaviour;
		}

		private void DestroyPin(Pin pin, PinBehaviour behaviour)
		{
			Destroy(behaviour.gameObject);
		}

		public Project Project
		{
			get { return project; }
		}

		public Chip CurrentChip
		{
			get { return currentChip; }
		}
	}

}