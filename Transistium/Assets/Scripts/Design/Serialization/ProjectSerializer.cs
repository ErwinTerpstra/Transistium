using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;
using Transistium.Design.Components;

namespace Transistium.Design
{
	public class ProjectSerializer
	{
		private JsonSerializer serializer;

		private string projectFile;
		private string backupFile;

		public ProjectSerializer()
		{
			serializer = new JsonSerializer();
			serializer.Formatting = Formatting.Indented;
			serializer.Converters.Add(new GuidConverter());
			serializer.Converters.Add(new Vector2Converter());
			serializer.ContractResolver = new ProjectContractResolver();

			projectFile = Path.Combine(Application.persistentDataPath, "project.json");
			backupFile = Path.Combine(Application.persistentDataPath, "project.backup.json");
		}

		public void Store(Project project)
		{
			if (File.Exists(projectFile))
			{
				if (File.Exists(backupFile))
					File.Delete(backupFile);

				File.Copy(projectFile, backupFile);
			}

			TextWriter writer = new StreamWriter(new FileStream(projectFile, FileMode.OpenOrCreate));
			serializer.Serialize(writer, project);

			writer.Close();
		}

		public Project Load()
		{
			if (!File.Exists(projectFile))
				return null;

			TextReader textReader = new StreamReader(new FileStream(projectFile, FileMode.Open));
			JsonReader jsonReader = new JsonTextReader(textReader);

			Project project = serializer.Deserialize<Project>(jsonReader);

			return project;
		}
	}
}
