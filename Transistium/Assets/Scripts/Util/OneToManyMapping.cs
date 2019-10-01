using System;
using System.Collections.Generic;

namespace Transistium.Util
{
	public class OneToManyMapping<One, Many>
	{
		private Dictionary<Many, One> manyToOne;

		private Dictionary<One, List<Many>> oneToMany;

		public OneToManyMapping()
		{
			manyToOne = new Dictionary<Many, One>();
			oneToMany = new Dictionary<One, List<Many>>();
		}

		public One this[Many many]
		{
			get { return manyToOne[many]; }
		}

		public void Map(One one, IEnumerable<Many> manies)
		{
			oneToMany.Add(one, new List<Many>(manies));

			foreach (Many many in manies)
				manyToOne.Add(many, one);
		}

		public bool Contains(One key)
		{
			return oneToMany.ContainsKey(key);
		}

		public bool Contains(Many key)
		{
			return manyToOne.ContainsKey(key);
		}

		public bool TryGetValue(One key, out List<Many> value)
		{
			return oneToMany.TryGetValue(key, out value);
		}

		public bool TryGetValue(Many key, out One value)
		{
			return manyToOne.TryGetValue(key, out value);
		}
	}

}