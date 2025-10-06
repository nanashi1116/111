using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using DynamicMaps.Utils;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x0200002D RID: 45
	internal class PlayerInventoryThrowItemPatch : ModulePatch
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x060001AF RID: 431 RVA: 0x0000A480 File Offset: 0x00008680
		// (remove) Token: 0x060001B0 RID: 432 RVA: 0x0000A4B4 File Offset: 0x000086B4
		internal static event Action<int, Item> OnThrowItem;

		// Token: 0x060001B1 RID: 433 RVA: 0x0000A4E8 File Offset: 0x000086E8
		protected override MethodBase GetTargetMethod()
		{
			if (!this._hasRegisteredEvents)
			{
				Action value;
				if ((value = PlayerInventoryThrowItemPatch.<>O.<0>__OnRaidEnd) == null)
				{
					value = (PlayerInventoryThrowItemPatch.<>O.<0>__OnRaidEnd = new Action(PlayerInventoryThrowItemPatch.OnRaidEnd));
				}
				GameWorldOnDestroyPatch.OnRaidEnd += value;
				Action<LootItem> value2;
				if ((value2 = PlayerInventoryThrowItemPatch.<>O.<1>__OnDestroyLoot) == null)
				{
					value2 = (PlayerInventoryThrowItemPatch.<>O.<1>__OnDestroyLoot = new Action<LootItem>(PlayerInventoryThrowItemPatch.OnDestroyLoot));
				}
				GameWorldDestroyLootPatch.OnDestroyLoot += value2;
				this._hasRegisteredEvents = true;
			}
			return AccessTools.Method(typeof(Player.PlayerInventoryController), "ThrowItem", null, null);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000A55C File Offset: 0x0000875C
		[PatchPostfix]
		public static void PatchPostfix(Player.PlayerInventoryController __instance, Item item)
		{
			Player x = PlayerInventoryThrowItemPatch._playerInventoryControllerPlayerField.GetValue(__instance) as Player;
			if (x == null || x != GameUtils.GetMainPlayer())
			{
				return;
			}
			int hashCode = item.Id.GetHashCode();
			PlayerInventoryThrowItemPatch.ThrownItems[hashCode] = item;
			Action<int, Item> onThrowItem = PlayerInventoryThrowItemPatch.OnThrowItem;
			if (onThrowItem == null)
			{
				return;
			}
			onThrowItem(hashCode, item);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000A5BA File Offset: 0x000087BA
		internal static void OnDestroyLoot(LootItem lootItem)
		{
			if (lootItem == null || lootItem.Item == null)
			{
				return;
			}
			PlayerInventoryThrowItemPatch.ThrownItems.Remove(lootItem.GetNetId());
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000A5DF File Offset: 0x000087DF
		internal static void OnRaidEnd()
		{
			PlayerInventoryThrowItemPatch.ThrownItems.Clear();
		}

		// Token: 0x040000E7 RID: 231
		private static FieldInfo _playerInventoryControllerPlayerField = AccessTools.Field(typeof(Player.PlayerInventoryController), "player_0");

		// Token: 0x040000E9 RID: 233
		internal static Dictionary<int, Item> ThrownItems = new Dictionary<int, Item>();

		// Token: 0x040000EA RID: 234
		private bool _hasRegisteredEvents;

		// Token: 0x0200005A RID: 90
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001FA RID: 506
			public static Action <0>__OnRaidEnd;

			// Token: 0x040001FB RID: 507
			public static Action<LootItem> <1>__OnDestroyLoot;
		}
	}
}
