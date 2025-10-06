using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000024 RID: 36
	internal class GameWorldOnDestroyPatch : ModulePatch
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000189 RID: 393 RVA: 0x00009CE4 File Offset: 0x00007EE4
		// (remove) Token: 0x0600018A RID: 394 RVA: 0x00009D18 File Offset: 0x00007F18
		internal static event Action OnRaidEnd;

		// Token: 0x0600018B RID: 395 RVA: 0x00009D4B File Offset: 0x00007F4B
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(GameWorld), "OnDestroy", null, null);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00009D64 File Offset: 0x00007F64
		[PatchPrefix]
		public static void PatchPrefix()
		{
			try
			{
				Action onRaidEnd = GameWorldOnDestroyPatch.OnRaidEnd;
				if (onRaidEnd != null)
				{
					onRaidEnd();
				}
				GameStartedPatch.HiddenStashes.Clear();
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Caught error while doing end of raid calculations");
				Plugin.Log.LogError(ex.Message ?? "");
				Plugin.Log.LogError(ex.StackTrace ?? "");
			}
		}
	}
}
