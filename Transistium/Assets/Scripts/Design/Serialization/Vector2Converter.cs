using System;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Transistium.Design
{
	public class Vector2Converter : JsonConverter<Vector2>
	{
		public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject token = JToken.ReadFrom(reader) as JObject;

			return new Vector2(token["x"].Value<float>(), token["y"].Value<float>());
		}

		public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
		{
			JObject jsonObject = new JObject();
			jsonObject.Add("x", value.x);
			jsonObject.Add("y", value.y);

			jsonObject.WriteTo(writer);
		}
	}
}
