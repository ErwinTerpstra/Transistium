using System;
using System.Collections.Generic;
using System.Linq;
using Transistium.Util;
using UnityEngine;

namespace Transistium.Design
{
	public class CompilationResult
	{
		public Runtime.Circuit circuit;

		public DebugSymbols symbols;
	}

	public class DebugSymbols
	{
		public ChipMapping rootChipMapping;

		public ChipMapping GetChipMapping(IEnumerable<ChipInstance> path)
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

	public class ChipMapping
	{
		public readonly Dictionary<ChipInstance, ChipMapping> chipInstanceMapping;

		public readonly OneToManyMapping<int, Junction> junctionMapping;

		public readonly Dictionary<Transistor, int> transistorMapping;

		public ChipMapping()
		{
			chipInstanceMapping = new Dictionary<ChipInstance, ChipMapping>();
			junctionMapping = new OneToManyMapping<int, Junction>();
			transistorMapping = new Dictionary<Transistor, int>();
		}
	}
}