using System;
using System.Reflection;
using EFT;
using EFT.Interactive;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000026 RID: 38
	internal class GameWorldRegisterLootItemPatch : ModulePatch
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000193 RID: 403 RVA: 0x00009E88 File Offset: 0x00008088
		// (remove) Token: 0x06000194 RID: 404 RVA: 0x00009EBC File Offset: 0x000080BC
		internal static event Action<LootItem> OnRegisterLoot;

		// Token: 0x06000195 RID: 405 RVA: 0x00009EEF File Offset: 0x000080EF
		protected override MethodBase GetTargetMethod()
		{
			return typeof(GameWorld).GetMethod("RegisterLoot").MakeGenericMethod(new Type[]
			{
				typeof(LootItem)
			});
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00009F1D File Offset: 0x0000811D
		[PatchPostfix]
		public static void PatchPostfix(LootItem loot)
		{
			Action<LootItem> onRegisterLoot = GameWorldRegisterLootItemPatch.OnRegisterLoot;
			if (onRegisterLoot == null)
			{
				return;
			}
			onRegisterLoot(loot);
		}
	}
}
