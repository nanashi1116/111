using System;
using System.Reflection;
using EFT.UI;
using EFT.UI.Map;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000022 RID: 34
	internal class CommonUIAwakePatch : ModulePatch
	{
		// Token: 0x06000181 RID: 385 RVA: 0x00009BDB File Offset: 0x00007DDB
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(CommonUI), "Awake", null, null);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00009BF4 File Offset: 0x00007DF4
		[PatchPostfix]
		public static void PatchPostfix(CommonUI __instance)
		{
			MapScreen value = Traverse.Create(__instance.InventoryScreen).Field("_mapScreen").GetValue<MapScreen>();
			Plugin.Instance.TryAttachToMapScreen(value);
		}
	}
}
