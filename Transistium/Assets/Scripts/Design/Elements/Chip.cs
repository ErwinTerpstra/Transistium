using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{

	public class Chip
	{
		public string name;

		public Circuit circuit;

		public HandleList<Pin> pins;

		public Handle<Pin> vccPinHandle;

		public Handle<Pin> groundPinHandle;

		public string NameOrDefault => name ?? "Unnamed chip";

		public Chip()
		{
			circuit = new Circuit();
			pins = new HandleList<Pin>((byte) ElementType.PIN);
		}

		public Pin AddPin(out Handle<Pin> handle) => AddPin(Guid.Generate(), out handle);

		public Pin AddPin(Guid guid, out Handle<Pin> handle)
		{
			var pin = new Pin();
			circuit.AddJunction(guid, CircuitElementFlags.EMBEDDED, out pin.junctionHandle);

			handle = pins.Add(guid, pin);

			return pin;
		}

		public void RemovePin(Pin pin)
		{
			circuit.RemoveJunction(pin.junctionHandle);

			pins.Remove(pin);
		}

		public void RemovePin(Handle<Pin> pinHandle)
		{
			var pin = pins[pinHandle];

			circuit.RemoveJunction(pin.junctionHandle);

			pins.Remove(pinHandle);
		}

		public bool ShouldInstantiatePin(Handle<Pin> pinHandle)
		{
			return pinHandle != vccPinHandle && pinHandle != groundPinHandle;
		}

		public static Chip Create()
		{
			var chip = new Chip();

			// Create the VCC and ground pins
			// These pins are always added
			// TODO: add something to "hide" these pins on the outside of a ChipInstance

			var vccPin = chip.AddPin(out chip.vccPinHandle);
			vccPin.transform.position = new Vector2(0.0f, 100.0f);
			vccPin.transform.rotation = Rotation.ROTATE_180;
			vccPin.name = "VCC";

			var groundPin = chip.AddPin(out chip.groundPinHandle);
			groundPin.transform.position = new Vector2(0.0f, -100.0f);
			groundPin.name = "GND";

			return chip;
		}

	}
}
