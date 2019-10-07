using System;
using System.Collections;
using System.Collections.Generic;

namespace Transistium.Design
{
	public class HandleList<T> : IEnumerable<Handle>
		where T : class
	{
		private List<T> elements;

		private List<int> freeIndices;

		public HandleList()
		{
			elements = new List<T>();
			freeIndices = new List<int>();
		}

		public T this[Handle handle]
		{
			get { return elements[handle.index]; }
		}	

		public Handle Add(T element)
		{
			int index;

			if (freeIndices.Count > 0)
			{
				index = freeIndices[freeIndices.Count - 1];
				elements[index] = element;

				freeIndices.RemoveAt(freeIndices.Count - 1);
			}
			else
			{
				index = elements.Count;
				elements.Add(element);
			}

			return new Handle(index);
		}
		
		public void Remove(Handle handle)
		{
			elements[handle.index] = null;
			freeIndices.Add(handle.index);
		}

		public IEnumerator<Handle> GetEnumerator()
		{
			for (int i = 0; i < elements.Count; ++i)
			{
				if (!freeIndices.Contains(i))
					yield return new Handle(i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

}