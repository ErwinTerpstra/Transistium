using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Transistium.Design
{
	public class ChipInstancePath : IEnumerable<ChipInstance>
	{
		public static readonly ChipInstancePath Root = new ChipInstancePath();

		private List<ChipInstance> path;
		
		public int Depth => path.Count;

		public bool IsRoot => path.Count == 0;

		public ChipInstance Leaf => path[path.Count - 1];

		public ChipInstancePath Parent => path.Count > 0 ? new ChipInstancePath(path.SkipLast(1)) : null;

		public ChipInstancePath()
		{
			path = new List<ChipInstance>();
		}

		public ChipInstancePath(IEnumerable<ChipInstance> path)
		{
			this.path = new List<ChipInstance>(path);
		}

		public IEnumerator<ChipInstance> GetEnumerator() => path.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)path).GetEnumerator();
		}

		public ChipInstancePath Add(ChipInstance chipInstance)
		{
			var childPath = new ChipInstancePath(this);
			childPath.path.Add(chipInstance);

			return childPath;
		}

		public bool Match(ChipInstancePath other)
		{
			if (other.Depth != Depth)
				return false;

			for (int i = 0; i < path.Count; ++i)
			{
				if (path[i] != other.path[i])
					return false;
			}

			return true;
		}
	}
}