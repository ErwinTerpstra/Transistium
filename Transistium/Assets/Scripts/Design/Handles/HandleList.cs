using System;
using System.Collections;
using System.Collections.Generic;

using Transistium.Util;

namespace Transistium.Design
{
	[Serializable]
	public class HandleList<T> : ICollection<T> where T : class
	{
		private List<Pair<string, T>> elements;

		public HandleList()
		{
			elements = new List<Pair<string, T>>();
		}

		public T this[Handle<T> handle]
		{
			get
			{
				foreach (var pair in elements)
				{
					if (pair.first == handle.guid)
						return pair.second;
				}

				return null;
			}
		}

		public Handle<T> Add(T element)
		{
			string guid = Guid.NewGuid().ToString();
			elements.Add(new Pair<string, T>(guid, element));

			return new Handle<T>(guid);
		}

		public T Remove(Handle<T> handle)
		{
			for (int i = 0; i < elements.Count; ++i)
			{
				if (elements[i].first == handle.guid)
				{
					var element = elements[i].second;

					elements.RemoveAt(i);

					return element;
				}
			}

			return null;
		}

		public Handle<T> Remove(T element)
		{
			for (int i = 0; i < elements.Count; ++i)
			{
				if (elements[i].second == element)
				{
					var handle = new Handle<T>(elements[i].first);

					elements.RemoveAt(i);

					return handle;
				}
			}

			return Handle<T>.Invalid;
		}
		public bool Contains(T item)
		{
			return LookupHandle(item) != Handle<T>.Invalid;
		}

		public Handle<T> LookupHandle(T element)
		{
			for (int i = 0; i < elements.Count; ++i)
			{
				if (elements[i].second == element)
					return new Handle<T>(elements[i].first);
			}

			return Handle<T>.Invalid;
		}

		public void Clear()
		{
			elements.Clear();
		}


		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < elements.Count; ++i)
				array[arrayIndex + 1] = elements[i].second;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < elements.Count; ++i)
				yield return elements[i].second;
		}

		#region Explicit interface definitions
		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		bool ICollection<T>.Remove(T item)
		{
			return Remove(item) != Handle<T>.Invalid;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		public int Count
		{
			get { return elements.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

	}

}