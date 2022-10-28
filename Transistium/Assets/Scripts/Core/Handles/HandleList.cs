using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Transistium.Util;

using Newtonsoft.Json;
using System.Linq;

namespace Transistium
{
	[Serializable, JsonObject]
	public class HandleList<T> : ICollection<T>, ISerializable 
		where T : class
	{
		public class ElementsList : List<Pair<string, T>> { }

		[JsonProperty]
		private ElementsList elements;

		[JsonProperty]
		private byte hashPrefix;

		public IEnumerable<T> AllElements => elements.Select(pair => pair.second);

		public IEnumerable<Handle<T>> AllHandles => elements.Select(pair => new Handle<T>(pair.first));

		public int Count => elements.Count;

		public bool IsReadOnly => false;

		public HandleList(byte hashPrefix)
		{
			this.hashPrefix = hashPrefix;

			elements = new ElementsList();
		}

		public HandleList(SerializationInfo info, StreamingContext context)
		{
			hashPrefix = (byte) info.GetValue("hashPrefix", typeof(byte));

			var list = info.GetValue("elements", typeof(ElementsList)) as ElementsList;

			elements = new ElementsList();
			elements.AddRange(list);
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

		private void PrefixGuid(ref Guid guid)
		{
			guid[0] = hashPrefix;
		}

		public Handle<T> Add(T element) => Add(Guid.Generate(), element);

		public Handle<T> Add(Guid guid, T element)
		{
			PrefixGuid(ref guid);
			
			string guidString = guid.ToString();

			elements.Add(new Pair<string, T>(guidString, element));

			return new Handle<T>(guidString);
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

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("elements", elements);
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

	}

}