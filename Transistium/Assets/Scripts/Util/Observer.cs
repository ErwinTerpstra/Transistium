using System;
using System.Collections.Generic;

namespace Transistium.Util
{
	public class Observer<Key, Value>
	{
		public delegate Value CreateValueHandler(Key key);
		public delegate void DestroyValueHandler(Key key, Value value);

		private CreateValueHandler createHandler;

		private DestroyValueHandler destroyHandler;

		private OneToOneMapping<Key, Value> mapping;

		private ICollection<Key> source;

		private List<Key> addedKeys;

		private List<Key> removedKeys;

		public Observer(CreateValueHandler createHandler, DestroyValueHandler destroyHandler)
		{
			this.createHandler = createHandler;
			this.destroyHandler = destroyHandler;

			mapping = new OneToOneMapping<Key, Value>();

			addedKeys = new List<Key>();
			removedKeys = new List<Key>();
		}

		public void Observe(ICollection<Key> source)
		{
			if (this.source == source)
				return;

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

		public OneToOneMapping<Key, Value> Mapping
		{
			get { return mapping; }
		}

	}
}
