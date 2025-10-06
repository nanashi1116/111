using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000023 RID: 35
	internal class GameStartedPatch : ModulePatch
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00009C2F File Offset: 0x00007E2F
		public static List<LootableContainer> HiddenStashes { get; } = new List<LootableContainer>();

		// Token: 0x06000185 RID: 389 RVA: 0x00009C36 File Offset: 0x00007E36
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(GameWorld), "OnGameStarted", null, null);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00009C50 File Offset: 0x00007E50
		[PatchPostfix]
		public static void PatchPostfix(GameWorld __instance)
		{
			foreach (LootableContainer item in Enumerable.ToList<LootableContainer>(Enumerable.Where<LootableContainer>(LocationScene.GetAllObjects<LootableContainer>(false), (LootableContainer x) => x.name.StartsWith("scontainer_wood_CAP") || x.name.StartsWith("scontainer_Blue_Barrel_Base_Cap"))))
			{
				GameStartedPatch.HiddenStashes.Add(item);
			}
		}
	}
}
