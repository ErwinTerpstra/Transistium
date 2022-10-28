using System;
using System.Collections.Generic;
using System.Linq;

namespace Transistium.Util
{
	public class Observer<Key, Value>
	{
		public delegate Value CreateValueHandler(Key key);
		public delegate void DestroyValueHandler(Key key, Value value);

		private CreateValueHandler createHandler;

		private DestroyValueHandler destroyHandler;

		private OneToOneMapping<Key, Value> mapping;

		private IEnumerable<Key> source;

		private List<Key> addedKeys;

		private List<Key> removedKeys;

		public OneToOneMapping<Key, Value> Mapping => mapping;

		public Observer(CreateValueHandler createHandler, DestroyValueHandler destroyHandler)
		{
			this.createHandler = createHandler;
			this.destroyHandler = destroyHandler;

			mapping = new OneToOneMapping<Key, Value>();

			addedKeys = new List<Key>();
			removedKeys = new List<Key>();
		}

		public void Observe(IEnumerable<Key> source, bool replaceExisting = true)
		{
			if (this.source == source)
				return;

			if (!replaceExisting && this.source != null)
				this.source = this.source.Union(source);
			else
				this.source = source;

			DetectChanges();
		}

		public void DetectChanges()
		{
			addedKeys.Clear();
			removedKeys.Clear();

			if (source == null)
				return;

			// Detect added keys
			foreach (var key in source)
			{
				if (!mapping.Contains(key))
					addedKeys.Add(key);
			}

			// Detect removed keys
			foreach (var pair in mapping)
			{
				if (!source.Contains(pair.Key))
					removedKeys.Add(pair.Key);
			}

			// Create values for all added keys
			foreach (var key in addedKeys)
			{
				var value = createHandler(key);

				if (value != null)
					mapping.Map(key, value);
			}

			// Destroy values for all removed keys
			foreach (var key in removedKeys)
			{
				var value = mapping[key];
				mapping.Remove(key);

				destroyHandler(key, value);
			}
		}

	}
}
