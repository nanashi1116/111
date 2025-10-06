using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x0200002C RID: 44
	internal class PlayerOnDeadPatch : ModulePatch
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060001AA RID: 426 RVA: 0x0000A3E4 File Offset: 0x000085E4
		// (remove) Token: 0x060001AB RID: 427 RVA: 0x0000A418 File Offset: 0x00008618
		internal static event Action<Player> OnDead;

		// Token: 0x060001AC RID: 428 RVA: 0x0000A44B File Offset: 0x0000864B
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(Player), "OnDead", null, null);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000A463 File Offset: 0x00008663
		[PatchPostfix]
		public static void PatchPostfix(Player __instance)
		{
			Action<Player> onDead = PlayerOnDeadPatch.OnDead;
			if (onDead == null)
			{
				return;
			}
			onDead(__instance);
		}
	}
}
