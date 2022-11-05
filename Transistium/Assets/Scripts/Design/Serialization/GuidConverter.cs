using System;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Transistium.Design
{
	public class GuidConverter : JsonConverter<Guid>
	{
		public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JToken token = JToken.ReadFrom(reader);

			return Guid.FromString(token.Value<string>());
		}

		public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}
}
