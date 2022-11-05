using System;
using System.Collections.Generic;
using System.Linq;
using Transistium.Design.Components;
using Transistium.Util;
using UnityEngine;

namespace Transistium.Design
{
	public class CompilationResult
	{
		public Runtime.Circuit circuit;

		public DebugSymbols symbols;

		public ComponentInstanceMapping componentInstances;
	}

	public class DebugSymbols
	{
		public ChipMapping rootChipMapping;

		public ChipMapping GetChipMapping(ChipInstancePath path)
		{
			ChipMapping current = rootChipMapping;

			foreach (var instance in path)
			{
				if (!current.chipInstanceMapping.TryGetValue(instance, out current))
					throw new ArgumentException("Chip path does not match the chip topology!");
			}

			return current;
		}
	}

}