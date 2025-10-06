using System;
using System.Reflection;
using DynamicMaps.UI;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;

namespace DynamicMaps.Patches
{
	// Token: 0x0200002B RID: 43
	internal class MenuLoadPatch : ModulePatch
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x0000A2D8 File Offset: 0x000084D8
		protected override MethodBase GetTargetMethod()
		{
			return PatchConstants.SingleCustom<Type>(PatchConstants.EftTypes, (Type x) => x.GetField("Taxonomy", BindingFlags.Instance | BindingFlags.Public) != null).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000A310 File Offset: 0x00008510
		[PatchPrefix]
		public static void PatchPostfix()
		{
			try
			{
				if (!MenuLoadPatch.serverConfigLoaded)
				{
					MenuLoadPatch.serverConfigLoaded = true;
					Plugin.Log.LogInfo("MenuLoadPatch: Using client-only mode, skipping server config loading");
					ModdedMapScreen._serverConfig = new ModdedMapScreen.DMServerConfig
					{
						allowShowFriendlyPlayerMarkersInRaid = true,
						allowShowEnemyPlayerMarkersInRaid = true,
						allowShowBossMarkersInRaid = true,
						allowShowScavMarkersInRaid = true
					};
					Plugin.Log.LogInfo(string.Format("MenuLoadPatch: Set client-only config - allowShowEnemyPlayerMarkersInRaid: {0}", ModdedMapScreen._serverConfig.allowShowEnemyPlayerMarkersInRaid));
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("MenuLoadPatch: Caught error while setting up client-only config");
				Plugin.Log.LogError("MenuLoadPatch: " + ex.Message);
				Plugin.Log.LogError("MenuLoadPatch: " + ex.StackTrace);
			}
		}

		// Token: 0x040000E5 RID: 229
		private static bool serverConfigLoaded;
	}
}
