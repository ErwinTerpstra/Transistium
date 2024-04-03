using System;
using System.Collections.Generic;
using UnityEngine;

namespace Transistium.Design.Components
{
	public class ComponentInstanceMapping
	{
		private Dictionary<ChipInstancePath, ComponentInstance> instances;

		public IEnumerable<ComponentInstance> All => instances.Values;

		public ComponentInstanceMapping()
		{
			instances = new Dictionary<ChipInstancePath, ComponentInstance>();
		}

		public void Add(ChipInstancePath path, ComponentInstance instance)
		{
			instances.Add(path, instance);
		}

		public bool Find(ChipInstancePath path, out ComponentInstance instance)
		{
			return instances.TryGetValue(path, out instance);
		}

		public IEnumerable<ComponentInstance> MatchPath(ChipInstancePath path)
		{
			foreach (var pair in instances)
			{
				if (pair.Key.Match(path))
					yield return pair.Value;
			}
		}
	}
}