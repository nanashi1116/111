using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000027 RID: 39
	internal class GameWorldDestroyLootPatch : ModulePatch
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000198 RID: 408 RVA: 0x00009F38 File Offset: 0x00008138
		// (remove) Token: 0x06000199 RID: 409 RVA: 0x00009F6C File Offset: 0x0000816C
		internal static event Action<LootItem> OnDestroyLoot;

		// Token: 0x0600019A RID: 410 RVA: 0x00009F9F File Offset: 0x0000819F
		protected override MethodBase GetTargetMethod()
		{
			return Enumerable.First<MethodInfo>(typeof(GameWorld).GetMethods(BindingFlags.Instance | BindingFlags.Public), delegate(MethodInfo m)
			{
				if (m.Name == "DestroyLoot")
				{
					return Enumerable.FirstOrDefault<ParameterInfo>(m.GetParameters(), (ParameterInfo p) => p.Name == "loot") != null;
				}
				return false;
			});
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00009FD8 File Offset: 0x000081D8
		[PatchPrefix]
		public static void PatchPrefix(object loot)
		{
			try
			{
				LootItem lootItem = loot as LootItem;
				if (lootItem != null)
				{
					Action<LootItem> onDestroyLoot = GameWorldDestroyLootPatch.OnDestroyLoot;
					if (onDestroyLoot != null)
					{
						onDestroyLoot(lootItem);
					}
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Caught error while running DestroyLoot patch");
				Plugin.Log.LogError(ex.Message ?? "");
				Plugin.Log.LogError(ex.StackTrace ?? "");
			}
		}
	}
}
