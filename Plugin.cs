using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using DrakiaXYZ.VersionChecker;
using DynamicMaps.Config;
using DynamicMaps.ExternalModSupport;
using DynamicMaps.Patches;
using DynamicMaps.UI;
using DynamicMaps.Utils;
using EFT;
using EFT.UI;
using EFT.UI.Map;

namespace DynamicMaps
{
	// Token: 0x02000007 RID: 7
	[BepInPlugin("com.mpstark.DynamicMaps", "DynamicMaps", "0.6.0")]
	[BepInDependency("com.SPT.custom", "3.11.0")]
	[BepInDependency("com.SamSWAT.HeliCrash.ArysReloaded", 2)]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000237F File Offset: 0x0000057F
		public static ManualLogSource Log
		{
			get
			{
				return Plugin.Instance.Logger;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000238C File Offset: 0x0000058C
		internal void Awake()
		{
			if (!VersionChecker.CheckEftVersion(base.Logger, base.Info, base.Config))
			{
				throw new Exception("Invalid EFT Version");
			}
			ModDetection.CheckforMods();
			Settings.Init(base.Config);
			base.Config.SettingChanged += delegate(object x, SettingChangedEventArgs y)
			{
				ModdedMapScreen map = this.Map;
				if (map == null)
				{
					return;
				}
				map.ReadConfig();
			};
			Plugin.Instance = this;
			new BattleUIScreenShowPatch().Enable();
			new CommonUIAwakePatch().Enable();
			new MapScreenShowPatch().Enable();
			new MapScreenClosePatch().Enable();
			new GameStartedPatch().Enable();
			new GameWorldOnDestroyPatch().Enable();
			new GameWorldUnregisterPlayerPatch().Enable();
			new GameWorldRegisterLootItemPatch().Enable();
			new GameWorldDestroyLootPatch().Enable();
			new AirdropBoxOnBoxLandPatch().Enable();
			new PlayerOnDeadPatch().Enable();
			new PlayerInventoryThrowItemPatch().Enable();
			new ShowViewButtonPatch().Enable();
			new MenuLoadPatch().Enable();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002478 File Offset: 0x00000678
		internal void TryAttachToMapScreen(MapScreen mapScreen)
		{
			if (this.Map != null)
			{
				return;
			}
			Plugin.Log.LogInfo("Trying to attach to MapScreen");
			this.Map = ModdedMapScreen.Create(Singleton<CommonUI>.Instance.gameObject);
			this.Map.transform.SetParent(mapScreen.transform);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024C8 File Offset: 0x000006C8
		internal void TryAttachToBattleUIScreen(EftBattleUIScreen battleUI)
		{
			if (this.Map == null || GameUtils.GetMainPlayer() is HideoutPlayer)
			{
				return;
			}
			this.Map.TryAddPeekComponent(battleUI);
		}

		// Token: 0x04000012 RID: 18
		public const int TarkovVersion = 39390;

		// Token: 0x04000013 RID: 19
		public static Plugin Instance;

		// Token: 0x04000014 RID: 20
		public static string Path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		// Token: 0x04000015 RID: 21
		public ModdedMapScreen Map;
	}
}
