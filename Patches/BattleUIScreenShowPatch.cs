using System;
using System.Reflection;
using DynamicMaps.Utils;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000021 RID: 33
	internal class BattleUIScreenShowPatch : ModulePatch
	{
		// Token: 0x0600017E RID: 382 RVA: 0x00009B8A File Offset: 0x00007D8A
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(EftBattleUIScreen), "Show", new Type[]
			{
				typeof(GamePlayerOwner)
			}, null);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009BB4 File Offset: 0x00007DB4
		[PatchPostfix]
		public static void PatchPostfix(EftBattleUIScreen __instance)
		{
			BattleUIScreenShowPatch.IsAttached = GameUtils.ShouldShowMapInRaid();
			if (!BattleUIScreenShowPatch.IsAttached)
			{
				return;
			}
			Plugin.Instance.TryAttachToBattleUIScreen(__instance);
		}

		// Token: 0x040000DD RID: 221
		public static bool IsAttached;
	}
}
