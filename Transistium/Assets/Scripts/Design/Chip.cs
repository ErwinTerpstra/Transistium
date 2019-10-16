using System.Collections.Generic;

using UnityEngine;

namespace Transistium.Design
{
	public enum PinSide
	{
		TOP,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public class Pin : CircuitElement
	{
		public string name;

		public PinSide side;

		public Handle<Junction> junctionHandle;

		public string NameOrDefault
		{
			get { return name ?? "???"; }
		}
	}

	public static class PinExtensions
	{
		public static Rotation ToRotation(this PinSide side)
		{
			switch (side)
			{
				default:
				case PinSide.TOP:
					return Rotation.NONE;

				case PinSide.BOTTOM:
					return Rotation.ROTATE_180;

				case PinSide.RIGHT:
					return Rotation.ROTATE_90;

				case PinSide.LEFT:
					return Rotation.ROTATE_270;
			}
		}
	}

	public class Chip
	{
		public string name;

		public Circuit circuit;

		public HandleList<Pin> pins;

		public Handle<Pin> vccPinHandle;

		public Handle<Pin> groundPinHandle;

		public Chip()
		{
			circuit = new Circuit();
			pins = new HandleList<Pin>();

			// Create the VCC and ground pins
			// These pins are always added
			// TODO: add something to "hide" these pins on the outside of a ChipInstance

			var vccPin = AddPin(out vccPinHandle);
			vccPin.transform.position = new Vector2(0.0f, 2.0f);
			vccPin.transform.rotation = Rotation.ROTATE_180;
			vccPin.name = "VCC";

			var groundPin = AddPin(out groundPinHandle);
			groundPin.transform.position = new Vector2(0.0f, -2.0f);
			groundPin.name = "GND";
		}

		public Pin AddPin(out Handle<Pin> handle)
		{
			var pin = new Pin();
			circuit.AddJunction(CircuitElementFlags.EMBEDDED, out pin.junctionHandle);

			handle = pins.Add(pin);

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

		public string NameOrDefault
		{
			get { return name ?? "Unnamed chip"; }
		}

	}
}
