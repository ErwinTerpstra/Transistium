using System;
using System.Collections.Generic;
using System.Linq;

namespace Transistium.Design.Components
{
	public class ComponentLibrary
	{
		public static readonly ComponentLibrary Default = Create();

		private Dictionary<string, Component> components;

		public IEnumerable<Component> AllComponents => components.Values;

		public IEnumerable<Handle<Chip>> AllChips => 
			AllComponents.Select(
				component => new Handle<Chip>(component.Guid.ToString())
			);

		public ComponentLibrary()
		{
			components = new Dictionary<string, Component>();
		}

		public void RegisterComponent(Component component)
		{
			components.Add(component.Guid.ToString(), component);
		}

		public Chip FindChip(string guid) => FindComponent(guid)?.Chip;

		public Chip FindChip(Handle<Chip> handle) => FindComponent(handle)?.Chip;

		public Component FindComponent(string guid)
		{
			if (components.TryGetValue(guid, out Component component))
				return component;
			else
				return null;
		}

		public Component FindComponent(Handle<Chip> handle)
		{
			return FindComponent(handle.guid);
		}

		public static ComponentLibrary Create()
		{
			var library = new ComponentLibrary();
			library.RegisterComponent(new Button());

			return library;
		}
	}
}