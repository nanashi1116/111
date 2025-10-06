using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using DynamicMaps.Data;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.Quests;
using HarmonyLib;
using UnityEngine;

namespace DynamicMaps.Utils
{
	// Token: 0x0200000E RID: 14
	public static class QuestUtils
	{
		// Token: 0x0600003F RID: 63 RVA: 0x000031B2 File Offset: 0x000013B2
		static QuestUtils()
		{
			QuestUtils.InitializeReflection();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000031E4 File Offset: 0x000013E4
		private static void InitializeReflection()
		{
			try
			{
				Plugin.Log.LogInfo("Initializing QuestUtils reflection...");
				QuestUtils._playerQuestControllerField = QuestUtils.TryFindField(typeof(Player), new string[]
				{
					"_questController",
					"questController",
					"m_questController",
					"QuestController"
				}, "Player quest controller");
				if (QuestUtils._playerQuestControllerField == null)
				{
					QuestUtils._reflectionFailed = true;
				}
				else
				{
					QuestUtils._questControllerQuestsProperty = QuestUtils.TryFindProperty(typeof(AbstractQuestControllerClass), new string[]
					{
						"Quests",
						"ActiveQuests",
						"PlayerQuests"
					}, "AbstractQuestControllerClass Quests");
					if (QuestUtils._questControllerQuestsProperty == null)
					{
						QuestUtils._reflectionFailed = true;
					}
					else
					{
						QuestUtils._questsListField = QuestUtils.TryFindField(QuestUtils._questControllerQuestsProperty.PropertyType, new string[]
						{
							"List_1",
							"List_0",
							"list_1",
							"list_0",
							"_list",
							"items",
							"_items"
						}, "quests list");
						if (QuestUtils._questsListField == null)
						{
							QuestUtils._reflectionFailed = true;
						}
						else
						{
							QuestUtils._questsGetConditionalMethod = QuestUtils.TryFindMethod(QuestUtils._questControllerQuestsProperty.PropertyType, new string[]
							{
								"GetConditional",
								"GetQuest",
								"FindQuest"
							}, new Type[]
							{
								typeof(string)
							}, "GetConditional method");
							if (QuestUtils._questsGetConditionalMethod == null)
							{
								QuestUtils._reflectionFailed = true;
							}
							else
							{
								Type baseType = QuestUtils._questControllerQuestsProperty.PropertyType.BaseType;
								Type questType;
								if (baseType == null)
								{
									questType = null;
								}
								else
								{
									Type[] genericArguments = baseType.GetGenericArguments();
									questType = ((genericArguments != null) ? genericArguments[0] : null);
								}
								QuestUtils._questType = questType;
								if (QuestUtils._questType == null)
								{
									Plugin.Log.LogError("Failed to determine quest type from generic arguments");
									foreach (string str in new string[]
									{
										"QuestDataClass",
										"Quest",
										"QuestData"
									})
									{
										QuestUtils._questType = (Type.GetType("EFT." + str + ", Assembly-CSharp") ?? Type.GetType("EFT.Quests." + str + ", Assembly-CSharp"));
										if (QuestUtils._questType != null)
										{
											Plugin.Log.LogInfo("Found quest type: " + QuestUtils._questType.FullName);
											break;
										}
									}
									if (QuestUtils._questType == null)
									{
										Plugin.Log.LogError("Failed to find quest type");
										QuestUtils._reflectionFailed = true;
										return;
									}
								}
								QuestUtils._questIsConditionDone = QuestUtils.TryFindMethod(QuestUtils._questType, new string[]
								{
									"IsConditionDone",
									"IsConditionComplete",
									"CheckCondition"
								}, new Type[]
								{
									typeof(Condition)
								}, "IsConditionDone method");
								if (QuestUtils._questIsConditionDone == null)
								{
									QuestUtils._reflectionFailed = true;
								}
								else
								{
									QuestUtils._conditionCounterTemplateField = QuestUtils.TryFindField(typeof(ConditionCounterCreator), new string[]
									{
										"TemplateConditions",
										"_templateConditions",
										"templateConditions",
										"m_templateConditions",
										"Template"
									}, "ConditionCounterCreator template");
									if (QuestUtils._conditionCounterTemplateField == null)
									{
										QuestUtils._reflectionFailed = true;
									}
									else
									{
										QuestUtils._templateConditionsConditionsField = QuestUtils.TryFindField(QuestUtils._conditionCounterTemplateField.FieldType, new string[]
										{
											"Conditions",
											"_conditions",
											"conditions",
											"ConditionsList"
										}, "template conditions");
										if (QuestUtils._templateConditionsConditionsField == null)
										{
											QuestUtils._reflectionFailed = true;
										}
										else
										{
											QuestUtils._conditionListField = QuestUtils.TryFindField(QuestUtils._templateConditionsConditionsField.FieldType, new string[]
											{
												"List_0",
												"List_1",
												"list_0",
												"list_1",
												"_list",
												"items",
												"_items"
											}, "conditions list");
											if (QuestUtils._conditionListField == null)
											{
												QuestUtils._reflectionFailed = true;
											}
											else
											{
												QuestUtils._reflectionInitialized = true;
												Plugin.Log.LogInfo("QuestUtils reflection initialized successfully");
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception during QuestUtils reflection initialization: {0}", arg));
				QuestUtils._reflectionFailed = true;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003648 File Offset: 0x00001848
		private static FieldInfo TryFindField(Type type, string[] possibleNames, string description)
		{
			foreach (string text in possibleNames)
			{
				FieldInfo fieldInfo = AccessTools.Field(type, text);
				if (fieldInfo != null)
				{
					Plugin.Log.LogInfo(string.Concat(new string[]
					{
						"Found ",
						description,
						" field: ",
						type.Name,
						".",
						text
					}));
					return fieldInfo;
				}
			}
			Plugin.Log.LogError(string.Concat(new string[]
			{
				"Failed to find ",
				description,
				" field in ",
				type.Name,
				". Tried: ",
				string.Join(", ", possibleNames)
			}));
			QuestUtils.LogAvailableMembers(type, "fields");
			return null;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003710 File Offset: 0x00001910
		private static PropertyInfo TryFindProperty(Type type, string[] possibleNames, string description)
		{
			foreach (string text in possibleNames)
			{
				PropertyInfo propertyInfo = AccessTools.Property(type, text);
				if (propertyInfo != null)
				{
					Plugin.Log.LogInfo(string.Concat(new string[]
					{
						"Found ",
						description,
						" property: ",
						type.Name,
						".",
						text
					}));
					return propertyInfo;
				}
			}
			Plugin.Log.LogError(string.Concat(new string[]
			{
				"Failed to find ",
				description,
				" property in ",
				type.Name,
				". Tried: ",
				string.Join(", ", possibleNames)
			}));
			QuestUtils.LogAvailableMembers(type, "properties");
			return null;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000037D8 File Offset: 0x000019D8
		private static MethodInfo TryFindMethod(Type type, string[] possibleNames, Type[] parameterTypes, string description)
		{
			foreach (string text in possibleNames)
			{
				MethodInfo methodInfo = AccessTools.Method(type, text, parameterTypes, null);
				if (methodInfo != null)
				{
					Plugin.Log.LogInfo(string.Concat(new string[]
					{
						"Found ",
						description,
						": ",
						type.Name,
						".",
						text
					}));
					return methodInfo;
				}
			}
			Plugin.Log.LogError(string.Concat(new string[]
			{
				"Failed to find ",
				description,
				" in ",
				type.Name,
				". Tried: ",
				string.Join(", ", possibleNames)
			}));
			QuestUtils.LogAvailableMembers(type, "methods");
			return null;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000038A0 File Offset: 0x00001AA0
		private static void LogAvailableMembers(Type type, string memberType)
		{
			try
			{
				Plugin.Log.LogInfo(string.Concat(new string[]
				{
					"Available ",
					memberType,
					" in ",
					type.Name,
					":"
				}));
				string a = memberType.ToLower();
				if (!(a == "fields"))
				{
					if (!(a == "properties"))
					{
						if (a == "methods")
						{
							IEnumerable<string> values = Enumerable.Select<MethodInfo, string>(Enumerable.Take<MethodInfo>(Enumerable.Where<MethodInfo>(type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), (MethodInfo m) => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")), 20), (MethodInfo m) => m.Name);
							Plugin.Log.LogInfo("  " + string.Join(", ", values));
						}
					}
					else
					{
						IEnumerable<string> values2 = Enumerable.Select<PropertyInfo, string>(Enumerable.Take<PropertyInfo>(type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), 20), (PropertyInfo p) => p.Name);
						Plugin.Log.LogInfo("  " + string.Join(", ", values2));
					}
				}
				else
				{
					IEnumerable<string> values3 = Enumerable.Select<FieldInfo, string>(Enumerable.Take<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), 20), (FieldInfo f) => f.Name);
					Plugin.Log.LogInfo("  " + string.Join(", ", values3));
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning(string.Concat(new string[]
				{
					"Failed to log available ",
					memberType,
					" for ",
					type.Name,
					": ",
					ex.Message
				}));
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003AA8 File Offset: 0x00001CA8
		internal static void TryCaptureQuestData()
		{
			if (QuestUtils._reflectionFailed)
			{
				Plugin.Log.LogWarning("QuestUtils reflection failed, skipping quest data capture");
				return;
			}
			try
			{
				GameWorld instance = Singleton<GameWorld>.Instance;
				if (instance == null)
				{
					Plugin.Log.LogWarning("GameWorld instance is null, cannot capture quest data");
				}
				else
				{
					if (QuestUtils.TriggersWithIds == null)
					{
						QuestUtils.TriggersWithIds = Enumerable.ToList<TriggerWithId>(Object.FindObjectsOfType<TriggerWithId>());
						Plugin.Log.LogInfo(string.Format("Found {0} trigger zones", QuestUtils.TriggersWithIds.Count));
					}
					if (QuestUtils.QuestItems == null)
					{
						List<LootItem> value = Traverse.Create(instance).Field("LootItems").Field("list_0").GetValue<List<LootItem>>();
						if (value != null)
						{
							QuestUtils.QuestItems = Enumerable.ToList<LootItem>(Enumerable.Where<LootItem>(value, delegate(LootItem i)
							{
								if (i == null)
								{
									return false;
								}
								Item item = i.Item;
								return ((item != null) ? new bool?(item.QuestItem) : null).GetValueOrDefault();
							}));
							Plugin.Log.LogInfo(string.Format("Found {0} quest items", QuestUtils.QuestItems.Count));
						}
						else
						{
							Plugin.Log.LogWarning("LootItems list is null, trying alternative field names");
							foreach (string text in new string[]
							{
								"List_0",
								"List_1",
								"list_1",
								"_lootItems",
								"lootItems"
							})
							{
								value = Traverse.Create(instance).Field("LootItems").Field(text).GetValue<List<LootItem>>();
								if (value != null)
								{
									Plugin.Log.LogInfo("Found LootItems using field name: " + text);
									QuestUtils.QuestItems = Enumerable.ToList<LootItem>(Enumerable.Where<LootItem>(value, delegate(LootItem i)
									{
										if (i == null)
										{
											return false;
										}
										Item item = i.Item;
										return ((item != null) ? new bool?(item.QuestItem) : null).GetValueOrDefault();
									}));
									Plugin.Log.LogInfo(string.Format("Found {0} quest items", QuestUtils.QuestItems.Count));
									break;
								}
							}
							if (value == null)
							{
								Plugin.Log.LogError("Could not find LootItems with any known field name");
								QuestUtils.QuestItems = new List<LootItem>();
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception in TryCaptureQuestData: {0}", arg));
				if (QuestUtils.TriggersWithIds == null)
				{
					QuestUtils.TriggersWithIds = new List<TriggerWithId>();
				}
				if (QuestUtils.QuestItems == null)
				{
					QuestUtils.QuestItems = new List<LootItem>();
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003D04 File Offset: 0x00001F04
		internal static void DiscardQuestData()
		{
			if (QuestUtils.TriggersWithIds != null)
			{
				QuestUtils.TriggersWithIds.Clear();
				QuestUtils.TriggersWithIds = null;
			}
			if (QuestUtils.QuestItems != null)
			{
				QuestUtils.QuestItems.Clear();
				QuestUtils.QuestItems = null;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003D34 File Offset: 0x00001F34
		internal static IEnumerable<MapMarkerDef> GetMarkerDefsForPlayer(Player player)
		{
			if (QuestUtils._reflectionFailed)
			{
				Plugin.Log.LogWarning("QuestUtils reflection failed, cannot get marker definitions");
				return new List<MapMarkerDef>();
			}
			if (QuestUtils.TriggersWithIds == null || QuestUtils.QuestItems == null)
			{
				Plugin.Log.LogWarning(string.Format("TriggersWithIds null: {0} or QuestItems null: {1}", QuestUtils.TriggersWithIds == null, QuestUtils.QuestItems == null));
				return new List<MapMarkerDef>();
			}
			List<MapMarkerDef> list = new List<MapMarkerDef>();
			foreach (QuestDataClass quest in QuestUtils.GetIncompleteQuests(player))
			{
				list.AddRange(QuestUtils.GetMarkerDefsForQuest(player, quest));
			}
			return list;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003DEC File Offset: 0x00001FEC
		internal static IEnumerable<MapMarkerDef> GetMarkerDefsForQuest(Player player, QuestDataClass quest)
		{
			List<MapMarkerDef> list = new List<MapMarkerDef>();
			foreach (Condition condition in QuestUtils.GetIncompleteQuestConditions(player, quest))
			{
				string questName = quest.Template.NameLocaleKey.BSGLocalized();
				string conditionDescription = condition.id.BSGLocalized();
				foreach (Vector3 vector in QuestUtils.GetPositionsForCondition(condition, questName, conditionDescription))
				{
					bool flag = false;
					foreach (MapMarkerDef mapMarkerDef in list)
					{
						if (MathUtils.ApproxEquals(mapMarkerDef.Position.x, vector.x) && MathUtils.ApproxEquals(mapMarkerDef.Position.y, vector.y) && MathUtils.ApproxEquals(mapMarkerDef.Position.z, vector.z))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(QuestUtils.CreateQuestMapMarkerDef(vector, questName, conditionDescription));
					}
				}
			}
			return list;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003F64 File Offset: 0x00002164
		private static IEnumerable<Vector3> GetPositionsForCondition(Condition condition, string questName, string conditionDescription)
		{
			List<Vector3> list = new List<Vector3>();
			ConditionZone conditionZone = condition as ConditionZone;
			if (conditionZone == null)
			{
				ConditionLaunchFlare conditionLaunchFlare = condition as ConditionLaunchFlare;
				if (conditionLaunchFlare == null)
				{
					ConditionVisitPlace conditionVisitPlace = condition as ConditionVisitPlace;
					if (conditionVisitPlace == null)
					{
						ConditionInZone conditionInZone = condition as ConditionInZone;
						if (conditionInZone == null)
						{
							ConditionFindItem conditionFindItem = condition as ConditionFindItem;
							if (conditionFindItem == null)
							{
								ConditionExitName <exitCondition>5__2 = condition as ConditionExitName;
								if (<exitCondition>5__2 == null)
								{
									ConditionCounterCreator conditionCounterCreator = condition as ConditionCounterCreator;
									if (conditionCounterCreator != null)
									{
										list.AddRange(QuestUtils.GetPositionsForConditionCreator(conditionCounterCreator, questName, conditionDescription));
									}
								}
								else
								{
									ExfiltrationPoint exfiltrationPoint = Enumerable.FirstOrDefault<ExfiltrationPoint>(Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints, (ExfiltrationPoint e) => e.Settings.Name == <exitCondition>5__2.exitName);
									if (exfiltrationPoint != null)
									{
										list.Add(MathUtils.ConvertToMapPosition(exfiltrationPoint.transform));
									}
								}
							}
							else
							{
								list.AddRange(QuestUtils.GetPositionsForQuestItems(conditionFindItem.target, questName, conditionDescription));
							}
						}
						else
						{
							foreach (string zoneId in conditionInZone.zoneIds)
							{
								list.AddRange(QuestUtils.GetPositionsForZoneId(zoneId, questName, conditionDescription));
							}
						}
					}
					else
					{
						list.AddRange(QuestUtils.GetPositionsForZoneId(conditionVisitPlace.target, questName, conditionDescription));
					}
				}
				else
				{
					list.AddRange(QuestUtils.GetPositionsForZoneId(conditionLaunchFlare.zoneID, questName, conditionDescription));
				}
			}
			else
			{
				list.AddRange(QuestUtils.GetPositionsForZoneId(conditionZone.zoneId, questName, conditionDescription));
			}
			return list;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000040CC File Offset: 0x000022CC
		private static IEnumerable<Vector3> GetPositionsForConditionCreator(ConditionCounterCreator conditionCreator, string questName, string conditionDescription)
		{
			if (QuestUtils._reflectionFailed)
			{
				Plugin.Log.LogWarning("QuestUtils reflection failed, cannot get positions for condition creator");
				return new List<Vector3>();
			}
			List<Vector3> list = new List<Vector3>();
			try
			{
				object value = QuestUtils._conditionCounterTemplateField.GetValue(conditionCreator);
				object value2 = QuestUtils._templateConditionsConditionsField.GetValue(value);
				IList<Condition> list2 = QuestUtils._conditionListField.GetValue(value2) as IList<Condition>;
				if (list2 != null)
				{
					foreach (Condition condition in list2)
					{
						list.AddRange(QuestUtils.GetPositionsForCondition(condition, questName, conditionDescription));
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception in GetPositionsForConditionCreator: {0}", arg));
			}
			return list;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000419C File Offset: 0x0000239C
		private static IEnumerable<Vector3> GetPositionsForZoneId(string zoneId, string questName, string conditionDescription)
		{
			List<Vector3> list = new List<Vector3>();
			List<TriggerWithId> triggersWithIds = QuestUtils.TriggersWithIds;
			IEnumerable<TriggerWithId> enumerable = (triggersWithIds != null) ? triggersWithIds.GetZoneTriggers(zoneId) : null;
			if (enumerable != null)
			{
				foreach (TriggerWithId triggerWithId in enumerable)
				{
					list.Add(MathUtils.ConvertToMapPosition(triggerWithId.transform.position));
				}
			}
			return list;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004210 File Offset: 0x00002410
		private static IEnumerable<Vector3> GetPositionsForQuestItems(IEnumerable<string> questItemIds, string questName, string conditionDescription)
		{
			List<Vector3> list = new List<Vector3>();
			using (IEnumerator<string> enumerator = questItemIds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string questItemId = enumerator.Current;
					List<LootItem> questItems = QuestUtils.QuestItems;
					IEnumerable<LootItem> enumerable = (questItems != null) ? Enumerable.Where<LootItem>(questItems, (LootItem i) => i.TemplateId == questItemId) : null;
					if (enumerable != null)
					{
						foreach (LootItem lootItem in enumerable)
						{
							list.Add(MathUtils.ConvertToMapPosition(lootItem.transform.position));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000042D4 File Offset: 0x000024D4
		private static IEnumerable<Condition> GetIncompleteQuestConditions(Player player, QuestDataClass quest)
		{
			List<Condition> list = new List<Condition>();
			bool flag;
			if (quest == null)
			{
				flag = (null != null);
			}
			else
			{
				RawQuestClass template = quest.Template;
				flag = (((template != null) ? template.Conditions : null) != null);
			}
			if (!flag)
			{
				Plugin.Log.LogError("GetIncompleteQuestConditions: quest.Template.Conditions is null, skipping quest");
				return list;
			}
			GClass1631 gclass;
			if (!quest.Template.Conditions.TryGetValue(EQuestStatus.AvailableForFinish, out gclass) || gclass == null)
			{
				Plugin.Log.LogError("Quest " + quest.Template.NameLocaleKey.BSGLocalized() + " doesn't have conditions marked AvailableForFinish, skipping it");
				return list;
			}
			foreach (Condition condition in gclass)
			{
				if (condition == null)
				{
					Plugin.Log.LogWarning("Quest " + quest.Template.NameLocaleKey.BSGLocalized() + " has null condition, skipping it");
				}
				else if (!QuestUtils.IsConditionCompleted(player, quest, condition))
				{
					list.Add(condition);
				}
			}
			return list;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000043CC File Offset: 0x000025CC
		private static IEnumerable<QuestDataClass> GetIncompleteQuests(Player player)
		{
			if (QuestUtils._reflectionFailed)
			{
				Plugin.Log.LogWarning("QuestUtils reflection failed, cannot get incomplete quests");
				return new List<QuestDataClass>();
			}
			List<QuestDataClass> list = new List<QuestDataClass>();
			try
			{
				object value = QuestUtils._playerQuestControllerField.GetValue(player);
				if (value == null)
				{
					Plugin.Log.LogError(string.Format("Not able to get quests for player: {0}, questController is null", player.Id));
					return list;
				}
				object value2 = QuestUtils._questControllerQuestsProperty.GetValue(value);
				if (value2 == null)
				{
					Plugin.Log.LogError(string.Format("Not able to get quests for player: {0}, quests is null", player.Id));
					return list;
				}
				List<QuestDataClass> list2 = QuestUtils._questsListField.GetValue(value2) as List<QuestDataClass>;
				if (list2 == null)
				{
					Plugin.Log.LogError(string.Format("Not able to get quests for player: {0}, questsList is null", player.Id));
					return list;
				}
				foreach (QuestDataClass questDataClass in list2)
				{
					bool flag;
					if (questDataClass == null)
					{
						flag = (null != null);
					}
					else
					{
						RawQuestClass template = questDataClass.Template;
						flag = (((template != null) ? template.Conditions : null) != null);
					}
					if (flag && questDataClass.Status == EQuestStatus.Started)
					{
						list.Add(questDataClass);
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception in GetIncompleteQuests: {0}", arg));
			}
			return list;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004550 File Offset: 0x00002750
		private static bool IsConditionCompleted(Player player, QuestDataClass questData, Condition condition)
		{
			if (QuestUtils._reflectionFailed)
			{
				Plugin.Log.LogWarning("QuestUtils reflection failed, cannot check condition completion");
				return false;
			}
			bool result;
			try
			{
				if (condition.IsNecessary && !questData.CompletedConditions.Contains(condition.id))
				{
					result = false;
				}
				else
				{
					object value = QuestUtils._playerQuestControllerField.GetValue(player);
					if (value == null)
					{
						result = false;
					}
					else
					{
						object value2 = QuestUtils._questControllerQuestsProperty.GetValue(value);
						if (value2 == null)
						{
							result = false;
						}
						else
						{
							object obj = QuestUtils._questsGetConditionalMethod.Invoke(value2, new object[]
							{
								questData.Id
							});
							if (obj == null)
							{
								result = false;
							}
							else
							{
								result = (bool)QuestUtils._questIsConditionDone.Invoke(obj, new object[]
								{
									condition
								});
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception in IsConditionCompleted: {0}", arg));
				result = false;
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000462C File Offset: 0x0000282C
		private static IEnumerable<TriggerWithId> GetZoneTriggers(this IEnumerable<TriggerWithId> triggerWithIds, string zoneId)
		{
			if (triggerWithIds == null)
			{
				return new List<TriggerWithId>();
			}
			return Enumerable.Where<TriggerWithId>(triggerWithIds, (TriggerWithId t) => t.Id == zoneId);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004664 File Offset: 0x00002864
		private static MapMarkerDef CreateQuestMapMarkerDef(Vector3 position, string questName, string conditionDescription)
		{
			return new MapMarkerDef
			{
				Category = "Quest",
				Color = QuestUtils._questColor,
				ImagePath = "Markers/quest.png",
				Position = position,
				Pivot = QuestUtils._questPivot,
				Text = questName
			};
		}

		// Token: 0x04000034 RID: 52
		private static FieldInfo _playerQuestControllerField;

		// Token: 0x04000035 RID: 53
		private static PropertyInfo _questControllerQuestsProperty;

		// Token: 0x04000036 RID: 54
		private static FieldInfo _questsListField;

		// Token: 0x04000037 RID: 55
		private static MethodInfo _questsGetConditionalMethod;

		// Token: 0x04000038 RID: 56
		private static Type _questType;

		// Token: 0x04000039 RID: 57
		private static MethodInfo _questIsConditionDone;

		// Token: 0x0400003A RID: 58
		private static FieldInfo _conditionCounterTemplateField;

		// Token: 0x0400003B RID: 59
		private static FieldInfo _templateConditionsConditionsField;

		// Token: 0x0400003C RID: 60
		private static FieldInfo _conditionListField;

		// Token: 0x0400003D RID: 61
		private static bool _reflectionInitialized = false;

		// Token: 0x0400003E RID: 62
		private static bool _reflectionFailed = false;

		// Token: 0x0400003F RID: 63
		private const string _questCategory = "Quest";

		// Token: 0x04000040 RID: 64
		private const string _questImagePath = "Markers/quest.png";

		// Token: 0x04000041 RID: 65
		private static Vector2 _questPivot = new Vector2(0.5f, 0f);

		// Token: 0x04000042 RID: 66
		private static Color _questColor = Color.green;

		// Token: 0x04000043 RID: 67
		public static List<TriggerWithId> TriggersWithIds;

		// Token: 0x04000044 RID: 68
		public static List<LootItem> QuestItems;
	}
}
