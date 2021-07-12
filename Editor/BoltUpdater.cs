using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

#if BOLT_1_3_OR_NEWER
using Photon.Bolt;
using Photon.Bolt.Editor.Utils;
using Photon.Bolt.LagCompensation;
using Photon.Bolt.Utils;
#else
using Bolt;
using Bolt.Editor.Utils;
using Bolt.LagCompensation;
#endif

namespace Photon.Bolt.Utils
{
	/// <summary>
	/// This is an utility class used only the convert entities from Bolt v1.2.x to v1.3
	/// </summary>
	public static class BoltUpdater
	{
		// --------------- PUBLIC MEMBERS -----------------------------------------------------------------------------------

		[MenuItem("Photon Bolt/Utils/Update/Save Prefabs", priority = 130)]
		public static void SerializePrefabs()
		{
			BoltLog.Info("Serializing from version: {0}", BoltNetwork.Version);

			if (VerifyVersion(true))
			{
				Save();
			}
			else
			{
				BoltLog.Error("Minimal Bolt SDK to update from is v1.2.14");
			}
		}

		[MenuItem("Photon Bolt/Utils/Update/Load Prefabs", priority = 131)]
		public static void DesealizePrefabs()
		{
			BoltLog.Info("Deserializing from version: {0}", BoltNetwork.Version);

			if (VerifyVersion(false))
			{
				Load();
			}
			else
			{
				BoltLog.Error("Minimal Bolt SDK to update to is v1.3.0");
			}
		}

		// --------------- PRIVATE MEMBERS ----------------------------------------------------------------------------------

		[MenuItem("Photon Bolt/Utils/Update/Save Prefabs", true)]
		private static bool ValidateSerializePrefabs()
		{
			return VerifyVersion(true);
		}

		[MenuItem("Photon Bolt/Utils/Update/Load Prefabs", true)]
		private static bool ValidateDesealizePrefabs()
		{
			return VerifyVersion(false);
		}

		/// <summary>
		/// Path where the serialized data from the entities will be saved
		/// </summary>
		private static string SerializedDataPath
		{
			get
			{
				return BoltPathUtility.BuildPath(BoltPathUtility.ResourcesPath, "entity_data.json");
			}
		}

		/// <summary>
		/// It will perform all necessary steps to serialize and save all configs from the entities on the current project
		/// </summary>
		private static void Save()
		{
			var prefabList = GetPrefabListFromBolt();

			if (prefabList != null && prefabList.Count != 0)
			{
				var updateList = new List<UpdateItem>();

				foreach (var prefabID in prefabList)
				{
					var go = PrefabDatabase.Find(prefabID);

					if (go != null)
					{
						var updateItem = BuildUpdateItem(go);
						if (updateItem != null)
						{
							updateList.Add(updateItem);
						}
					}
				}

				JsonSerializerUpdateUtils.SaveData(updateList, SerializedDataPath);

				BoltLog.Info("Save DONE!");
			}
			else
			{
				BoltLog.Warn("No prefabs found to be serialized");
			}
		}

		/// <summary>
		/// It will perform all steps to load the serialized data on re-config the entities on the project
		/// </summary>
		private static void Load()
		{
			var result = JsonSerializerUpdateUtils.LoadData(SerializedDataPath);

			if (result != null)
			{
				var prefabList = GetPrefabListFromProject();

				foreach (var prefab in prefabList)
				{
					foreach (var updateItem in result)
					{
						if (prefab.name.Equals(updateItem.name))
						{
							UpdateBoltEntity(prefab, updateItem);
						}
					}
				}

				BoltLog.Info("Load DONE!");
			}
			else
			{
				BoltLog.Error("Unable to load data");
			}
		}

