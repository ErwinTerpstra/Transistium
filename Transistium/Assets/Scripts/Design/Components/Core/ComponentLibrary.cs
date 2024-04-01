using System;
using System.Collections.Generic;
using System.Linq;

namespace Transistium.Design.Components
{
	public class ComponentLibrary
	{
		public static readonly ComponentLibrary Default = Create();

		private Dictionary<Guid, Component> components;

		public IEnumerable<Component> AllComponents => components.Values;

		public IEnumerable<Handle<Chip>> AllChips => 
			AllComponents.Select(
				component => new Handle<Chip>(component.Guid)
			);

		public ComponentLibrary()
		{
			components = new Dictionary<Guid, Component>();
		}

		public void RegisterComponent(Component component)
		{
			var guid = component.Guid;
			guid[0] = (byte)ElementType.CHIP;
			
			components.Add(guid, component);
		}

		public Chip FindChip(Guid guid) => FindComponent(guid)?.Chip;

		public Chip FindChip(Handle<Chip> handle) => FindComponent(handle)?.Chip;

		public Component FindComponent(Guid guid)
		{
			guid[0] = (byte)ElementType.CHIP;

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
			library.RegisterComponent(new Switch());

			return library;
		}
	}
}