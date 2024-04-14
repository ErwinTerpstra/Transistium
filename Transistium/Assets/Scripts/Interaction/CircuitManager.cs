using System;
using System.Collections.Generic;

using UnityEngine;

using Transistium.Util;
using Transistium.Interaction.Components;
using Transistium.Design;

using Guid = Transistium.Guid;
using Transistor = Transistium.Design.Transistor;
using Circuit = Transistium.Design.Circuit;
using CircuitState = Transistium.Runtime.CircuitState;
using System.Linq;
using Transistium.Design.Components;
using Transistium.Runtime;

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

		public ComponentPrefab[] components;
	}

	[Serializable]
	public class ComponentPrefab
	{
		public string componentID;

		public ComponentBehaviour prefab;
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

		private ProjectSerializer projectSerializer;

		private Project project;

		private List<Pair<Chip, ChipInstance>> activeChipStack;

		private Observer<Pin, PinBehaviour> pins;
		private Observer<Wire, WireBehaviour> wires;
		private Observer<Junction, JunctionBehaviour> junctions;
		private Observer<Transistor, TransistorBehaviour> transistors;
		private Observer<ChipInstance, ChipInstanceBehaviour> chipInstances;

		public Project Project => project;

		public Chip CurrentChip
		{
			get
			{
				if (activeChipStack.Count == 0)
					return project.RootChip;

				var pair = activeChipStack[activeChipStack.Count - 1];
				return pair.first;
			}
		}

		public Circuit CurrentCircuit => CurrentChip?.circuit;

		public IEnumerable<Pair<Chip, ChipInstance>> ChipPath => activeChipStack;

		public ChipInstancePath ChipInstancePath => new ChipInstancePath(activeChipStack.Select(pair => pair.second));

		public bool IsEditingChipBlueprint => activeChipStack.Count == 1 && activeChipStack[0].second == null;

		protected override void Awake()
		{
			base.Awake();

			activeChipStack = new List<Pair<Chip, ChipInstance>>();

			chipInstances	= new Observer<ChipInstance,	ChipInstanceBehaviour>(CreateChipInstance, DestroyChipInstance);
			transistors		= new Observer<Transistor,		TransistorBehaviour>(CreateTransistor, DestroyTransistor);
			junctions		= new Observer<Junction,		JunctionBehaviour>(CreateJunction, DestroyJunction);
			wires			= new Observer<Wire,			WireBehaviour>(CreateWire, DestroyWire);
			pins			= new Observer<Pin,				PinBehaviour>(CreatePin, DestroyPin);

			projectSerializer = new ProjectSerializer();
			project = projectSerializer.Load();

			if (project == null)
				project = Project.Create();

			SwitchChip(project.RootChip);
		}

		private void Update()
		{
			pins.DetectChanges();
			wires.DetectChanges();
			junctions.DetectChanges();
			transistors.DetectChanges();
			chipInstances.DetectChanges();
		}

		public void ClearCircuitState()
		{
			foreach (var pair in wires.Mapping)
				pair.Value.Signal = Runtime.Signal.FLOATING;

			foreach (var pair in junctions.Mapping)
				pair.Value.Signal = Runtime.Signal.FLOATING;
		}

		public void LoadCircuitState(
			CircuitState state,
			CircuitMetrics metrics,
			DebugSymbols symbols
		)
		{
			if (IsEditingChipBlueprint)
				return;

			var circuit = CurrentCircuit;
			var chipMapping = symbols.GetChipMapping(ChipInstancePath);

			foreach ((Wire wire, WireBehaviour wireBehaviour) in wires.Mapping)
			{
				Junction junctionA = wire.a.IsValid ? circuit.junctions[wire.a] : null;
				Junction junctionB = wire.b.IsValid ? circuit.junctions[wire.b] : null;

				// NOTE: both A and B ends of the wire should connect to the same compiled wire index
				int wireIndex = chipMapping.junctionMapping[junctionA ?? junctionB];

				// Assign the signal to the wire behaviour
				var signal = state.wires[wireIndex];
				wireBehaviour.Signal = signal;
				wireBehaviour.Metrics = metrics.GetWireMetrics(wireIndex);

				// Assign to junction behaviours as well
				if (junctionA != null)
				{
					JunctionBehaviour junctionBehaviour = junctions.Mapping[junctionA];
					junctionBehaviour.Signal = signal;
				}

				if (junctionB != null)
				{
					JunctionBehaviour junctionBehaviour = junctions.Mapping[junctionB];
					junctionBehaviour.Signal = signal;
				}
			}
		}

		public void LoadComponentState(ComponentInstanceMapping componentInstances)
		{
			if (IsEditingChipBlueprint)
				return;

			var currentPath = ChipInstancePath;
			foreach (var instance in componentInstances.All)
			{
				// Only search for instances in our current active chip
				if (!instance.Path.Parent.Match(currentPath))
					continue;

				ChipInstance chipInstance = instance.Path.Leaf;

				// Retrieve the prefab instance for this chip instance/component
				ChipInstanceBehaviour chipInstanceBehaviour = chipInstances.Mapping[chipInstance];
				ComponentBehaviour componentBehaviour = chipInstanceBehaviour.GetComponent<ComponentBehaviour>();

				// Let the component behaviour load UI state from the component data
				componentBehaviour.LoadState(instance.Data);
			}
		}


		public void StoreComponentState(ComponentInstanceMapping componentInstances)
		{
			if (IsEditingChipBlueprint)
				return;

			var currentPath = ChipInstancePath;
			foreach (var instance in componentInstances.All)
			{
				// Only search for instances in our current active chip
				if (!instance.Path.Parent.Match(currentPath))
					continue;

				ChipInstance chipInstance = instance.Path.Leaf;

				// Retrieve the prefab instance for this chip instance/component
				ChipInstanceBehaviour chipInstanceBehaviour = chipInstances.Mapping[chipInstance];
				ComponentBehaviour componentBehaviour = chipInstanceBehaviour.GetComponent<ComponentBehaviour>();

				// Let the component behaviour store UI state in the component data
				componentBehaviour.StoreState(instance.Data);
			}
		}


		public void StoreProject()
		{
			projectSerializer.Store(project);
		}

		public void SwitchToRoot()
		{
			SwitchChip(project.RootChip);
		}

		public void SwitchChip(ChipInstance chipInstance)
		{
			SwitchChip(project.GetChip(chipInstance.chipHandle), chipInstance);
		}

		public void SwitchChip(Chip chip)
		{
			SwitchChip(chip, null);
		}

		public void SwitchChip(Chip chip, ChipInstance instance)
		{
			var currentChip = CurrentChip;

			if (currentChip != null)
				ChipLeft?.Invoke(currentChip);

			// If we open a chip blueprint instead of an instance, clear the hierarchy stack
			if (chip == project.RootChip || instance == null)
				activeChipStack.Clear();

			if (chip != project.RootChip)
			{
				// Check if we are navigating up by searching for the target instance in the hierarchy stack
				var pair = new Pair<Chip, ChipInstance>(chip, instance);
				int index = activeChipStack.IndexOf(pair);

				if (index >= 0)
					activeChipStack.RemoveRange(index + 1, activeChipStack.Count - index - 1);
				else
					activeChipStack.Add(pair);
			}

			// Take this opportunity to update chip instances for changed properties
			// (e.g. pin instances have been added/removed)
			project.UpdateChipInstances();

			// Change our observers to the new chip
			chipInstances.Observe(chip.circuit.chipInstances);
			transistors.Observe(chip.circuit.transistors);
			junctions.Observe(chip.circuit.junctions);
			wires.Observe(chip.circuit.wires);
			pins.Observe(chip.pins);

			ChipEnterred?.Invoke(chip);
		}

		public Vector2 GetCircuitPosition(Vector3 worldPosition) => transform.InverseTransformPoint(worldPosition);
	
		public Vector3 GetWorldPosition(Vector2 circuitPosition) => transform.TransformPoint(circuitPosition);

		public Vector3 GetJunctionPosition(Handle<Junction> handle)
		{
			return GetJunctionPosition(CurrentChip.circuit.junctions[handle]);
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
			var junction = CurrentChip.circuit.junctions[handle];
			junctionBehaviour.Junction = junction;

			var elementBehaviour = junctionBehaviour.GetComponent<CircuitElementBehaviour>();
			elementBehaviour.Element = junction;

			junctions.Mapping.Map(junction, junctionBehaviour);
		}

		private ComponentBehaviour GetComponentPrefab(Guid guid)
		{
			guid[0] = (byte)ElementType.CHIP;

			foreach (var componentPrefab in prefabs.components)
			{
				Guid componentGuid = Guid.Hash(componentPrefab.componentID);
				componentGuid[0] = (byte)ElementType.CHIP;

				if (componentGuid == guid)
					return componentPrefab.prefab;
			}

			return null;
		}
		
		private ChipInstanceBehaviour CreateChipInstance(ChipInstance chipInstance)
		{
			// Check if this chip instance is for a built-in component
			var component = project.componentLibrary.FindComponent(chipInstance.chipHandle.guid);

			Chip chip;
			ChipInstanceBehaviour prefab;

			// If it is for a component, retrieve the chip and prefab to instantiate
			if (component != null)
			{
				chip = component.Chip;
				
				var componentPrefab = GetComponentPrefab(component.Guid);
				prefab = componentPrefab.GetComponent<ChipInstanceBehaviour>();
			}
			else
			{
				// Otherwise use the default chip instance prefa
				chip = project.GetChip(chipInstance.chipHandle);
				prefab = prefabs.chipInstance;
			}

			// Instantiate the behaviour
			ChipInstanceBehaviour chipInstanceBehaviour = Instantiate(prefab, elementRoot, false);
			chipInstanceBehaviour.PinInstanceCreated += OnPinInstanceCreated;
			chipInstanceBehaviour.PinInstanceDestroyed += OnPinInstanceDestroyed;
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
			SetupJunctionBehaviour(transistor.@base, transistorBehaviour.Base);
			SetupJunctionBehaviour(transistor.collector, transistorBehaviour.Collector);
			SetupJunctionBehaviour(transistor.emitter, transistorBehaviour.Emitter);

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

		private void OnPinInstanceCreated(PinInstance pinInstance, PinInstanceBehaviour behaviour)
		{
			SetupJunctionBehaviour(pinInstance.junctionHandle, behaviour.Junction);
		}

		private void OnPinInstanceDestroyed(PinInstance pinInstance, PinInstanceBehaviour behaviour)
		{

		}
	}

}