		/// <summary>
		/// Based on a UpdateItem, this method will update it's settings with any Bolt related data
		/// </summary>
		/// <param name="target">Target GameObject that should represent a Bolt Entity</param>
		/// <param name="item">Configuration data stored into a UpdateItem</param>
		private static void UpdateBoltEntity(GameObject target, UpdateItem item)
		{
			if (target.name.Equals(item.name) == false) { return; }

			BoltLog.Info("Updating {0}", target.name);

			foreach (var entityComponent in item.entityComponents)
			{
				var boltEntityComponent = target.AddComponent<BoltEntity>();
				var entityModify = boltEntityComponent.ModifySettings();

				entityModify.prefabId = entityComponent.prefabId;
				entityModify.sceneId = entityComponent.sceneId;
				entityModify.serializerId = entityComponent.serializerId;
				entityModify.updateRate = entityComponent.updateRate;
				entityModify.autoFreezeProxyFrames = entityComponent.autoFreezeProxyFrames;
				entityModify.clientPredicted = entityComponent.clientPredicted;
				entityModify.allowInstantiateOnClient = entityComponent.allowInstantiateOnClient;
				entityModify.persistThroughSceneLoads = entityComponent.persistThroughSceneLoads;
				entityModify.sceneObjectDestroyOnDetach = entityComponent.sceneObjectDestroyOnDetach;
				entityModify.sceneObjectAutoAttach = entityComponent.sceneObjectAutoAttach;
				entityModify.alwaysProxy = entityComponent.alwaysProxy;

				EditorUtils.MarkDirty((Component)boltEntityComponent);
			}

			if (item.hasEntityHitboxBody)
			{
				var hitboxComponent = target.AddComponent<BoltHitboxBody>();

				EditorUtils.MarkDirty(hitboxComponent);
			}

			foreach (var hitBoxData in item.entityHitboxes)
			{
				var hitBoxComponent = target.AddComponent<BoltHitbox>();

				hitBoxComponent.hitboxShape = hitBoxData.hitboxShape;
				hitBoxComponent.hitboxType = hitBoxData.hitboxType;
				hitBoxComponent.hitboxSphereRadius = hitBoxData.hitboxSphereRadius;
				hitBoxComponent.hitboxCenter = hitBoxData.hitboxCenter;
				hitBoxComponent.hitboxBoxSize = hitBoxData.hitboxBoxSize;

				EditorUtils.MarkDirty(hitBoxComponent);
			}

			foreach (var childItem in item.childItems)
			{
				for (int i = 0; i < target.transform.childCount; i++)
				{
					var childTransform = target.transform.GetChild(i);

					if (childTransform.name.Equals(childItem.name))
					{
						UpdateBoltEntity(childTransform.gameObject, childItem);
					}
				}
			}

			EditorUtils.MarkDirty(target);
		}

		/// <summary>
		/// Build a UpdateItem based on a GameObject that should represent a BoltEntity
		/// </summary>
		/// <param name="go">GameObject as a BoltEntity</param>
		/// <returns>UpdateItem with all settings from the BoltEntity, or null if none was found</returns>
		private static UpdateItem BuildUpdateItem(GameObject go)
		{
			var item = new UpdateItem
			{
				name = go.name,
				entityComponents = new List<BoltEntitySettingsModifier>(),
				entityHitboxes = new List<BoltHitbox>(),
				childItems = new List<UpdateItem>()
			};

			foreach (var itemComponent in go.GetComponents<BoltEntity>())
			{
				item.entityComponents.Add(itemComponent.ModifySettings());
			}

			item.hasEntityHitboxBody = go.GetComponents<BoltHitboxBody>().Length > 0;
			item.entityHitboxes.AddRange(go.GetComponents<BoltHitbox>());

			for (int i = 0; i < go.transform.childCount; i++)
			{
				var childGO = go.transform.GetChild(i);

				var itemChild = BuildUpdateItem(childGO.gameObject);
				if (itemChild != null)
				{
					item.childItems.Add(itemChild);
				}
			}

			if (item.entityComponents.Count == 0 &&
							item.hasEntityHitboxBody == false &&
							item.entityHitboxes.Count == 0 &&
							item.childItems.Count == 0)
			{
				return null;
			}

			return item;
		}

