using System;
using System.Collections.Generic;
using System.Linq;
using Transistium.Util;
using UnityEngine;

namespace Transistium.Design
{
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