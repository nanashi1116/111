using System;
using System.Collections.Generic;
using System.IO;
using Comfort.Common;
using DynamicMaps.Data;
using EFT;
using EFT.Interactive;
using EFT.Interactive.SecretExfiltrations;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicMaps.Utils
{
	// Token: 0x0200000A RID: 10
	public static class DumpUtils
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000025DC File Offset: 0x000007DC
		public static void DumpExtracts()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			ScavExfiltrationPoint[] scavExfiltrationPoints = instance.ExfiltrationController.ScavExfiltrationPoints;
			ExfiltrationPoint[] exfiltrationPoints = instance.ExfiltrationController.ExfiltrationPoints;
			SecretExfiltrationPoint[] secretExfiltrationPoints = instance.ExfiltrationController.SecretExfiltrationPoints;
			List<MapMarkerDef> list = new List<MapMarkerDef>();
			foreach (ScavExfiltrationPoint scavExfiltrationPoint in scavExfiltrationPoints)
			{
				MapMarkerDef item = new MapMarkerDef
				{
					Category = "Extract",
					ShowInRaid = false,
					ImagePath = "Markers/exit.png",
					Text = scavExfiltrationPoint.Settings.Name.BSGLocalized(),
					Position = MathUtils.ConvertToMapPosition(scavExfiltrationPoint.transform),
					Color = DumpUtils.ExtractScavColor
				};
				list.Add(item);
			}
			foreach (ExfiltrationPoint exfiltrationPoint in exfiltrationPoints)
			{
				MapMarkerDef item2 = new MapMarkerDef
				{
					Category = "Extract",
					ShowInRaid = false,
					ImagePath = "Markers/exit.png",
					Text = exfiltrationPoint.Settings.Name.BSGLocalized(),
					Position = MathUtils.ConvertToMapPosition(exfiltrationPoint.transform),
					Color = DumpUtils.ExtractPmcColor
				};
				list.Add(item2);
			}
			foreach (TransitPoint transitPoint in LocationScene.GetAllObjects<TransitPoint>(true))
			{
				MapMarkerDef item3 = new MapMarkerDef
				{
					Category = "Transit",
					ShowInRaid = false,
					ImagePath = "Makers/transit.png",
					Text = transitPoint.parameters.description.BSGLocalized(),
					Position = MathUtils.ConvertToMapPosition(transitPoint.transform),
					Color = DumpUtils.TransitColor
				};
				list.Add(item3);
			}
			foreach (SecretExfiltrationPoint secretExfiltrationPoint in secretExfiltrationPoints)
			{
				MapMarkerDef item4 = new MapMarkerDef
				{
					Category = "Secret",
					ShowInRaid = false,
					ImagePath = "Markers/exit.png",
					Text = secretExfiltrationPoint.Settings.Name.BSGLocalized(),
					Position = MathUtils.ConvertToMapPosition(secretExfiltrationPoint.transform),
					Color = DumpUtils.SecretColor
				};
				list.Add(item4);
			}
			string currentMapInternalName = GameUtils.GetCurrentMapInternalName();
			string text = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(Path.Combine(Plugin.Path, currentMapInternalName + "-extracts.json"), text);
			Plugin.Log.LogInfo("Dumped extracts");
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000286C File Offset: 0x00000A6C
		public static void DumpSwitches()
		{
			Switch[] array = Object.FindObjectsOfType<Switch>();
			List<MapMarkerDef> list = new List<MapMarkerDef>();
			foreach (Switch @switch in array)
			{
				if (@switch.Operatable && @switch.HasAuthority)
				{
					MapMarkerDef item = new MapMarkerDef
					{
						Category = "Switch",
						ImagePath = "Markers/lever.png",
						Text = @switch.name,
						Position = MathUtils.ConvertToMapPosition(@switch.transform)
					};
					list.Add(item);
				}
			}
			string currentMapInternalName = GameUtils.GetCurrentMapInternalName();
			string text = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(Path.Combine(Plugin.Path, currentMapInternalName + "-switches.json"), text);
			Plugin.Log.LogInfo("Dumped switches");
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000292C File Offset: 0x00000B2C
		public static void DumpLocks()
		{
			WorldInteractiveObject[] array = Object.FindObjectsOfType<WorldInteractiveObject>();
			List<MapMarkerDef> list = new List<MapMarkerDef>();
			int num = 1;
			foreach (WorldInteractiveObject worldInteractiveObject in array)
			{
				if (!string.IsNullOrEmpty(worldInteractiveObject.KeyId) && worldInteractiveObject.Operatable)
				{
					MapMarkerDef item = new MapMarkerDef
					{
						Text = string.Format("door {0}", num++),
						Category = "Locked Door",
						ImagePath = "Markers/door_with_lock.png",
						Position = MathUtils.ConvertToMapPosition(worldInteractiveObject.transform),
						AssociatedItemId = worldInteractiveObject.KeyId,
						Color = DumpUtils.LockedDoorColor
					};
					list.Add(item);
				}
			}
			string currentMapInternalName = GameUtils.GetCurrentMapInternalName();
			string text = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(Path.Combine(Plugin.Path, currentMapInternalName + "-locked.json"), text);
			Plugin.Log.LogInfo("Dumped locks");
		}

		// Token: 0x0400001A RID: 26
		private const string ExtractCategory = "Extract";

		// Token: 0x0400001B RID: 27
		private const string ExtractImagePath = "Markers/exit.png";

		// Token: 0x0400001C RID: 28
		private const string SecretCategory = "Secret";

		// Token: 0x0400001D RID: 29
		private const string SecretImagePath = "Markers/exit.png";

		// Token: 0x0400001E RID: 30
		private const string TransitCategory = "Transit";

		// Token: 0x0400001F RID: 31
		private const string TransitImagePath = "Makers/transit.png";

		// Token: 0x04000020 RID: 32
		private static readonly Color ExtractScavColor = Color.Lerp(Color.yellow, Color.red, 0.5f);

		// Token: 0x04000021 RID: 33
		private static readonly Color TransitColor = Color.Lerp(Color.yellow, Color.red, 0.6f);

		// Token: 0x04000022 RID: 34
		private static readonly Color SecretColor = new Color(0.1f, 0.6f, 0.6f);

		// Token: 0x04000023 RID: 35
		private static readonly Color ExtractPmcColor = Color.green;

		// Token: 0x04000024 RID: 36
		private const string SwitchCategory = "Switch";

		// Token: 0x04000025 RID: 37
		private const string SwitchImagePath = "Markers/lever.png";

		// Token: 0x04000026 RID: 38
		private const string LockedDoorCategory = "Locked Door";

		// Token: 0x04000027 RID: 39
		private const string LockedDoorImagePath = "Markers/door_with_lock.png";

		// Token: 0x04000028 RID: 40
		private static readonly Color LockedDoorColor = Color.yellow;
	}
}