		/// <summary>
		/// Check the current versio of the Bolt SDK
		/// Min version to update FROM is v1.2.14
		/// Min version to update TO is v1.3.0
		/// </summary>
		/// <param name="export">Flag to signal if the code will try to export or import</param>
		/// <returns>True if it's a value version based on the Export case</returns>
		private static bool VerifyVersion(bool export)
		{
			var currentVersion = BoltNetwork.Version;

			var major = currentVersion.Major;
			var minor = currentVersion.Minor;
			var build = currentVersion.Build;

			if (export)
			{
				if (major == 1 && minor == 2 && build >= 10)
				{
					return true;
				}
			}
			else
			{
				if (major == 1 && minor == 3 && build >= 0)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get a list of all Prefab on the project
		/// </summary>
		/// <returns>List of prefabs</returns>
		private static List<GameObject> GetPrefabListFromProject()
		{
			var prefabList = new List<GameObject>();

			var folders = new string[] { "Assets" };
			var iter = AssetDatabase.FindAssets("t:Prefab", folders).GetEnumerator();

			while (iter.MoveNext())
			{
				var guid = (string)iter.Current;
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

				prefabList.Add(go);
			}
			return prefabList;
		}

		/// <summary>
		/// Get a list of all prefabs registered on Bolt
		/// </summary>
		/// <returns>List of prefabs</returns>
		private static List<PrefabId> GetPrefabListFromBolt()
		{
			var output = new List<PrefabId>();
			var fieldList = typeof(BoltPrefabs).GetFields(BindingFlags.Public | BindingFlags.Static);

			foreach (var item in fieldList)
			{
				if (typeof(PrefabId).IsAssignableFrom(item.FieldType))
				{
					var id = (PrefabId)item.GetValue(null);
					output.Add(id);
				}
			}

			return output;
		}

		/// <summary>
		/// Class that will store all information from a Bolt Entity while updating the SDK
		/// This serves as an intermediary media to update the entity settings and components
		/// </summary>
		private class UpdateItem
		{
			public string name;
			public List<BoltEntitySettingsModifier> entityComponents;
			public bool hasEntityHitboxBody;
			public List<BoltHitbox> entityHitboxes;
			public List<UpdateItem> childItems;
		}

		private static class JsonSerializerUpdateUtils
		{
			private static readonly JsonSerializer serializer;

			static JsonSerializerUpdateUtils()
			{
				serializer = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					TypeNameHandling = TypeNameHandling.None,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					NullValueHandling = NullValueHandling.Ignore,
					DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					PreserveReferencesHandling = PreserveReferencesHandling.None,
					ObjectCreationHandling = ObjectCreationHandling.Auto
				};

				serializer.ContractResolver = new ShouldSerializeContractResolver();
				serializer.Converters.Add(new BoltEntityCreationConverter());
			}

			/// <summary>
			/// Serialize a list of UpdateItem into a JSON file
			/// </summary>
			/// <param name="items">UpdateItem list to save</param>
			/// <param name="path">Path to save the file</param>
			/// <returns>True if all went well, false otherwise</returns>
			public static bool SaveData(List<UpdateItem> items, string path)
			{
				try
				{
					using (var sw = new StreamWriter(path))
					{
						using (JsonWriter writer = new JsonTextWriter(sw))
						{
							serializer.Serialize(writer, items, typeof(List<UpdateItem>));
						}
					}

					return true;
				}
				catch (Exception e)
				{
					BoltLog.Error(e);
					BoltLog.Error(string.Format("Error while serializing Bolt Entities to file at {0}", path));
				}

				return false;
			}

			/// <summary>
			/// Load data from a JSON into a list of UpdateItem
			/// </summary>
			/// <param name="path">Path of the JSON file</param>
			/// <returns>List of UpdateItem</returns>
			public static List<UpdateItem> LoadData(string path)
			{
				List<UpdateItem> result = null;

				try
				{
					using (var sw = new StreamReader(path))
					{
						using (JsonReader reader = new JsonTextReader(sw))
						{
							result = (List<UpdateItem>)serializer.Deserialize(reader, typeof(List<UpdateItem>));
						}
					}
				}
				catch (Exception e)
				{
					BoltLog.Error(e);
					BoltLog.Error(string.Format("Error while deserializing Bolt Entities to file at {0}", path));

					result = null;
				}

				return result;
			}

			/// <summary>
			/// Serializer solver
			/// </summary>
			public class ShouldSerializeContractResolver : DefaultContractResolver
			{
				protected override JsonContract CreateContract(Type objectType)
				{
					var jsonContract = base.CreateContract(objectType);

					if (typeof(BoltHitbox).IsAssignableFrom(objectType))
					{
						jsonContract.Converter = new HitBoxConverter();
					}

					if (typeof(UniqueId).IsAssignableFrom(objectType))
					{
						jsonContract.Converter = new UniqueIdConverter();
					}

					return jsonContract;
				}
			}

			/// <summary>
			/// Converter for BoltEntity component
			/// </summary>
			public class BoltEntityCreationConverter : CustomCreationConverter<BoltEntitySettingsModifier>
			{
				public override BoltEntitySettingsModifier Create(Type objectType)
				{
					var go = new GameObject
					{
						hideFlags = HideFlags.HideAndDontSave
					};
					var entity = go.AddComponent<BoltEntity>();

					return entity.ModifySettings();
				}
			}

			/// <summary>
			/// Converted for UniqueId
			/// </summary>
			public class UniqueIdConverter : JsonConverter
			{
				private const string PropertyName = "id";

				public override bool CanConvert(Type objectType)
				{
					return typeof(UniqueId).IsAssignableFrom(objectType);
				}

				public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
				{
					if (reader.TokenType == JsonToken.StartObject)
					{
						JObject item = JObject.Load(reader);
						return new UniqueId(item[PropertyName].Value<string>());
					}

					return null;
				}

				public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
				{
					var id = (UniqueId)value;

					JObject @object = new JObject(
							new JProperty(PropertyName, id.IdString)
					);

					@object.WriteTo(writer);
				}
			}

			/// <summary>
			/// Converted for Bolt HitBox
			/// </summary>
			public class HitBoxConverter : JsonConverter
			{
				private const string PropHitBoxShape = "hitboxShape";
				private const string PropHitBoxType = "hitboxType";
				private const string PropHitBoxSphereRadius = "hitboxSphereRadius";
				private const string PropHitBoxCenter = "hitboxCenter";
				private const string PropHitBoxSize = "hitboxBoxSize";
				private const string PropX = "x";
				private const string PropY = "y";
				private const string PropZ = "z";

				public override bool CanConvert(Type objectType)
				{
					return typeof(BoltHitbox).IsAssignableFrom(objectType);
				}

				public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
				{
					if (reader.TokenType == JsonToken.StartObject)
					{
						var go = new GameObject
						{
							hideFlags = HideFlags.HideAndDontSave
						};

						var hitBox = go.AddComponent<BoltHitbox>();

						JObject item = JObject.Load(reader);

						hitBox.hitboxShape = (BoltHitboxShape)item[PropHitBoxShape].Value<int>();
						hitBox.hitboxType = (BoltHitboxType)item[PropHitBoxType].Value<int>();
						hitBox.hitboxSphereRadius = item[PropHitBoxSphereRadius].Value<float>();

						hitBox.hitboxCenter = new Vector3(
								item[PropHitBoxCenter][PropX].Value<float>(),
								item[PropHitBoxCenter][PropY].Value<float>(),
								item[PropHitBoxCenter][PropZ].Value<float>()
						);

						hitBox.hitboxBoxSize = new Vector3(
								item[PropHitBoxSize][PropX].Value<float>(),
								item[PropHitBoxSize][PropY].Value<float>(),
								item[PropHitBoxSize][PropZ].Value<float>()
						);

						return hitBox;
					}

					return null;
				}

				public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
				{
					BoltHitbox hitbox = (BoltHitbox)value;

					JObject hitboxObject = new JObject
								{
										new JProperty(PropHitBoxShape, hitbox.hitboxShape),
										new JProperty(PropHitBoxType, hitbox.hitboxType),
										new JProperty(PropHitBoxSphereRadius, hitbox.hitboxSphereRadius),
										new JProperty(PropHitBoxCenter, new JObject {
												new JProperty(PropX, hitbox.hitboxCenter.x),
												new JProperty(PropY, hitbox.hitboxCenter.y),
												new JProperty(PropZ, hitbox.hitboxCenter.z)
										}),
										new JProperty(PropHitBoxSize, new JObject {
												new JProperty(PropX, hitbox.hitboxBoxSize.x),
												new JProperty(PropY, hitbox.hitboxBoxSize.y),
												new JProperty(PropZ, hitbox.hitboxBoxSize.z)
										})
								};

					hitboxObject.WriteTo(writer);
				}
			}
		}

		private static class EditorUtils
		{
			public static bool HasNewPrefabSystem { get; set; }

			static EditorUtils()
			{
				HasNewPrefabSystem =
				Application.unityVersion.StartsWith("5.", StringComparison.OrdinalIgnoreCase) == false &&
				Application.unityVersion.StartsWith("2017.", StringComparison.OrdinalIgnoreCase) == false &&
				Application.unityVersion.StartsWith("2018.0.", StringComparison.OrdinalIgnoreCase) == false &&
				Application.unityVersion.StartsWith("2018.1.", StringComparison.OrdinalIgnoreCase) == false &&
				Application.unityVersion.StartsWith("2018.2.", StringComparison.OrdinalIgnoreCase) == false;
			}

			internal static void MarkDirty(GameObject gameObject)
			{
				MarkDirty(gameObject, gameObject.scene);
			}

			internal static void MarkDirty(Component component)
			{
				MarkDirty(component, component.gameObject.scene);
			}

			private static void MarkDirty(UnityEngine.Object obj, UnityEngine.SceneManagement.Scene scene)
			{
				UnityEditor.EditorUtility.SetDirty(obj);

				if (HasNewPrefabSystem == true && Application.isPlaying == false)
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
					UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
				}
			}
		}
	}
}
