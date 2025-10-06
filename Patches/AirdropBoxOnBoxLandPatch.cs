using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using EFT.SynchronizableObjects;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000020 RID: 32
	internal class AirdropBoxOnBoxLandPatch : ModulePatch
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000177 RID: 375 RVA: 0x00009A74 File Offset: 0x00007C74
		// (remove) Token: 0x06000178 RID: 376 RVA: 0x00009AA8 File Offset: 0x00007CA8
		internal static event Action<AirdropSynchronizableObject> OnAirdropLanded;

		// Token: 0x06000179 RID: 377 RVA: 0x00009ADC File Offset: 0x00007CDC
		protected override MethodBase GetTargetMethod()
		{
			if (!this._hasRegisteredEvents)
			{
				Action value;
				if ((value = AirdropBoxOnBoxLandPatch.<>O.<0>__OnRaidEnd) == null)
				{
					value = (AirdropBoxOnBoxLandPatch.<>O.<0>__OnRaidEnd = new Action(AirdropBoxOnBoxLandPatch.OnRaidEnd));
				}
				GameWorldOnDestroyPatch.OnRaidEnd += value;
				this._hasRegisteredEvents = true;
			}
			return AccessTools.Method(typeof(AirdropLogicClass), "method_3", null, null);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00009B2E File Offset: 0x00007D2E
		[PatchPostfix]
		public static void PatchPostfix(AirdropLogicClass __instance)
		{
			if (__instance != null && !AirdropBoxOnBoxLandPatch.Airdrops.Contains(__instance.AirdropSynchronizableObject_0))
			{
				AirdropBoxOnBoxLandPatch.Airdrops.Add(__instance.AirdropSynchronizableObject_0);
				Action<AirdropSynchronizableObject> onAirdropLanded = AirdropBoxOnBoxLandPatch.OnAirdropLanded;
				if (onAirdropLanded == null)
				{
					return;
				}
				onAirdropLanded(__instance.AirdropSynchronizableObject_0);
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00009B6A File Offset: 0x00007D6A
		internal static void OnRaidEnd()
		{
			AirdropBoxOnBoxLandPatch.Airdrops.Clear();
		}

		// Token: 0x040000DB RID: 219
		internal static List<AirdropSynchronizableObject> Airdrops = new List<AirdropSynchronizableObject>();

		// Token: 0x040000DC RID: 220
		private bool _hasRegisteredEvents;

		// Token: 0x02000055 RID: 85
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001F0 RID: 496
			public static Action <0>__OnRaidEnd;
		}
	}
}
